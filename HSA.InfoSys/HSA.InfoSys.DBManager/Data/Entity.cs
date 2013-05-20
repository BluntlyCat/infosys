// ------------------------------------------------------------------------
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
    public class Entity
    {
        /// <summary>
        /// Gets or sets the entity GUID.
        /// </summary>
        /// <value>
        /// The entity GUID.
        /// </value>
        [DataMember]
        public virtual Guid EntityId { get; set; }
    }
}
