// ------------------------------------------------------------------------
// <copyright file="Countdown.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class represents the countdown thread.
    /// </summary>
    public class Countdown : Service
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Countdown");

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public Countdown(OrgUnitConfig orgUnitConfig, Guid serviceGUID, ZeroEventHandler zeroEventHandler) : base(serviceGUID)
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
            this.OrgUnitConfig = orgUnitConfig;
            this.OnZero = zeroEventHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        /// <param name="time">The time.</param>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public Countdown(OrgUnitConfig orgUnitConfig, CountdownTime time, Guid serviceGUID, ZeroEventHandler zeroEventHandler)
            : base(serviceGUID)
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
            this.OrgUnitConfig = orgUnitConfig;
            this.Time = time;
            this.OnZero = zeroEventHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public Countdown(CountdownTime time, Guid serviceGUID, ZeroEventHandler zeroEventHandler)
            : base(serviceGUID)
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
            this.Time = time;
            this.OnZero = zeroEventHandler;
        }

        /// <summary>
        /// The delegate for indicating that the time value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="remainTime">The remain time.</param>
        public delegate void TickEventHandler(object sender, TimeSpan remainTime);

        /// <summary>
        /// The delegate for indicating that the time value is zero.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="orgUnitConfig">The org unit config.</param>
        public delegate void ZeroEventHandler(object sender, OrgUnitConfig orgUnitConfig);

        /// <summary>
        /// The delegate for indicating that there was an error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="error">The error.</param>
        public delegate void ErrorEventHandler(object sender, string error);

        /// <summary>
        /// Occurs when [tick].
        /// </summary>
        public event TickEventHandler OnTick;

        /// <summary>
        /// Occurs when [on error].
        /// </summary>
        public event ErrorEventHandler OnError;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Countdown"/> is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if active; otherwise, <c>false</c>.
        /// </value>
        public bool Active { get; private set; }

        /// <summary>
        /// Gets the shutdown time.
        /// </summary>
        /// <value>
        /// The shutdown time.
        /// </value>
        public CountdownTime Time { get; private set; }

        /// <summary>
        /// Gets or sets the on zero event handler.
        /// </summary>
        /// <value>
        /// The on zero event handler.
        /// </value>
        private ZeroEventHandler OnZero { get; set; }

        /// <summary>
        /// Gets or sets the org unit config.
        /// </summary>
        /// <value>
        /// The org unit config.
        /// </value>
        private OrgUnitConfig OrgUnitConfig { get; set; }

        /// <summary>
        /// Updates the org unit config.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        public void UpdateOrgUnitConfig(OrgUnitConfig orgUnitConfig)
        {
            Log.InfoFormat(
                Properties.Resources.COUNTDOWN_SET_NEW_CONFIG,
                this.OrgUnitConfig,
                orgUnitConfig,
                this.OrgUnitConfig.EntityId);

            this.OrgUnitConfig = orgUnitConfig;
            this.ServiceGUID = orgUnitConfig.EntityId;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void StartService()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_START_COUNTDOWN);

            this.Active = true;

            this.SetTime();

            base.StartService();
        }

        /// <summary>
        /// Sets the time to repeat.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void RestartService(bool cancel = false)
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_SET_REPEAT_TIME);
            base.RestartService(cancel);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public override void StopService(bool cancel = false)
        {
            this.Cancel = cancel;
            this.Active = false;

            base.StopService();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(Properties.Resources.COUNTDOWN_TO_STRING, this.Time, this.OrgUnitConfig);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_IS_RUNNING);

            var runTime = this.Time.RepeatIn.Subtract(DateTime.Now);

#if DEBUG
            var sleep = 1000;
#else
            var sleep = 300000;
#endif

            while (this.Running && this.Active && runTime.TotalSeconds > 0)
            {
                if (this.OnTick != null)
                {
                    this.OnTick(this, runTime);
                }

                runTime = runTime.Subtract(new TimeSpan(0, 0, 0, 0, sleep));

                Thread.Sleep(sleep);
            }

            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_ENDS);

            this.Active = false;

            if (!this.Cancel)
            {
                this.OnZero(this, this.OrgUnitConfig);
            }
        }

        /// <summary>
        /// Sets the time.
        /// </summary>
        private void SetTime()
        {
            var now = DateTime.Now;
            DateTime repeatIn;

            try
            {
#if DEBUG
                repeatIn = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(this.OrgUnitConfig.Days);
#else
                repeatIn = new DateTime(now.Year, now.Month, now.Day, this.OrgUnitConfig.Time, 0, 0).AddDays(this.OrgUnitConfig.Days);
#endif
                this.Time = new CountdownTime(this.OrgUnitConfig.Time, this.OrgUnitConfig.Days, repeatIn, this.OrgUnitConfig.EntityId, true);
            }
            catch (Exception e)
            {
                this.OnError(this, e.Message);
            }
        }
    }
}
