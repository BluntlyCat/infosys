// ------------------------------------------------------------------------
// <copyright file="Time.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Timing
{
    using System;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class represents the time object.
    /// </summary>
    public class Time
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<Type>.GetLogger(typeof(Time));

        /// <summary>
        /// Initializes a new instance of the <see cref="Time" /> class.
        /// </summary>
        /// <param name="start">The start time.</param>
        /// <param name="end">The end time.</param>
        /// <param name="repeatIn">The repeat in.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public Time(
            DateTime start,
            DateTime end,
            TimeSpan repeatIn,
            Guid id,
            bool repeat)
        {
            this.StartTime = start;
            this.Endtime = end;
            this.RepeatIn = repeatIn;
            this.RemainTime = new RemainTime(end.Subtract(start), id);
            this.Repeat = repeat;
            this.ID = id;
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; private set; }

        /// <summary>
        /// Gets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        public DateTime Endtime { get; private set; }

        /// <summary>
        /// Gets or sets the remain time.
        /// </summary>
        /// <value>
        /// The remain time.
        /// </value>
        public RemainTime RemainTime { get; set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan RepeatIn { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Time"/> is repeat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if repeat; otherwise, <c>false</c>.
        /// </value>
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public Guid ID { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is time in future.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is time in future; otherwise, <c>false</c>.
        /// </value>
        public bool IsTimeInFuture
        {
            get
            {
                return DateTime.Now < this.Endtime;
            }
        }

        /// <summary>
        /// Resets the values.
        /// </summary>
        public void ResetValues()
        {
            this.StartTime = new DateTime();
            this.Endtime = new DateTime();
            this.RemainTime = null;
            this.Repeat = false;

            Log.DebugFormat(
                Properties.Resources.LOG_TIME_RESET_VALUES,
                this.StartTime,
                this.Endtime,
                this.RemainTime);
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
                Properties.Resources.TIME_TO_STRING,
                this.StartTime,
                this.Endtime,
                this.RemainTime);
        }
    }
}
