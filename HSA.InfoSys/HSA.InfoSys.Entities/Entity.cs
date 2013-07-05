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
        /// The logger of Entity.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("Entity");

        /// <summary>
        /// The type name of the entity.
        /// </summary>
        private string type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class
        /// and sets the type name depending on the initializing type.
        /// </summary>
        protected Entity()
        {
            this.type = this.GetType().Name;
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
        /// Gets or sets the type name.
        /// </summary>
        /// <value>
        /// The type name of this instance.
        /// </value>
        [DataMember]
        public virtual string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// Loads this instance from NHibernate.
        /// NHibernate supports lazy loading, so we need some
        /// functionality to load a reference to a foreign table too.
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
        /// Size of an entity is calculated by serializing the
        /// instance. The length of the stream is the size of
        /// this entity.
        /// </summary>
        /// <returns>The size of this instance.</returns>
        public virtual long SizeOf()
        {
            var m = new MemoryStream();
            var b = new BinaryFormatter();

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
