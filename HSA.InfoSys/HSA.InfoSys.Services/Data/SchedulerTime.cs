// ------------------------------------------------------------------------
// <copyright file="SchedulerTime.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents the scheduler which
    /// stores the values for repeating crawling
    /// for a component
    /// </summary>
    [DataContract]
    public class SchedulerTime : Entity
    {
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        public virtual DateTime Begin { get; set; }

        /// <summary>
        /// Gets or sets the Days.
        /// </summary>
        /// <value>
        /// The Days.
        /// </value>
        [DataMember]
        public virtual int Days { get; set; }

        /// <summary>
        /// Gets or sets the Hours.
        /// </summary>
        /// <value>
        /// The Hours.
        /// </value>
        [DataMember]
        public virtual int Hours { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "ID: {0}, Days: {1}, Hours: {2}, Begin: {3}",
                this.EntityId,
                this.Days,
                this.Hours,
                this.Begin);
        }
    }
}
