// ------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Scheduling
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using HSA.InfoSys.Common.CrawlController;
    using HSA.InfoSys.Common.DBManager;
    using HSA.InfoSys.Common.DBManager.Data;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Timing;
    using log4net;

    /// <summary>
    /// This class watches the scheduling objects in database
    /// and runs a task when necessary.
    /// </summary>
    public class Scheduler : Service
    {
        /// <summary>
        /// The thread logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Scheduler");

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// The jobs list.
        /// </summary>
        private List<Countdown> jobs = new List<Countdown>();

        /// <summary>
        /// The scheduler times.
        /// </summary>
        private Dictionary<Guid, SchedulerTime> schedulerTimes = new Dictionary<Guid, SchedulerTime>();

        /// <summary>
        /// The time span for repeating the jobDispatcherTimer.
        /// </summary>
        private TimeSpan dispatcherTime;

        /// <summary>
        /// The job dispatcher timer.
        /// </summary>
        private Countdown jobDispatcherTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        public Scheduler()
        {
            Log.DebugFormat(Properties.Resources.LOG_INSTANCIATE_NEW_SCHEDULER, this.GetType().Name);

            this.dbManager = DBManager.Manager;

            this.jobDispatcherTimer = new Countdown();
            this.jobDispatcherTimer.OnZero += this.JobDispatcherTimer_OnZero;

            this.dispatcherTime = new TimeSpan(0, Properties.Settings.Default.JOB_DISPATCHER_TIME, 0);
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            Log.DebugFormat(Properties.Resources.LOG_START_SERVICE, this.GetType().Name);

            var errorMessage = string.Empty;
            var time = new Time(this.dispatcherTime, TypeOfTime.Timespan, true);

            this.jobDispatcherTimer.Start(time, out errorMessage);

            if (!errorMessage.Equals(string.Empty))
            {
                Log.ErrorFormat(Properties.Resources.ERROR_TIME_INIT, errorMessage);
            }

            base.StartService();
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            Log.DebugFormat(Properties.Resources.LOG_STOP_SERVICE, this.GetType().Name);

            this.jobDispatcherTimer.Stop(cancel);
            base.StopService(cancel);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            while (this.Running)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// If the job dispatcher timer finished its interval.
        /// </summary>
        private void JobDispatcherTimer_OnZero()
        {
            Log.Debug(Properties.Resources.LOG_DISPATCHER_ZERO);

            var errorMessage = string.Empty;
            this.FetchJobsFromDB();

            var time = this.jobDispatcherTimer.SetTimeToRepeat(this.dispatcherTime, TypeOfTime.Timespan, true);
            this.jobDispatcherTimer.Start(time, out errorMessage);

            Log.Debug(Properties.Resources.LOG_RESTART_DISPATCHER);

            if (!errorMessage.Equals(string.Empty))
            {
                Log.ErrorFormat(Properties.Resources.ERROR_TIME_INIT, errorMessage);
            }
        }

        /// <summary>
        /// Fetches the jobs from DB.
        /// </summary>
        private void FetchJobsFromDB()
        {
            Log.Debug(Properties.Resources.LOG_FETCH_JOBS_FROM_DB);

            IList<SchedulerTime> scheduler = this.dbManager.GetSchedulerTimes();
            Log.DebugFormat(Properties.Resources.LOG_GOT_SCHEDULER_LIST_FROM_DB, scheduler);

            foreach (var time in scheduler)
            {
                if (!this.schedulerTimes.ContainsKey(time.EntityId))
                {
                    this.schedulerTimes.Add(time.EntityId, time);
                    Log.DebugFormat(Properties.Resources.LOG_SCHEDULER_ADD, time);
                }
                else
                {
                    Log.Debug(Properties.Resources.LOG_SCHEDULER_ALREADY_EXIST);
                }
            }
        }
    }
}
