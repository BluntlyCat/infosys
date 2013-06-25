namespace HSA.InfoSys.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    public class FinishAndRepeatService : Service
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<String>.GetLogger("FinishAndRepeatService");

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public FinishAndRepeatService(Guid serviceGUID, string[] urls, ZeroEventHandler zeroEventHandler)
            : base(serviceGUID)
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
            this.URLs = urls;
            this.OnZero = zeroEventHandler;
        }

        /// <summary>
        /// The delegate for indicating that the time value is zero.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitConfig">The org unit config.</param>
        public delegate void ZeroEventHandler(object sender, string[] urls);

        /// <summary>
        /// Gets or sets the on zero event handler.
        /// </summary>
        /// <value>
        /// The on zero event handler.
        /// </value>
        private ZeroEventHandler OnZero { get; set; }

        /// <summary>
        /// Gets the URls.
        /// </summary>
        /// <value>
        /// The URls.
        /// </value>
        public string[] URLs { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_START_COUNTDOWN);
            base.StartService();
        }

        /// <summary>
        /// Sets the time to repeat.
        /// </summary>
        public void Restart()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_SET_REPEAT_TIME);
            this.StartService();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            this.Cancel = cancel;
            base.StopService();
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            if (!this.Cancel)
            {
                this.OnZero(this, this.URLs);
            }
        }
    }
}
