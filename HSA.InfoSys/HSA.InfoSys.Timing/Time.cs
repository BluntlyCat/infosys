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
        /// <param name="remain">The remain time.</param>
        /// <param name="type">The type of time.</param>
        /// <param name="values">The time values.</param>
        /// <param name="time">The time string.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public Time(
            DateTime start,
            DateTime end,
            RemainTime remain,
            TypeOfTime type,
            string[] values,
            string time,
            bool repeat)
        {
            this.StartTime = start;
            this.Endtime = end;
            this.RemainTime = remain;
            this.TypeOfTime = type;
            this.TimeValues = values;
            this.TimeString = time;
            this.Repeat = repeat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time"/> class.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="type">The type.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public Time(TimeSpan timeSpan, TypeOfTime type, bool repeat)
        {
            this.StartTime = DateTime.Now;
            this.Endtime = DateTime.Now.Add(timeSpan);
            this.RemainTime = new RemainTime(this.Endtime.Subtract(this.StartTime));
            this.TypeOfTime = type;
            this.Repeat = repeat;
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
        /// Gets or sets the type of time.
        /// </summary>
        /// <value>
        /// The type of time.
        /// </value>
        public TypeOfTime TypeOfTime { get; set; }

        /// <summary>
        /// Gets the time values.
        /// </summary>
        /// <value>
        /// The time values.
        /// </value>
        public string[] TimeValues { get; private set; }

        /// <summary>
        /// Gets or sets the time string.
        /// </summary>
        /// <value>
        /// The time string.
        /// </value>
        public string TimeString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Time"/> is repeat.
        /// </summary>
        /// <value>
        ///   <c>true</c> if repeat; otherwise, <c>false</c>.
        /// </value>
        public bool Repeat { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Time"/> is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public bool Valid { get; private set; }

        /// <summary>
        /// Resets the values.
        /// </summary>
        public void ResetValues()
        {
            this.StartTime = new DateTime();
            this.Endtime = new DateTime();
            this.RemainTime = null;
            this.TypeOfTime = TypeOfTime.Invalid;
            this.TimeValues = null;
            this.TimeString = string.Empty;
            this.Repeat = false;
            this.Valid = false;

            Log.DebugFormat(
                Properties.Resources.LOG_TIME_RESET_VALUES,
                this.StartTime,
                this.Endtime,
                this.RemainTime,
                this.TypeOfTime,
                this.TimeValues,
                this.TimeString);
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
                this.RemainTime,
                this.TypeOfTime,
                this.TimeValues,
                this.TimeString);
        }
    }
}
