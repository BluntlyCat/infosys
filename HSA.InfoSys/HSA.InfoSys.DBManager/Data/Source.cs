// ------------------------------------------------------------------------
// <copyright file="Source.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents the source of an issue.
    /// </summary>
    [DataContract]
    public class Source : Entity
    {
        /// <summary>
        /// Gets or sets the source URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember]
        public virtual string URL { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", this.EntityId, this.URL);
        }
    }
}
