// ------------------------------------------------------------------------
// <copyright file="TimeValidation.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Timing
{
    using System;
    using System.Linq;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This class contains methods to
    /// validate if a given string is a
    /// time or date formatted string.
    /// </summary>
    public class TimeValidation
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<Type>.GetLogger(typeof(TimeValidation));

        /// <summary>
        /// Prevents a default instance of the <see cref="TimeValidation"/> class from being created.
        /// </summary>
        private TimeValidation()
        {
        }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        private string Error { get; set; }

        /// <summary>
        /// Time factory validates the given time string and creates
        /// a new time instance on success.
        /// </summary>
        /// <param name="timeString">The time string.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        /// A new instance of time on success.
        /// </returns>
        public static Time TimeFactory(string timeString, bool repeat, out string errorMessage)
        {
            Time time = null;

            var validation = new TimeValidation();
            var valid = IsValidTimeString(timeString, out errorMessage);
            var inFuture = false;

            if (!valid)
            {
                errorMessage = Properties.Resources.TIME_VALIDATION_ERROR_TIME_NOT_VALID;
            }

            time = validation.InitializeTimeString(timeString, repeat, out errorMessage);
            inFuture = IsTimeInFuture(time, out errorMessage);

            if (!inFuture)
            {
                errorMessage = Properties.Resources.TIME_VALIDATION_ERROR_TIME_IN_PAST;
            }

            return time;
        }

        /// <summary>
        /// Determines whether [is time string valid].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if [is time string valid]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidTimeString(string time, out string errorMessage)
        {
            Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_BEGIN_VALIDATION, time);

            var validation = new TimeValidation();
            errorMessage = string.Empty;

            var valid = validation.IsNullOrEmpty(time)
                && validation.HasValidCharacters(time)
                && validation.HasValidLength(time)
                && validation.HasSeparators(time)
                && validation.CheckIndexForSeparators(time, 2, 5)
                && validation.HasNoWrongPlacedSeparators(time, 2, 5);

            if (valid)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_IS_VALID, time);
            }
            else
            {
                errorMessage = validation.Error;
                Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_IS_NOT_VALID, time, validation.Error);
            }

            return valid;
        }

        /// <summary>
        /// Determines whether [is time in future] [the specified time string].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>
        ///   <c>true</c> if [is time in future] [the specified time string]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTimeInFuture(Time time, out string errorMessage)
        {
            errorMessage = string.Empty;

            Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_SET_IF_IN_FUTURE, time);

            if (time == null)
            {
                return false;
            }
            else if (DateTime.Now < time.Endtime)
            {
                return true;
            }
            else
            {
                errorMessage = Properties.Resources.TIME_VALIDATION_ERROR_TIME_IN_PAST;
                Log.Error(Properties.Resources.LOG_TIME_VALIDATION_TIME_IN_PAST);

                return false;
            }
        }

        /// <summary>
        /// Gets the kind of time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        /// The type of time.
        /// </returns>
        public static TypeOfTime GetTypeOfTime(string time)
        {
            if (0 < time.Length && time.Length <= (int)TypeOfTime.Timespan)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_STRING_IS_TYPE_OF, TypeOfTime.Timespan);
                return TypeOfTime.Timespan;
            }
            else if (time.Length == (int)TypeOfTime.Time)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_STRING_IS_TYPE_OF, TypeOfTime.Time);
                return TypeOfTime.Time;
            }
            else if (time.Length == (int)TypeOfTime.Date)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_STRING_IS_TYPE_OF, TypeOfTime.Date);
                return TypeOfTime.Date;
            }
            else
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_STRING_IS_TYPE_OF, TypeOfTime.Invalid);
                return TypeOfTime.Invalid;
            }
        }

        /// <summary>
        /// Initializes the time string.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of time by the given time string.</returns>
        public Time InitializeTimeString(string time, bool repeat, out string errorMessage)
        {
            try
            {
                return this.Initialize(time, repeat, out errorMessage);
            }
            catch (Exception e)
            {
                errorMessage = Properties.Resources.TIME_VALIDATION_ERROR_INVALID_TIME_FORMAT;

                Log.ErrorFormat(
                    Properties.Resources.LOG_TIME_VALIDATION_INVALID_TIME_FORMAT,
                    this,
                    e);

                return null;
            }
        }

        /// <summary>
        /// Determines whether [is null or empty] [the specified time string].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified time string]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNullOrEmpty(string time)
        {
            var empty = string.IsNullOrWhiteSpace(time);

            if (empty)
            {
                this.Error = Properties.Resources.TIME_VALIDATION_EMPTY_STRING;
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_EMPTY_STRING, time);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether [has valid characters] [the specified time string].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        ///   <c>true</c> if [has valid characters] [the specified time string]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasValidCharacters(string time)
        {
            var valid = time.All(c =>
                ('0' <= c && c <= '9')
                || c == '.'
                || c == ':');

            if (valid)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_VALID_CHARS, time);
            }
            else
            {
                this.Error = Properties.Resources.TIME_VALIDATION_ERROR_INVALID_CHARS;
                Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_INVALID_CHARS, time);
            }

            return valid;
        }

        /// <summary>
        /// Determines whether [has correct length] [the specified time string].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        ///   <c>true</c> if [has correct length] [the specified time string]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasValidLength(string time)
        {
            var valid = (1 <= time.Length && time.Length <= 3)
                || time.Length == 5
                || time.Length == 10;

            if (valid)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_VALID_LENGTH, time, time.Length);
            }
            else
            {
                this.Error = Properties.Resources.TIME_VALIDATION_ERROR_INVALID_LENGTH;
                Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_INVALID_LENGTH, time, time.Length);
            }

            return valid;
        }

        /// <summary>
        /// Determines whether the specified time string has separators.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>
        ///   <c>true</c> if the specified time string has separators; otherwise, <c>false</c>.
        /// </returns>
        private bool HasSeparators(string time)
        {
            bool valid = true;

            if (time.Length > 3)
            {
                valid = time.Contains('.')
                    || time.Contains(':');

                if (valid)
                {
                    Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_HAS_SEPARATORS, time);
                }
                else
                {
                    this.Error = Properties.Resources.TIME_VALIDATION_ERROR_HAS_NO_SEPARATORS;
                    Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_NO_SEPARATORS, time);
                }
            }
            else
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_NO_SEPARATORS_NEEDED, time);
            }

            return valid;
        }

        /// <summary>
        /// Checks the time string at the  given index for separators.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="indices">The indices where must be a separator.</param>
        /// <returns>
        ///   <c>true</c> if [separators are placed correct]; otherwise, <c>false</c>.
        /// </returns>
        private bool CheckIndexForSeparators(string time, params int[] indices)
        {
            if (time.Length > 3)
            {
                for (int i = 0; i < time.Length; i++)
                {
                    if (indices.Contains(i))
                    {
                        var valid = this.IsSeparatorHere(time, i);

                        if (valid)
                        {
                            Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_HAS_SEPARATOR_AT_INDEX, time, i);
                        }
                        else
                        {
                            this.Error = string.Format(Properties.Resources.TIME_VALIDATION_ERROR_NO_SEPARATOR_AT_INDEX, i);
                            Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_NO_SEPARATOR_AT_INDEX, time, i);
                        }

                        return valid;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether [has wrong placed separators] [the specified time string].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>
        ///   <c>true</c> if [has wrong placed separators] [the specified time string]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasNoWrongPlacedSeparators(string time, params int[] indices)
        {
            if (time.Length > 3)
            {
                for (int i = 0; i < time.Length; i++)
                {
                    if (indices.All(indice => indice != i))
                    {
                        if (this.IsSeparatorHere(time, i))
                        {
                            this.Error = string.Format(Properties.Resources.TIME_VALIDATION_ERROR_WRONG_PLACED_SEPARATOR_AT_INDEX, i);
                            Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_WRONG_PLACED_SEPARATOR_AT_INDEX, time, i);
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (time.Contains('.') || time.Contains(':'))
                {
                    this.Error = Properties.Resources.TIME_VALIDATION_ERROR_WRONG_PLACED_SEPARATOR;
                    Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_WRONG_PLACED_SEPARATOR, time);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is separator here] [the specified index].
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if [is separator here] [the specified index]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSeparatorHere(string time, int index)
        {
            var valid = time[index].Equals('.')
                || time[index].Equals(':');

            if (valid)
            {
                Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_HAS_SEPARATOR_AT_INDEX, time, index);
            }
            else
            {
                this.Error = string.Format(Properties.Resources.TIME_VALIDATION_ERROR_NO_SEPARATOR_AT_INDEX, index);
                Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_NO_SEPARATOR_AT_INDEX, time, index);
            }

            return valid;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="repeat">if set to <c>true</c> [repeat].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>A new instance of time by the given time string.</returns>
        private Time Initialize(string time, bool repeat, out string errorMessage)
        {
            var now = DateTime.Now;
            var timeValues = time.Split('.', ':');

            var endtime = new DateTime();
            var type = GetTypeOfTime(time);

            int year;
            int month;
            int day;
            int hour;
            int minute;

            errorMessage = string.Empty;

            switch (type)
            {
                case TypeOfTime.Timespan:
#if !DEBUG
                    endtime = DateTime.Now.Add(new TimeSpan(0, int.Parse(timeValues[0].ToString()), 0));
#else
                    endtime = DateTime.Now.Add(new TimeSpan(0, 0, int.Parse(timeValues[0].ToString())));
#endif
                    break;

                case TypeOfTime.Time:
                    hour = int.Parse(timeValues[0]);
                    minute = int.Parse(timeValues[1]);

                    endtime = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

                    Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_GOT_VALUES_OF_TYPE_TIME, hour, minute);

                    break;

                case TypeOfTime.Date:
                    year = int.Parse(timeValues[2]);
                    month = int.Parse(timeValues[1]);
                    day = int.Parse(timeValues[0]);

                    endtime = new DateTime(year, month, day);

                    Log.DebugFormat(Properties.Resources.LOG_TIME_VALIDATION_GOT_VALUES_OF_TYPE_DATE, year, month, day);

                    break;

                case TypeOfTime.Invalid:
                default:
                    errorMessage = string.Format(Properties.Resources.TIME_VALIDATION_ERROR_INVALID_TYPE_OF_TIME, type);
                    Log.ErrorFormat(Properties.Resources.LOG_TIME_VALIDATION_INVALID_TYPE_OF_TIME, type);

                    break;
            }

            var remainTime = new RemainTime(endtime.Subtract(now));
            var repeatIn = remainTime.Time
                ;
            return new Time(now, endtime, repeatIn, remainTime, type, timeValues, time, repeat);
        }
    }
}
