// ------------------------------------------------------------------------
// <copyright file="Component.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component represents a system as a whole
    /// for example a web server or a database server.
    /// </summary>
    [DataContract]
    public class Component : Entity
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [DataMember]
        public virtual string Category { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [DataMember]
        public virtual Result Result { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "{0}, {1}, {2}, {3}",
                this.EntityId,
                this.Name,
                this.Category,
                this.Result);
        }
    }
}
