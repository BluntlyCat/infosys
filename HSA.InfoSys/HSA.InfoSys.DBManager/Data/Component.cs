// ------------------------------------------------------------------------
// <copyright file="Component.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A component represents a system as a whole
    /// for example a web server or a database server.
    /// </summary>
    [KnownType(typeof(Component))]
    public class Component
    {
        /// <summary>
        /// Gets or sets the component GUID.
        /// </summary>
        /// <value>
        /// The component GUID.
        /// </value>
        [DataMember]
        public virtual Guid ComponentGUID { get; set; }

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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", this.ComponentGUID, this.Name, this.Category);
        }
    }
}
