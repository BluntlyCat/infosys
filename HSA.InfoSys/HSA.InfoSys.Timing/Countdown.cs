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
        /// The timer thread
        /// </summary>
        private Thread countdown;

        /// <summary>
        /// Initializes a new instance of the <see cref="Countdown" /> class.
        /// </summary>
        public Countdown()
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_INITIALIZE);
        }

        /// <summary>
        /// The delegate for indicating that the time value has changed.
        /// </summary>
        public delegate void TickEventHandler();

        /// <summary>
        /// The delegate for indicating that the time value is zero.
        /// </summary>
        public delegate void ZeroEventHandler();

        /// <summary>
        /// Occurs when [tick].
        /// </summary>
        public event TickEventHandler OnTick;

        /// <summary>
        /// Occurs when [zero].
        /// </summary>
        public event ZeroEventHandler OnZero;

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
        /// Sets the time to repeat.
        /// </summary>
        /// <returns>A new time instance for next countdown.</returns>
        public Time SetTimeToRepeat()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_SET_REPEAT_TIME);

            DateTime endtime = new DateTime();

            switch (this.Time.TypeOfTime)
            {
#if DEBUG
                case TypeOfTime.Timespan:
                    endtime = DateTime.Now.Add(new TimeSpan(0, 0, int.Parse(this.Time.TimeValues[0].ToString())));
                    break;
#endif
                case TypeOfTime.Time:
                    endtime = new DateTime(
                        this.Time.Endtime.Year,
                        this.Time.Endtime.Month,
                        this.Time.Endtime.Day + 1,
                        this.Time.Endtime.Hour,
                        this.Time.Endtime.Minute,
                        this.Time.Endtime.Second);
                    break;

                case TypeOfTime.Date:
                    endtime = new DateTime(
                        this.Time.Endtime.Year,
                        this.Time.Endtime.Month + 1,
                        this.Time.Endtime.Day,
                        this.Time.Endtime.Hour,
                        this.Time.Endtime.Minute,
                        this.Time.Endtime.Second);
                    break;
            }

            Log.DebugFormat(Properties.Resources.LOG_COUNTDOWN_SET_NEW_REPEAT_TIME, endtime);

            var remain = new RemainTime(endtime.Subtract(DateTime.Now));

            return new Time(
                DateTime.Now,
                endtime,
                remain,
                this.Time.TypeOfTime,
                this.Time.TimeValues,
                this.Time.TimeString,
                this.Time.Repeat);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="errorMessage">The error message.</param>
        public void Start(Time time, out string errorMessage)
        {
            if (time != null && TimeValidation.IsTimeInFuture(time, out errorMessage))
            {
                Log.Info(Properties.Resources.LOG_COUNTDOWN_START_COUNTDOWN);

                this.Time = time;
                this.Active = true;

                this.countdown = new Thread(new ThreadStart(this.Run));
                this.countdown.Start();
            }

            errorMessage = Properties.Resources.TIME_VALIDATION_ERROR_INVALID_TIME_FORMAT;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            Log.Info(Properties.Resources.LOG_COUNTDOWN_STOP_COUNTDOWN);
            this.Active = false;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        private void Run()
        {
            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_IS_RUNNING);

            while (this.Active && this.Time.RemainTime.Time.Ticks > 0)
            {
                this.OnTick();
                this.Time.RemainTime.Time = this.Time.Endtime.Subtract(DateTime.Now);
                Thread.Sleep(500);
            }

            Log.Debug(Properties.Resources.LOG_COUNTDOWN_THREAD_ENDS);

            this.ResetValues();
            this.OnZero();
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
