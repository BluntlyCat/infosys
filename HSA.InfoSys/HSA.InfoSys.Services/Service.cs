// ------------------------------------------------------------------------
// <copyright file="Service.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services
{
    using System;
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
        /// The service mutex.
        /// </summary>
        private Mutex serviceMutex = new Mutex();

        /// <summary>
        /// Initializes a new instance of the <see cref="Service"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        protected Service(Guid serviceGUID)
        {
            this.ServiceGUID = serviceGUID;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public Guid ServiceGUID { get; private set; }

        /// <summary>
        /// Gets the service thread.
        /// </summary>
        /// <value>
        /// The service thread.
        /// </value>
        protected Thread ServiceThread { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Service"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        protected bool Running { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Countdown"/> is canceled.
        /// </summary>
        /// <value>
        /// Cancel is <c>true</c> if cancels; otherwise, <c>false</c>.
        /// </value>
        protected bool Cancel { get; set; }

        /// <summary>
        /// Gets the service mutex.
        /// </summary>
        /// <value>
        /// The service mutex.
        /// </value>
        protected Mutex ServiceMutex
        {
            get
            {
                return this.serviceMutex;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void StartService()
        {
            Log.DebugFormat(Properties.Resources.LOG_START_SERVICE, this.GetType().Name, this.ServiceGUID);
            
            this.ServiceThread = new Thread(new ThreadStart(this.Run));

            this.Running = true;
            this.ServiceThread.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public virtual void StopService(bool cancel = false)
        {
            Log.WarnFormat(Properties.Resources.LOG_STOP_SERVICE, this.GetType().Name, this.ServiceGUID);
            this.Running = false;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected abstract void Run();
    }
}
