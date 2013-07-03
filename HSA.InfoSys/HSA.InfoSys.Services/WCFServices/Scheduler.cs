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
        private readonly Mutex jobMutex = new Mutex();

        /// <summary>
        /// The database manager.
        /// </summary>
        private readonly IDbManager dbManager;

        /// <summary>
        /// The jobs dictionary.
        /// </summary>
        private readonly Dictionary<Guid, Countdown> jobs = new Dictionary<Guid, Countdown>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="dbManager">The db manager.</param>
        private Scheduler(Guid serviceGUID, IDbManager dbManager) : base(serviceGUID)
        {
            Log.DebugFormat(Properties.Resources.LOG_INSTANCIATE_NEW_SCHEDULER, this.GetType().Name);

            this.dbManager = dbManager;
        }

        /// <summary>
        /// The scheduler service.
        /// </summary>
        /// <param name="dbManager">The db manager.</param>
        /// <returns>
        /// An instance of the scheduler service.
        /// </returns>
        public static Scheduler SchedulerFactory(IDbManager dbManager)
        {
            return scheduler ?? (scheduler = new Scheduler(Guid.NewGuid(), dbManager));
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

                if (this.jobs.ContainsKey(orgUnitConfig.EntityId))
                {
                    var job = this.UpdateJob(orgUnitConfig);

                    if (job != null)
                    {
                        this.jobs[orgUnitConfig.EntityId].StopService(true);
                        this.jobs[orgUnitConfig.EntityId] = job;
                        this.jobs[orgUnitConfig.EntityId].StartService();
                    }
                }
                else
                {
                    var job = this.SetNewJob(orgUnitConfig);

                    if (job != null)
                    {
                        this.jobs.Add(orgUnitConfig.EntityId, job);
                        job.StartService();
                    }
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
        /// Occurs when [on error].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        private void Job_OnError(object sender, string error)
        {
            var job = sender as Countdown;
            Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_ERROR, job, error);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            var configs = this.dbManager.GetOrgUnitConfigsByActiveScheduler();

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

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
        }

        /// <summary>
        /// Occurs when [tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="remainTime">The remain time.</param>
        private void Job_OnTick(object sender, TimeSpan remainTime)
        {
            var job = sender as Countdown;

            if (job != null)
            {
                Log.InfoFormat(Properties.Resources.SCHEDULER_ON_TICK, job.ServiceGUID, remainTime);
            }
        }

        /// <summary>
        /// Occurs when [zero].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitConfig">The org unit config.</param>
        private void StartSolrSearch(object sender, OrgUnitConfig orgUnitConfig)
        {
            this.ServiceMutex.WaitOne();

            var countdown = sender as Countdown;
            var solrController = new SolrSearchController(this.dbManager);

            var orgUnitGUID = this.dbManager.GetOrgUnitGuidByOrgUnitConfigGuid(orgUnitConfig.EntityId);

            solrController.StartSearch(orgUnitGUID);

            if (countdown != null && (countdown.Time == null || countdown.Time.Repeat))
            {
                countdown.RestartService();
            }

            this.ServiceMutex.ReleaseMutex();
        }

        /// <summary>
        /// Sets the countdown times.
        /// </summary>
        /// <param name="orgUnitConfig">The config.</param>
        /// <returns>A started countdown.</returns>
        private Countdown SetNewJob(OrgUnitConfig orgUnitConfig)
        {
            if (orgUnitConfig.SchedulerActive)
            {
                Log.InfoFormat(Properties.Resources.SCHEDULER_CREATE_NEW_JOB, orgUnitConfig);

                var job = new Countdown(orgUnitConfig, orgUnitConfig.EntityId, this.StartSolrSearch);
                job.OnError += this.Job_OnError;
                job.OnTick += this.Job_OnTick;

                return job;
            }

            Log.WarnFormat(Properties.Resources.SCHEDULER_SKIP_NOT_ACTIVE_JOB, orgUnitConfig);

            return null;
        }

        /// <summary>
        /// Updates the job.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        /// <returns>The updated job.</returns>
        private Countdown UpdateJob(OrgUnitConfig orgUnitConfig)
        {
            if (orgUnitConfig.SchedulerActive)
            {
                Log.DebugFormat(Properties.Resources.SCHEDULER_UPDATE_JOB, orgUnitConfig);
                this.jobs[orgUnitConfig.EntityId].UpdateOrgUnitConfig(orgUnitConfig);
                return this.jobs[orgUnitConfig.EntityId];
            }

            Log.WarnFormat(Properties.Resources.SCHEDULER_SKIP_NOT_ACTIVE_JOB, orgUnitConfig);

            return null;
        }
    }
}
