// ------------------------------------------------------------------------
// <copyright file="Service.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.CrawlController
{
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class represents the base functionality of a service.
    /// </summary>
    public abstract class Service : IService
    {
        /// <summary>
        /// The thread logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Scheduler");

        /// <summary>
        /// The scheduler thread.
        /// </summary>
        private Thread serviceThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        public Service()
        {
            this.serviceThread = new Thread(new ThreadStart(this.Run));
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Service"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        protected bool Running { get; set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void StartService()
        {
            Log.Debug(Properties.Resources.LOG_START_SERVICE);

            this.Running = true;
            this.serviceThread.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public virtual void StopService(bool cancel = false)
        {
            Log.Debug(Properties.Resources.LOG_STOP_SERVICE);

            this.Running = false;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected abstract void Run();
    }
}
