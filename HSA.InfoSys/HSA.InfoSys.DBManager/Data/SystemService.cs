// ------------------------------------------------------------------------
// <copyright file="SystemService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This represents the system information of a Component
    /// </summary>
    [DataContract]
    public class SystemService : Entity
    {
        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        /// <value>
        /// The UserId.
        /// </value>
        [DataMember]
        public virtual int UserId { get; set; }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        /// <value>
        /// The service name.
        /// </value>
        [DataMember]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        public virtual DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        [DataMember]
        public virtual Component Component { get; set; }

        /// <summary>
        /// Gets or sets the system config.
        /// </summary>
        /// <value>
        /// The system config.
        /// </value>
        [DataMember]
        public virtual SystemConfig SystemConfig { get; set; }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// </summary>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity behind the NHibernate proxy.
        /// </returns>
        public override Entity Unproxy(Type[] types = null)
        {
            if (types != null)
            {
                foreach (var type in types)
                {
                    if (type == typeof(Component))
                    {
                        this.Component = this.Component.Unproxy(types) as Component;
                    }
                    else if (type == typeof(SystemConfig))
                    {
                        this.SystemConfig = this.SystemConfig.Unproxy(types) as SystemConfig;
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
                "{0}, {1}, {2}, {3}, {4}",
                this.EntityId,
                this.UserId,
                this.TimeStamp,
                this.Component,
                this.SystemConfig);
        }
    }
}
