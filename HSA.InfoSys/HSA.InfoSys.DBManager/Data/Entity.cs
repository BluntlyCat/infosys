namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    [KnownType(typeof(Component))]
    [KnownType(typeof(Issue))]
    [KnownType(typeof(Source))]
    [KnownType(typeof(Result))]
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
