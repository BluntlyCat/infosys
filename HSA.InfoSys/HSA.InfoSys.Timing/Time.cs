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
        /// <param name="time">The time.</param>
        /// <param name="days">The days.</param>
        /// <param name="id">The id.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public Time(
            int time,
            int days,
            Guid id,
            bool repeat)
        {
            var now = DateTime.Now;
            this.RepeatIn = new DateTime(now.Year, now.Month, now.Day, time, 0, 0).AddDays(days);
            this.Repeat = repeat;
            this.ID = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="days">The days.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public Time(
            int time,
            int days,
            bool repeat)
        {
            var now = DateTime.Now;
            this.RepeatIn = new DateTime(now.Year, now.Month, now.Day, time, 0, 0).AddDays(days);
            this.Repeat = repeat;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Time"/> class.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="days">The days.</param>
        /// <param name="repeatIn">The repeat in.</param>
        /// <param name="id">The id.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        public Time(
            int time,
            int days,
            DateTime repeatIn,
            Guid id,
            bool repeat)
        {
            this.RepeatIn = repeatIn;
            this.Repeat = repeat;
            this.ID = id;
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public DateTime RepeatIn { get; private set; }

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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.TIME_TO_STRING,
                this.ID,
                this.Repeat,
                this.RepeatIn.Subtract(DateTime.Now).TotalHours);
        }
    }
}
