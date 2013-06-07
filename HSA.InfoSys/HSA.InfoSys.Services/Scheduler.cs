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
    using HSA.InfoSys.Common.Services.Data;
    using HSA.InfoSys.Common.Timing;
    using log4net;

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
        /// The scheduler times.
        /// </summary>
        private Dictionary<Guid, OrgUnitConfig> orgUnitConfigurations = new Dictionary<Guid, OrgUnitConfig>();

        /// <summary>
        /// The failed configurations dictionary.
        /// </summary>
        private Dictionary<Guid, OrgUnitConfig> failedConfigs = new Dictionary<Guid, OrgUnitConfig>();

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

                if (this.orgUnitConfigurations.ContainsKey(orgConfig.EntityId))
                {
                    this.orgUnitConfigurations[orgConfig.EntityId] = orgConfig;
                }
                else
                {
                    this.orgUnitConfigurations.Add(orgConfig.EntityId, orgConfig);
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
                job.OnZero -= this.Countdown_OnZero;
                job.OnError -= this.Countdown_OnError;
#if DEBUG
                job.OnTick += this.Countdown_OnTick;
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
        public void Countdown_OnZero(object sender, object source)
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
        public void Countdown_OnError(object sender, string error)
        {
            var job = sender as Countdown;
            Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_ERROR, job, error);
        }

#if DEBUG
        public void Countdown_OnTick(object sender)
        {
            var countdown = sender as Countdown;
            Log.DebugFormat(Properties.Resources.SCHEDULER_ON_TICK, countdown.Time.RemainTime);
        }
#endif

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            /*var configs = DBManager.Session.QueryOver<OrgUnitConfig>().List<OrgUnitConfig>();

            mutex.WaitOne();

            foreach (var config in configs)
            {
                this.orgUnitConfigurations.Add(config.EntityId, config);
            }

            mutex.ReleaseMutex();

            base.StartService();*/
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

            mutex.ReleaseMutex();

            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            var errorMessage = string.Empty;

            while (this.Running)
            {
                mutex.WaitOne();

                this.SetCountdownTimes();
                this.RemoveFailedConfigurations();
                this.RemoveHandledConfigs();

                mutex.ReleaseMutex();

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Sets the countdown times.
        /// </summary>
        private void SetCountdownTimes()
        {
            if (this.orgUnitConfigurations.Count > 0)
            {
                foreach (var config in this.orgUnitConfigurations.Values)
                {
                    if (!this.jobs.ContainsKey(config.EntityId) && config.SchedulerActive)
                    {
                        var now = DateTime.Now;
                        var startTime = now;

                        var endTime = new DateTime(now.Year, now.Month, now.Day, config.Time, 0, 0);
                        var repeatIn = new TimeSpan(config.Days, config.Time, 0, 0);

                        var time = new Time(startTime, endTime, repeatIn, true);
                        var countdown = new Countdown(config, time);

                        countdown.OnZero += this.Countdown_OnZero;
                        countdown.OnError += this.Countdown_OnError;
#if DEBUG
                        countdown.OnTick += this.Countdown_OnTick;
#endif

                        this.StartCountdown(config, countdown);
                    }
                }
            }
        }

        /// <summary>
        /// Starts the countdown.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="countdown">The countdown.</param>
        private void StartCountdown(OrgUnitConfig config, Countdown countdown)
        {
            if (countdown.Start())
            {
                this.jobs.Add(config.EntityId, countdown);
            }
            else
            {
                if (!countdown.Time.IsTimeInFuture)
                {
                    if (countdown.Start(this.SetNextSearch(config, countdown)))
                    {
                        this.jobs.Add(config.EntityId, countdown);
                    }
                    else
                    {
                        this.failedConfigs.Add(config.EntityId, config);
                    }
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

        /// <summary>
        /// Removes the handled configurations.
        /// </summary>
        private void RemoveHandledConfigs()
        {
            if (this.orgUnitConfigurations.Count > 0)
            {
                foreach (var job in this.jobs.Values)
                {
                    var config = job.Source as OrgUnitConfig;

                    if (this.orgUnitConfigurations.ContainsKey(config.EntityId))
                    {
                        this.orgUnitConfigurations.Remove(config.EntityId);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the failed configurations.
        /// </summary>
        private void RemoveFailedConfigurations()
        {
            if (this.failedConfigs.Count > 0)
            {
                foreach (var config in this.failedConfigs.Values)
                {
                    if (this.orgUnitConfigurations.ContainsKey(config.EntityId))
                    {
                        this.orgUnitConfigurations.Remove(config.EntityId);
                    }
                }

                this.failedConfigs.Clear();
            }
        }
    }
}
