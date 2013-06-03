// ------------------------------------------------------------------------
// <copyright file="RemainTime.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Timing
{
    using System;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class represents the remaining time.
    /// </summary>
    public class RemainTime
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<Type>.GetLogger(typeof(RemainTime));

        /// <summary>
        /// Initializes a new instance of the <see cref="RemainTime"/> class.
        /// </summary>
        /// <param name="time">The remaining time.</param>
        public RemainTime(TimeSpan time)
        {
            Log.DebugFormat(Properties.Resources.LOG_REMAIN_TIME_INITIALIZE, time);
            this.Time = time;
        }

        /// <summary>
        /// Gets or sets the remaining time.
        /// </summary>
        /// <value>
        /// The remaining time.
        /// </value>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Gets the days.
        /// </summary>
        /// <value>
        /// The days.
        /// </value>
        public string Days
        {
            get
            {
                Log.DebugFormat(Properties.Resources.LOG_REMAIN_TIME_GET_DAYS, this.Time.Days);

                if (this.Time.Days < 10)
                {
                    return string.Format("0{0}", this.Time.Days.ToString());
                }
                else
                {
                    return this.Time.Days.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the hours.
        /// </summary>
        /// <value>
        /// The hours.
        /// </value>
        public string Hours
        {
            get
            {
                Log.DebugFormat(Properties.Resources.LOG_REMAIN_TIME_GET_HOURS, this.Time.Hours);

                if (this.Time.Hours < 10)
                {
                    return string.Format("0{0}", this.Time.Hours.ToString());
                }
                else
                {
                    return this.Time.Hours.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the minutes.
        /// </summary>
        /// <value>
        /// The minutes.
        /// </value>
        public string Minutes
        {
            get
            {
                Log.DebugFormat(Properties.Resources.LOG_REMAIN_TIME_GET_MINUTES, this.Time.Minutes);

                if (this.Time.Minutes < 10)
                {
                    return string.Format("0{0}", this.Time.Minutes.ToString());
                }
                else
                {
                    return this.Time.Minutes.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the seconds.
        /// </summary>
        /// <value>
        /// The seconds.
        /// </value>
        public string Seconds
        {
            get
            {
                Log.DebugFormat(Properties.Resources.LOG_REMAIN_TIME_GET_SECONDS, this.Time.Seconds);

                if (this.Time.Seconds < 10)
                {
                    return string.Format("0{0}", this.Time.Seconds.ToString());
                }
                else
                {
                    return this.Time.Seconds.ToString();
                }
            }
        }

        /// <summary>
        /// Gets the milliseconds.
        /// </summary>
        /// <value>
        /// The milliseconds.
        /// </value>
        public string Milliseconds
        {
            get
            {
                Log.DebugFormat(Properties.Resources.LOG_REMAIN_TIME_GET_MILLISECONDS, this.Time.Milliseconds);

                if (this.Time.Milliseconds < 10)
                {
                    return string.Format("0{0}", this.Time.Milliseconds.ToString());
                }
                else
                {
                    return this.Time.Milliseconds.ToString();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.LOG_REMAIN_TIME_TO_STRING,
                this.Days,
                this.Hours,
                this.Minutes,
                this.Seconds);
        }
    }
}
