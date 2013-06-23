// ------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.WCFServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.LocalServices;
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
        /// The job mutex.
        /// </summary>
        private Mutex jobMutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// The Nutch manager.
        /// </summary>
        private NutchController nutchController;

        /// <summary>
        /// The crawler
        /// </summary>
        private Countdown crawler;

        /// <summary>
        /// The jobs dictionary.
        /// </summary>
        private Dictionary<Guid, Countdown> jobs = new Dictionary<Guid, Countdown>();

        /// <summary>
        /// Prevents a default instance of the <see cref="Scheduler" /> class from being created.
        /// </summary>
        private Scheduler()
        {
            Log.DebugFormat(Properties.Resources.LOG_INSTANCIATE_NEW_SCHEDULER, this.GetType().Name);

            this.dbManager = DBManager.ManagerFactory;
            this.nutchController = NutchController.NutchFactory;
            this.nutchController.OnCrawlFinished += this.NutchController_OnCrawlFinished;

            var orgUnitConfigs = this.dbManager.GetOrgUnitConfigurations();
            var urlList = new List<string>();

            foreach (var config in orgUnitConfigs)
            {
                if (config.URLS != null)
                {
                    var urls = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(config.URLS);

                    foreach (var url in urls)
                    {
                        urlList.Add(url);
                    }
                }
            }

            var allURLs = Newtonsoft.Json.JsonConvert.SerializeObject(urlList.Distinct());

            var orgUnitConfig = this.dbManager.CreateOrgUnitConfig(
                allURLs,
                string.Empty,
                true,
                false,
                1,
                0,
                new DateTime(),
                true);

            this.crawler = new Countdown(
                orgUnitConfig,
                new CountdownTime(0, 1, true),
                new Countdown.ZeroEventHandler(this.CrawlFinished));
        }

        /// <summary>
        /// Gets the scheduler service.
        /// </summary>
        /// <value>
        /// The scheduler factory.
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
        /// <param name="orgUnitConfig">The OrgUnitConfig.</param>
        public void AddOrgUnitConfig(OrgUnitConfig orgUnitConfig)
        {
            if (orgUnitConfig.SchedulerActive)
            {
                Log.DebugFormat(Properties.Resources.LOG_SCHEDULER_ADD, orgUnitConfig);

                this.jobMutex.WaitOne();

                var job = this.SetNewJob(orgUnitConfig);

                if (job != null && this.jobs.ContainsKey(orgUnitConfig.EntityId) && job.Active)
                {
                    this.jobs[orgUnitConfig.EntityId].StopService(true);
                    this.jobs[orgUnitConfig.EntityId] = job;
                }
                else if (job != null && job.Active)
                {
                    this.jobs.Add(orgUnitConfig.EntityId, job);
                }

                this.jobMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Removes the scheduler time.
        /// </summary>
        /// <param name="orgUnitConfigGUID">The OrgUnitConfigGUID.</param>
        public void RemoveOrgUnitConfig(Guid orgUnitConfigGUID)
        {
            this.jobMutex.WaitOne();

            if (this.jobs.ContainsKey(orgUnitConfigGUID))
            {
                Log.DebugFormat(Properties.Resources.LOG_SCHEDULER_REMOVE, orgUnitConfigGUID);

                var job = this.jobs[orgUnitConfigGUID];
                job.StopService(true);
                job.OnError -= this.Job_OnError;
                job.OnTick -= this.Job_OnTick;

                this.jobs.Remove(orgUnitConfigGUID);
            }

            this.jobMutex.ReleaseMutex();
        }

        /// <summary>
        /// Occurs when [on crawl finished].
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void NutchController_OnCrawlFinished(object sender)
        {
            this.crawler.Restart();
        }

        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public void Job_OnError(object sender, string error)
        {
            var job = sender as Countdown;
            Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_ERROR, job, error);
        }

        /// <summary>
        /// Occurs when [tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="remainTime">The remain time.</param>
        public void Job_OnTick(object sender, TimeSpan remainTime)
        {
            var job = sender as Countdown;
            Log.InfoFormat(Properties.Resources.SCHEDULER_ON_TICK, job.ID, remainTime);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            var configs = DBManager.Session.QueryOver<OrgUnitConfig>()
                .Where(x => x.SchedulerActive)
                .List<OrgUnitConfig>();

            this.jobMutex.WaitOne();

            foreach (var config in configs)
            {
                var job = this.SetNewJob(config);

                if (job != null)
                {
                    this.jobs.Add(config.EntityId, job);
                    job.StartService();
                }
            }

            this.jobMutex.ReleaseMutex();

            this.crawler.StartService();

            base.StartService();
        }

        /// <summary>
        /// Stops all jobs.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            this.jobMutex.WaitOne();

            foreach (var job in this.jobs.Values)
            {
                job.StopService(cancel);
            }

            this.jobs.Clear();

            this.jobMutex.ReleaseMutex();

            this.crawler.StopService(cancel);

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }

        /// <summary>
        /// Crawls the finished.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitConfig">The org unit config.</param>
        private void CrawlFinished(object sender, OrgUnitConfig orgUnitConfig)
        {
            Log.InfoFormat(Properties.Resources.SCHEDULER_CRAWL_SUCCEEDED);

            var countdown = sender as Countdown;
            var urls = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(orgUnitConfig.URLS);
            var nutchController = NutchController.NutchFactory;

            nutchController.SetNextCrawl("crawler", 10, 10, urls);

            Log.Debug(Properties.Resources.SCHEDULER_CRAWL_RESTART);
            countdown.Restart();
        }

        /// <summary>
        /// Occurs when [zero].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitConfig">The org unit config.</param>
        private void StartSolrSearch(object sender, OrgUnitConfig orgUnitConfig)
        {
            var countdown = sender as Countdown;
            var solrController = new SolrSearchController();
            var orgUnitGUID = DBManager.Session.QueryOver<OrgUnit>()
                .Where(u => u.OrgUnitConfig.EntityId == orgUnitConfig.EntityId)
                .SingleOrDefault().EntityId;

            solrController.StartSearch(orgUnitGUID);

            if (countdown.Time.Repeat)
            {
                countdown.Restart();
            }
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

                var job = new Countdown(orgUnitConfig, orgUnitConfig.EntityId, new Countdown.ZeroEventHandler(this.StartSolrSearch));
                job.OnError += new Countdown.ErrorEventHandler(this.Job_OnError);
                job.OnTick += new Countdown.TickEventHandler(this.Job_OnTick);
                return job;
            }
            else if (orgUnitConfig.SchedulerActive)
            {
                return this.jobs[orgUnitConfig.EntityId];
            }

            return null;
        }
    }
}
