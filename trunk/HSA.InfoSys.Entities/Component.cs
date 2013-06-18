// ------------------------------------------------------------------------
// <copyright file="Component.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Collections.Generic;
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
        /// Gets or sets the org unit.
        /// </summary>
        /// <value>
        /// The org unit.
        /// </value>
        [DataMember]
        public virtual OrgUnit OrgUnit { get; set; }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// </summary>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity behind the NHibernate proxy.
        /// </returns>
        public override Entity Unproxy(string[] types = null)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    if (type.Equals(typeof(OrgUnit).Name) && this.OrgUnit != null)
                    {
                        this.OrgUnit = this.OrgUnit.Unproxy(types) as OrgUnit;
                    }
                }
            }

            return base.Unproxy(types);
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
                "ID: {0}, Name: {1}, OrgUnit: ({2})",
                this.EntityId,
                this.Name,
                this.OrgUnit);
        }
    }
}
