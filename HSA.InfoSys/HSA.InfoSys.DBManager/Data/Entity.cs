﻿// ------------------------------------------------------------------------
// <copyright file="Entity.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This is the base class for all data base objects
    /// It holds the unique id for each entity.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Component))]
    [KnownType(typeof(Issue))]
    [KnownType(typeof(Source))]
    [KnownType(typeof(Result))]
    [KnownType(typeof(SystemService))]
    [KnownType(typeof(SystemConfig))]
    [KnownType(typeof(Scheduler))]
    public abstract class Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        protected Entity()
        {
            var type = this.GetType().ToString().Split('.');
            this.Type = type[type.Length - 1];
        }

        /// <summary>
        /// Gets or sets the entity GUID.
        /// </summary>
        /// <value>
        /// The entity GUID.
        /// </value>
        [DataMember]
        public virtual Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type of this instance.
        /// </value>
        public virtual string Type { get; protected set; }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// </summary>
        /// <returns>The entity behind the NHibernate proxy.</returns>
        public virtual Entity Unproxy()
        {
            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}", this.EntityId);
        }
    }
}
