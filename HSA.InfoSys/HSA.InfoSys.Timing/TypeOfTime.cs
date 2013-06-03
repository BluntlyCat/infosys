// ------------------------------------------------------------------------
// <copyright file="TypeOfTime.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Timing
{
    /// <summary>
    /// This enumeration determines what kind
    /// of time the user typed in.
    /// </summary>
    public enum TypeOfTime
    {
        /// <summary>
        /// The timespan
        /// </summary>
        Timespan = 3,

        /// <summary>
        /// The time
        /// </summary>
        Time = 5,

        /// <summary>
        /// The date
        /// </summary>
        Date = 10,

        /// <summary>
        /// Invalid time value
        /// </summary>
        Invalid
    }
}
