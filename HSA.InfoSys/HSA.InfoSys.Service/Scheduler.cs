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
    using HSA.InfoSys.Common.Services.Data;
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
        /// The database manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// The scheduler times.
        /// </summary>
        private Dictionary<Guid, SchedulerTime> schedulerTimes = new Dictionary<Guid, SchedulerTime>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        public Scheduler()
        {
            Log.DebugFormat(Properties.Resources.LOG_INSTANCIATE_NEW_SCHEDULER, this.GetType().Name);

            this.dbManager = DBManager.Manager;
        }

        /// <summary>
        /// Adds the scheduler time.
        /// </summary>
        /// <param name="schedulerTime">The scheduler time.</param>
        public void AddSchedulerTime(SchedulerTime schedulerTime)
        {
            Log.DebugFormat(Properties.Resources.LOG_SCHEDULER_ADD, schedulerTime);

            if (!this.schedulerTimes.ContainsKey(schedulerTime.EntityId))
            {
                this.schedulerTimes.Add(schedulerTime.EntityId, schedulerTime);
            }
        }

        /// <summary>
        /// Removes the scheduler time.
        /// </summary>
        /// <param name="schedulerTimeGUID">The scheduler time GUID.</param>
        public void RemoveSchedulerTime(Guid schedulerTimeGUID)
        {
            if (this.schedulerTimes.ContainsKey(schedulerTimeGUID))
            {
                this.schedulerTimes.Remove(schedulerTimeGUID);
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            while (this.Running)
            {
                foreach (var scheduler in this.schedulerTimes.Values)
                {
                }

                Thread.Sleep(1000);
            }
        }
    }
}
