// ------------------------------------------------------------------------
// <copyright file="SolrSearchClientSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// There we can store settings for solr search client.
    /// </summary>
    [DataContract]
    [Serializable]
    public class SolrSearchClientSettings : Entity
    {
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [DataMember]
        public virtual string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [DataMember]
        public virtual int Port { get; set; }

        /// <summary>
        /// Gets or sets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        [DataMember]
        public virtual string Collection { get; set; }

        /// <summary>
        /// Gets or sets the query format.
        /// </summary>
        /// <value>
        /// The query format.
        /// </value>
        [DataMember]
        public virtual string QueryFormat { get; set; }

        /// <summary>
        /// Gets or sets the request format.
        /// </summary>
        /// <value>
        /// The request format.
        /// </value>
        [DataMember]
        public virtual string RequestFormat { get; set; }

        /// <summary>
        /// Gets or sets the filter query format.
        /// </summary>
        /// <value>
        /// The filter query format.
        /// </value>
        [DataMember]
        public virtual string FilterQueryFormat { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        [DataMember]
        public virtual string Filter { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.SOLRSEARCHCLIENTSETTINGS_TO_STRING,
                this.Host,
                this.Port,
                this.Collection,
                this.QueryFormat,
                this.RequestFormat,
                this.FilterQueryFormat,
                this.Filter,
                this.SizeOf());
        }
    }
}
