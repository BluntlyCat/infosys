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
        /// Initializes a new instance of the <see cref="SolrSearchClientSettings"/> class.
        /// </summary>
        public SolrSearchClientSettings()
        {
            this.Host = string.Empty;
            this.Port = 0;
            this.Collection = string.Empty;
            this.FilterQuery = string.Empty;
            this.RequestFormat = string.Empty;
            this.QueryFormat = string.Empty;
        }

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
        /// Gets or sets the filter query format.
        /// </summary>
        /// <value>
        /// The filter query format.
        /// </value>
        [DataMember]
        public virtual string FilterQuery { get; set; }

        #region No user settings
        /// <summary>
        /// Gets or sets the request format.
        /// </summary>
        /// <value>
        /// The request format.
        /// </value>
        [DataMember]
        public virtual string RequestFormat { get; set; }

        /// <summary>
        /// Gets or sets the query format.
        /// </summary>
        /// <value>
        /// The query format.
        /// </value>
        [DataMember]
        public virtual string QueryFormat { get; set; }
        #endregion

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
                this.FilterQuery,
                this.RequestFormat,
                this.QueryFormat,
                this.SizeOf());
        }
    }
}
