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
        /// The job dispatcher timer.
        /// </summary>
        private Countdown jobDispatcherTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        public Scheduler()
        {
            this.dbManager = DBManager.Manager;

            this.jobDispatcherTimer = new Countdown();
            this.jobDispatcherTimer.OnZero += this.JobDispatcherTimer_OnZero;
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public override void StartService()
        {
            var errorMessage = string.Empty;
            var timeSpan = new TimeSpan(0, Properties.Settings.Default.JOB_DISPATCHER_TIME, 0);
            var time = new Time(timeSpan, TypeOfTime.Timespan, true);

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
            this.jobDispatcherTimer.Stop(cancel);
            base.StopService();
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
            Log.Debug("Job dispatcher timer reached zero.");

            var errorMessage = string.Empty;
            this.FetchJobsFromDB();

            var repeatTime = new TimeSpan(0, Properties.Settings.Default.JOB_DISPATCHER_TIME, 0);
            var time = this.jobDispatcherTimer.SetTimeToRepeat(repeatTime, TypeOfTime.Timespan, true);
            this.jobDispatcherTimer.Start(time, out errorMessage);

            Log.Debug("Job dispatcher timer restarted.");

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
            Log.Debug("Fetch jobs from database.");

            IList<SchedulerTime> scheduler = this.dbManager.GetSchedulerTimes();

            foreach (var time in scheduler)
            {
                if (!this.schedulerTimes.ContainsKey(time.EntityId))
                {
                    this.schedulerTimes.Add(time.EntityId, time);
                }
            }
        }
    }
}
