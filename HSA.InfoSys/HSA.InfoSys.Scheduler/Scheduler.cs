// ------------------------------------------------------------------------
// <copyright file="Scheduler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Scheduler
{
    using System.Threading;
    using HSA.InfoSys.Common.DBManager;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class watches the scheduling objects in database
    /// and runs a task when necessary.
    /// </summary>
    public class Scheduler
    {
        /// <summary>
        /// The thread logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("Scheduler");

        /// <summary>
        /// The scheduler thread.
        /// </summary>
        private Thread schedulerThread;

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// The run flag for running this thread.
        /// </summary>
        private bool run = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scheduler"/> class.
        /// </summary>
        public Scheduler()
        {
            this.schedulerThread = new Thread(new ThreadStart(this.Run));
            this.dbManager = DBManager.Manager;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.schedulerThread.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.run = false;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            while (this.run)
            {
            }
        }
    }
}
