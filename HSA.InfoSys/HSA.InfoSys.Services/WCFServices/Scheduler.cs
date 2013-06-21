// ------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Timing;
    using log4net;

    /// <summary>
    /// This class watches the scheduling objects in database
    /// and runs a task when necessary.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Scheduler : Service, IScheduler
    {
        /// <summary>
        /// The thread logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Scheduler");

        /// <summary>
        /// The scheduler.
        /// </summary>
        private static Scheduler scheduler;

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// The Nutch manager.
        /// </summary>
        private NutchController nutchController;

        /// <summary>
        /// The jobs dictionary.
        /// </summary>
        private Dictionary<Guid, Countdown> jobs = new Dictionary<Guid, Countdown>();

        /// <summary>
        /// Prevents a default instance of the <see cref="Scheduler"/> class from being created.
        /// </summary>
        private Scheduler(NutchController nutchController)
        {
            Log.DebugFormat(Properties.Resources.LOG_INSTANCIATE_NEW_SCHEDULER, this.GetType().Name);

            this.dbManager = DBManager.ManagerFactory;
            this.nutchController = nutchController;
            this.nutchController.OnCrawlFinished += this.NutchController_OnCrawlFinished;
        }

        /// <summary>
        /// Gets the scheduler.
        /// </summary>
        /// <value>
        /// The scheduler.
        /// </value>
        public static Scheduler SchedulerFactory(NutchController nutchController)
        {
            if (scheduler == null)
            {
                scheduler = new Scheduler(nutchController);
            }

            return scheduler;
        }

        /// <summary>
        /// Adds the scheduler time.
        /// </summary>
        /// <param name="orgUnitConfig">The OrgUnitConfig.</param>
        public void AddOrgUnitConfig(OrgUnitConfig orgUnitConfig)
        {
            if (orgUnitConfig.SchedulerActive)
            {
                Log.DebugFormat(Properties.Resources.LOG_SCHEDULER_ADD, orgUnitConfig);

                this.ServiceMutex.WaitOne();

                var job = this.SetNewJob(orgUnitConfig);

                if (job != null && this.jobs.ContainsKey(orgUnitConfig.EntityId) && job.Active)
                {
                    this.jobs[orgUnitConfig.EntityId].Stop(true);
                    this.jobs[orgUnitConfig.EntityId] = job;
                }
                else if (job != null && job.Active)
                {
                    this.jobs.Add(orgUnitConfig.EntityId, job);
                }

                this.ServiceMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes the scheduler time.
        /// </summary>
        /// <param name="orgUnitConfigGUID">The OrgUnitConfigGUID.</param>
        public void RemoveOrgUnitConfig(Guid orgUnitConfigGUID)
        {
            this.ServiceMutex.WaitOne();

            if (this.jobs.ContainsKey(orgUnitConfigGUID))
            {
                var job = this.jobs[orgUnitConfigGUID];
                job.Stop(true);
                job.OnZero -= this.Job_OnZero;
                job.OnError -= this.Job_OnError;
#if DEBUG
                job.OnTick -= this.Job_OnTick;
#endif

                this.jobs.Remove(orgUnitConfigGUID);
            }

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Occurs when [zero].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="source">The source.</param>
        public void Job_OnZero(object sender, object source)
        {
            var job = sender as Countdown;
            var config = job.Source as OrgUnitConfig;

            var orgUnitGUID = DBManager.Session.QueryOver<OrgUnit>()
                .Where(u => u.OrgUnitConfig.EntityId == config.EntityId)
                .SingleOrDefault().EntityId;

            var time = this.SetNextSearch(config, job);
            job.Start(time);

#warning username must be set here
            this.nutchController.SetPendingCrawl(orgUnitGUID, "michael", 1, 1);
        }

        /// <summary>
        /// Occurs when [on crawl finished].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitGUID">The org unit GUID.</param>
        public void NutchController_OnCrawlFinished(object sender, Guid orgUnitGUID, bool success)
        {
            if (success)
            {
                Log.InfoFormat(Properties.Resources.SCHEDULER_CRAWL_SUCCEEDED, orgUnitGUID);
                var solrController = new SolrSearchController();
                solrController.StartSearch(orgUnitGUID);
            }
            else
            {
                try
                {
                    Log.ErrorFormat(Properties.Resources.SCHEDULER_CRAWL_FAILED, orgUnitGUID);
                    EmailNotifier mailNotifier = new EmailNotifier();
                    mailNotifier.CrawlFailed(orgUnitGUID);
                }
                catch (CommunicationException ce)
                {
                    Log.ErrorFormat(Properties.Resources.WCF_COMMUNICATION_ERROR, ce);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
            }
        }

        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void Job_OnError(object sender, string error)
        {
            var job = sender as Countdown;
            Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_ERROR, job, error);
        }

#if DEBUG
        /// <summary>
        /// Occurs when [on tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void Job_OnTick(object sender)
        {
            var job = sender as Countdown;

#warning should not be in this way but the call to the logger causes a freeze of the whole application
            Log.DebugFormat(Properties.Resources.SCHEDULER_ON_TICK, job.Time.RemainTime);
            //Console.WriteLine(string.Format(Properties.Resources.SCHEDULER_ON_TICK, job.Time.RemainTime));
        }
#endif

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            var configs = DBManager.Session.QueryOver<OrgUnitConfig>()
                .Where(x => x.SchedulerActive)
                .List<OrgUnitConfig>();

            this.ServiceMutex.WaitOne();

            foreach (var config in configs)
            {
                var job = this.SetNewJob(config);

                if (job != null && job.Active)
                {
                    this.jobs.Add(config.EntityId, job);
                }
            }

            this.ServiceMutex.ReleaseMutex();

            base.StartService();
        }

        /// <summary>
        /// Stops all jobs.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            this.ServiceMutex.WaitOne();

            foreach (var job in this.jobs.Values)
            {
                job.Stop(cancel);
            }

            this.jobs.Clear();

            this.ServiceMutex.ReleaseMutex();

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }

        /// <summary>
        /// Sets the countdown times.
        /// </summary>
        /// <param name="orgUnitConfig">The config.</param>
        /// <returns>A started countdown.</returns>
        private Countdown SetNewJob(OrgUnitConfig orgUnitConfig)
        {
            if (!this.jobs.ContainsKey(orgUnitConfig.EntityId) && orgUnitConfig.SchedulerActive)
            {
                Log.InfoFormat(Properties.Resources.SCHEDULER_CREATE_NEW_JOB, orgUnitConfig);

                DateTime endTime = DateTime.Now;
                TimeSpan repeatIn = new TimeSpan();

                var now = DateTime.Now;
                var startTime = now;

                try
                {
#if DEBUG
                    endTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).AddMinutes(orgUnitConfig.Days);
                    repeatIn = new TimeSpan(0, 0, orgUnitConfig.Days, orgUnitConfig.Time);
#else
                    var endTime = new DateTime(now.Year, now.Month, now.Day, orgUnitConfig.Time.RepeatIn.Hours, 0, 0);
                    var repeatIn = new TimeSpan(orgUnitConfig.Days, orgUnitConfig.Time, 0, 0);
#endif
                }
                catch
                {
                    EmailNotifier mailNotifier = new EmailNotifier();

                    var subject = "Wrong time format";
                    var body = string.Format("The scheduler time of system {0} could not be set.");

                    mailNotifier.SendMailToEntityOwner(orgUnitConfig, subject, body);

                    return null;
                }

                var time = new Time(startTime, endTime, repeatIn, orgUnitConfig.EntityId, true);

                var job = new Countdown(orgUnitConfig, orgUnitConfig.EntityId, time);
                job.OnZero += new Countdown.ZeroEventHandler(this.Job_OnZero);
                job.OnError += new Countdown.ErrorEventHandler(this.Job_OnError);
#if DEBUG
                job.OnTick += new Countdown.TickEventHandler(this.Job_OnTick);
#endif

                this.StartJob(orgUnitConfig, job);

                return job;
            }
            else if (orgUnitConfig.SchedulerActive)
            {
                return this.jobs[orgUnitConfig.EntityId];
            }

            Log.WarnFormat(Properties.Resources.SCHEDULER_CANT_SET_JOB, orgUnitConfig);

            return null;
        }

        /// <summary>
        /// Starts the countdown.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="job">The countdown.</param>
        private void StartJob(OrgUnitConfig config, Countdown job)
        {
            if (job.Time.IsTimeInFuture)
            {
                job.Start();
            }
            else
            {
                job.Start(this.SetNextSearch(config, job));
            }
        }

        /// <summary>
        /// Sets the next search.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="countdown">The countdown.</param>
        /// <returns>The time for the next search.</returns>
        private Time SetNextSearch(OrgUnitConfig config, Countdown countdown)
        {
            var time = countdown.SetTimeToRepeat();

            config.NextSearch = time.Endtime;
            this.dbManager.UpdateEntity(config);

            return time;
        }
    }
}
