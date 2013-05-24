// ------------------------------------------------------------------------
// <copyright file="Result.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents the search result to an issue.
    /// </summary>
    [DataContract]
    public class Result : Entity
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [DataMember]
        public virtual string Data { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source URL.
        /// </value>
        [DataMember]
        public virtual string Source { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        public virtual DateTime Time { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "ID: {0}, Data: {1}, Source: {2}, Time: {3}",
                this.EntityId,
                this.Data,
                this.Source,
                this.Time);
        }
    }
}
