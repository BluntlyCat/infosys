// ------------------------------------------------------------------------
// <copyright file="Entity.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// This is the base class for all data base objects
    /// It holds the unique id for each entity.
    /// </summary>
    [DataContract]
    [KnownType(typeof(Result))]
    [KnownType(typeof(OrgUnit))]
    [KnownType(typeof(Settings))]
    [KnownType(typeof(Component))]
    [KnownType(typeof(WCFSettings))]
    [KnownType(typeof(OrgUnitConfig))]
    [KnownType(typeof(EmailNotifierSettings))]
    [KnownType(typeof(SolrSearchClientSettings))]
    [KnownType(typeof(NutchControllerClientSettings))]
    [Serializable]
    public abstract class Entity
    {
        /// <summary>
        /// The logger of result.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Entity");

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        protected Entity()
        {
            this.Type = this.GetType().Name;
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
        [DataMember]
        public virtual string Type { get; set; }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// </summary>
        /// <param name="types">The types you want load eager.</param>
        /// <returns>
        /// The entity behind the NHibernate proxy.
        /// </returns>
        public virtual Entity Unproxy(string[] types = null)
        {
            return this;
        }

        /// <summary>
        /// Size of result.
        /// </summary>
        /// <returns>The size of this instance.</returns>
        public virtual long SizeOf()
        {
            MemoryStream m = new MemoryStream();
            BinaryFormatter b = new BinaryFormatter();

            long size = -1;

            try
            {
                b.Serialize(m, this);
                size = m.Length;

                Log.DebugFormat(Properties.Resources.ENTITY_SIZE_OF, size);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.ENTITY_SIZEOF_ERROR, e);
            }
            finally
            {
                m.Close();
            }

            return size;
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
                Properties.Resources.ENTITY_TO_STRING,
                this.EntityId,
                this.SizeOf());
        }
    }
}
