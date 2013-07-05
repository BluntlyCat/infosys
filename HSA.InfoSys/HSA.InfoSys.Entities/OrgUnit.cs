// ------------------------------------------------------------------------
// <copyright file="OrgUnit.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This represents the system information of a Component
    /// </summary>
    [DataContract]
    [Serializable]
    public class OrgUnit : Entity
    {
        /// <summary>
        /// Gets or sets the UserId of the user who owns this unit.
        /// </summary>
        /// <value>
        /// The UserId.
        /// </value>
        [DataMember]
        public virtual int UserId { get; set; }

        /// <summary>
        /// Gets or sets the unit name.
        /// </summary>
        /// <value>
        /// The name of this OrgUnit.
        /// </value>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The OrgUnit configuration.
        /// </value>
        [DataMember]
        public virtual OrgUnitConfig OrgUnitConfig { get; set; }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// NHibernate supports lazy loading, so we need some
        /// functionality to load a reference to a foreign table too.
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
                    if (type.Equals(typeof(OrgUnitConfig).Name) && this.OrgUnitConfig != null)
                    {
                        this.OrgUnitConfig = this.OrgUnitConfig.Unproxy(types) as OrgUnitConfig;
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
                Properties.Resources.ORGUNIT_TO_STRING,
                this.EntityId,
                this.UserId,
                this.Name,
                this.OrgUnitConfig,
                this.SizeOf());
        }
    }
}
