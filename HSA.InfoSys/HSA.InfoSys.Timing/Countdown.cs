// ------------------------------------------------------------------------
// <copyright file="Countdown.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Timing
{
    using System;
    using System.Threading;
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
        /// The tick mutex.
        /// </summary>
        private static Mutex onTickMutex = new Mutex();

        /// <summary>
        /// The tick mutex.
        /// </summary>
        private static Mutex onErrorMutex = new Mutex();

        /// <summary>
        /// The tick mutex.
        /// </summary>
        private static Mutex onZeroMutex = new Mutex();

        /// <summary>
        /// The timer thread
        /// </summary>
        private Thread countdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="id">The id.</param>
        /// <param name="time">The time.</param>
        public Countdown(object source, Guid id, Time time = null)
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
            this.Source = source;
            this.ID = id;
            this.Time = time;
        }

        /// <summary>
        /// The delegate for indicating that the time value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public delegate void TickEventHandler(object sender);

        /// <summary>
        /// The delegate for indicating that the time value is zero.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="source">The source.</param>
        public delegate void ZeroEventHandler(object sender, object source);

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
        /// Occurs when [zero].
        /// </summary>
        public event ZeroEventHandler OnZero;

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
        /// Gets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public object Source { get; private set; }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public Guid ID { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Countdown"/> is canceled.
        /// </summary>
        /// <value>
        /// Cancel is <c>true</c> if cancels; otherwise, <c>false</c>.
        /// </value>
        private bool Cancel { get; set; }

        /// <summary>
        /// Sets the time to repeat.
        /// </summary>
        /// <returns>A new time instance for next countdown.</returns>
        public Time SetTimeToRepeat()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_SET_REPEAT_TIME);

            var now = DateTime.Now;
            var startTime = now;
#if DEBUG
            var endTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute + this.Time.RepeatIn.Minutes, this.Time.RepeatIn.Seconds);
#else
            var endTime = new DateTime(now.Year, now.Month, now.Day + this.Time.RepeatIn.Days, this.Time.RepeatIn.Hours, 0, 0);
#endif
            Log.DebugFormat(Properties.Resources.LOG_COUNTDOWN_SET_NEW_REPEAT_TIME, endTime);

            return new Time(startTime, endTime, this.Time.RepeatIn, this.ID, this.Time.Repeat);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool Start()
        {
            return this.Start(this.Time);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        /// True on success.
        /// </returns>
        public bool Start(Time time)
        {
            var errorMessage = string.Empty;

            if (time != null && time.IsTimeInFuture)
            {
                Log.Info(Properties.Resources.LOG_COUNTDOWN_START_COUNTDOWN);

                this.Time = time;
                this.Active = true;

                this.countdown = new Thread(new ThreadStart(this.Run));
                this.countdown.Start();
            }
            else
            {
                errorMessage = Properties.Resources.TIME_VALIDATION_ERROR_INVALID_TIME_FORMAT;
            }

            if (this.OnError != null && !errorMessage.Equals(string.Empty))
            {
                this.OnError(this, errorMessage);
                return false;
            }

            return true;
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
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(Properties.Resources.COUNTDOWN_TO_STRING, this.Time, this.Source);
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_IS_RUNNING);

            while (this.Active && this.Time.RemainTime.Time.Ticks > 0)
            {
                if (this.OnTick != null)
                {
                    this.OnTick(this);
                }
                
                this.Time.RemainTime.Time = this.Time.Endtime.Subtract(DateTime.Now);

                Thread.Sleep(1000);
            }

            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_ENDS);

            this.ResetValues();

            if (this.OnZero != null && !this.Cancel)
            {
                this.OnZero(this, this.Source);
            }
        }

        /// <summary>
        /// Resets the values.
        /// </summary>
        private void ResetValues()
        {
            Log.Debug(Properties.Resources.LOG_RESET_VALUES);

            this.Active = false;

            Log.DebugFormat(
                Properties.Resources.LOG_COUNTDOWN_RESET_VALUES,
                this.Active,
                this.Time);
        }
    }
}
