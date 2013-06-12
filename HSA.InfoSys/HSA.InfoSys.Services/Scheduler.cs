// ------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Nutch;
    using HSA.InfoSys.Common.Timing;
    using log4net;
    using HSA.InfoSys.Common.Entities;

#warning Suchvorgang muss noch gestartet werden.

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
        /// The mutex for the OrgUnitConfigurations dictionary.
        /// </summary>
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// The Nutch manager.
        /// </summary>
        private INutchManager nutchManager;

        /// <summary>
        /// The jobs dictionary.
        /// </summary>
        private Dictionary<Guid, Countdown> jobs = new Dictionary<Guid, Countdown>();

        /// <summary>
        /// Prevents a default instance of the <see cref="Scheduler"/> class from being created.
        /// </summary>
        private Scheduler()
        {
            Log.DebugFormat(Properties.Resources.LOG_INSTANCIATE_NEW_SCHEDULER, this.GetType().Name);

            this.dbManager = DBManager.ManagerFactory;
            this.nutchManager = NutchManager.ManagerFactory;
        }

        /// <summary>
        /// Gets the scheduler.
        /// </summary>
        /// <value>
        /// The scheduler.
        /// </value>
        public static Scheduler SchedulerFactory
        {
            get
            {
                if (scheduler == null)
                {
                    scheduler = new Scheduler();
                }

                return scheduler;
            }
        }

        /// <summary>
        /// Adds the scheduler time.
        /// </summary>
        /// <param name="orgConfig">The OrgUnitConfig.</param>
        public void AddOrgUnitConfig(OrgUnitConfig orgConfig)
        {
            if (orgConfig.SchedulerActive)
            {
                Log.DebugFormat(Properties.Resources.LOG_SCHEDULER_ADD, orgConfig);

                mutex.WaitOne();

                var job = this.SetNewJob(orgConfig);

                if (this.jobs.ContainsKey(orgConfig.EntityId) && job.Active)
                {
                    this.jobs[orgConfig.EntityId].Stop(true);
                    this.jobs[orgConfig.EntityId] = job;
                }
                else if (job.Active)
                {
                    this.jobs.Add(orgConfig.EntityId, job);
                }

                mutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes the scheduler time.
        /// </summary>
        /// <param name="orgUnitConfigGUID">The OrgUnitConfigGUID.</param>
        public void RemoveOrgUnitConfig(Guid orgUnitConfigGUID)
        {
            mutex.WaitOne();

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

            mutex.ReleaseMutex();
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
            var time = this.SetNextSearch(config, job);

            this.nutchManager.StartCrawl("michael", 1, 1);

            job.Start(time);
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
            // Log.DebugFormat(Properties.Resources.SCHEDULER_ON_TICK, job.Time.RemainTime);
            Console.WriteLine(string.Format(Properties.Resources.SCHEDULER_ON_TICK, job.Time.RemainTime));
        }
#endif

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            var configs = DBManager.Session.QueryOver<OrgUnitConfig>().List<OrgUnitConfig>();

            mutex.WaitOne();

            foreach (var config in configs)
            {
                var job = this.SetNewJob(config);

                if (job.Active)
                {
                    this.jobs.Add(config.EntityId, job);
                }
            }

            mutex.ReleaseMutex();

            base.StartService();
        }

        /// <summary>
        /// Stops all jobs.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            mutex.WaitOne();

            foreach (var job in this.jobs.Values)
            {
                job.Stop(cancel);
            }

            this.jobs.Clear();

            mutex.ReleaseMutex();

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
        /// <param name="config">The config.</param>
        /// <returns>A started countdown.</returns>
        private Countdown SetNewJob(OrgUnitConfig config)
        {
            Countdown job = null;

            if (!this.jobs.ContainsKey(config.EntityId) && config.SchedulerActive)
            {
                var now = DateTime.Now;
                var startTime = now;

#if DEBUG
                var endTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, config.Time);
                var repeatIn = new TimeSpan(0, 0, config.Days, config.Time);
#else
                var endTime = new DateTime(now.Year, now.Month, now.Day, config.Time, 0, 0);
                var repeatIn = new TimeSpan(config.Days, config.Time, 0, 0);
#endif
                var time = new Time(startTime, endTime, repeatIn, config.EntityId, true);

                job = new Countdown(config, config.EntityId, time);
                job.OnZero += new Countdown.ZeroEventHandler(this.Job_OnZero);
                job.OnError += new Countdown.ErrorEventHandler(this.Job_OnError);
#if DEBUG
                job.OnTick += new Countdown.TickEventHandler(this.Job_OnTick);
#endif

                this.StartJob(config, job);
            }

            return job;
        }

        /// <summary>
        /// Starts the countdown.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="job">The countdown.</param>
        private void StartJob(OrgUnitConfig config, Countdown job)
        {
            if (!job.Start())
            {
                if (!job.Time.IsTimeInFuture)
                {
                    job.Start(this.SetNextSearch(config, job));
                }
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
