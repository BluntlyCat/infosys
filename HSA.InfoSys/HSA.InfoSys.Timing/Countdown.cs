// ------------------------------------------------------------------------
// <copyright file="Countdown.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Timing
{
    using System;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class represents the countdown thread.
    /// </summary>
    public class Countdown
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<Type>.GetLogger(typeof(Countdown));

        /// <summary>
        /// The timer thread
        /// </summary>
        private Thread countdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        /// <param name="id">The id.</param>
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public Countdown(OrgUnitConfig orgUnitConfig, Guid id, ZeroEventHandler zeroEventHandler)
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
            this.OrgUnitConfig = orgUnitConfig;
            this.ID = id;
            this.OnZero = zeroEventHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="orgUnitConfig">The org unit config.</param>
        /// <param name="time">The time.</param>
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public Countdown(OrgUnitConfig orgUnitConfig, Time time, ZeroEventHandler zeroEventHandler)
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
        /// <param name="zeroEventHandler">The zero event handler.</param>
        public Countdown(Time time, ZeroEventHandler zeroEventHandler)
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
        public Time Time { get; private set; }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public Guid ID { get; private set; }

        /// <summary>
        /// Gets or sets the org unit config.
        /// </summary>
        /// <value>
        /// The org unit config.
        /// </value>
        private OrgUnitConfig OrgUnitConfig { get; set; }

        /// <summary>
        /// Gets or sets the on zero event handler.
        /// </summary>
        /// <value>
        /// The on zero event handler.
        /// </value>
        private ZeroEventHandler OnZero { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Countdown"/> is canceled.
        /// </summary>
        /// <value>
        /// Cancel is <c>true</c> if cancels; otherwise, <c>false</c>.
        /// </value>
        private bool Cancel { get; set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_START_COUNTDOWN);

            this.Active = true;

            this.SetTime();

            this.countdown = new Thread(new ThreadStart(this.Run));
            this.countdown.Start();
        }

        /// <summary>
        /// Sets the time to repeat.
        /// </summary>
        public void Restart()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_SET_REPEAT_TIME);

            this.SetTime();

            if (this.Time != null)
            {
                this.Start();
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        /// <param name="cancel">if set to <c>true</c> [cancel].</param>
        public void Stop(bool cancel = false)
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_STOP_COUNTDOWN);
            this.Cancel = cancel;
            this.Active = false;

            this.countdown.Interrupt();
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
                repeatIn = new DateTime(now.Year, now.Month, now.Day, orgUnitConfig.Time, 0, 0).AddDays(orgUnitConfig.Days);
#endif
                this.Time = new Time(this.OrgUnitConfig.Time, this.OrgUnitConfig.Days, repeatIn, this.OrgUnitConfig.EntityId, true);
            }
            catch (Exception e)
            {
                this.OnError(this, e.Message);
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_IS_RUNNING);

#if DEBUG
            var runTime = this.Time.RepeatIn.Subtract(DateTime.Now);

            while (this.Active && runTime.TotalSeconds > 0)
            {
                if (this.OnTick != null)
                {
                    this.OnTick(this, runTime);
                }

                runTime = runTime.Subtract(new TimeSpan(0, 0, 1));
                Thread.Sleep(1000);
            }
#else
            var runtime = this.Time.RepeatIn.Subtract(DateTime.Now).Ticks / 10000;
            Thread.Sleep((int)runtime);
#endif
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_ENDS);

            this.Active = false;

            if (!this.Cancel)
            {
                this.OnZero(this, this.OrgUnitConfig);
            }
        }
    }
}
