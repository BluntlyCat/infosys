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
    public class SolrSearchClientSettings : Settings
    {
        /// <summary>
        /// Gets or sets the solr host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [DataMember]
        public virtual string Host { get; set; }

        /// <summary>
        /// Gets or sets the port on whitch solr is listening.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [DataMember]
        public virtual int Port { get; set; }

        /// <summary>
        /// Gets or sets the collection.
        /// The collection is the database container
        /// of solr where we store our results.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        [DataMember]
        public virtual string Collection { get; set; }

        /// <summary>
        /// Gets or sets the filter query format.
        /// The filter query is a more or less complex
        /// string of key words for searching in our solr
        /// database to get the results for a component.
        /// </summary>
        /// <value>
        /// The filter query format.
        /// </value>
        [DataMember]
        public virtual string FilterQuery { get; set; }

        #region No user settings
        /// <summary>
        /// Gets or sets the request format.
        /// Is a pattern to build the search request to solr.
        /// </summary>
        /// <value>
        /// The request format.
        /// </value>
        [DataMember]
        public virtual string RequestFormat { get; set; }

        /// <summary>
        /// Gets or sets the query format.
        /// Is a pattern for buidlding the search query.
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

        /// <summary>
        /// Sets the default values.
        /// </summary>
        public virtual void SetDefaults()
        {
            var newSettings = this.GetDefaults() as SolrSearchClientSettings;

            if (newSettings != null)
            {
                //// Public settings
                this.Host = newSettings.Host;
                this.Port = newSettings.Port;
                this.Collection = newSettings.Collection;
                this.FilterQuery = newSettings.FilterQuery;

                //// Non public settings
                this.RequestFormat = newSettings.RequestFormat;
                this.QueryFormat = newSettings.QueryFormat;
            }
        }

        /// <summary>
        /// Gets the settings with default values.
        /// </summary>
        /// <returns>
        /// A new settings object with its default values.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual Settings GetDefaults()
        {
            var newSettings = new SolrSearchClientSettings
                {
                    //// Public settings
                    Host = string.Empty,
                    Port = 0,
                    Collection = string.Empty,
                    FilterQuery = string.Empty,

                    //// Non public settings
                    RequestFormat = "GET {0} HTTP/1.1{1}Host: {2}{3}Content-Length: 0{4}",
                    QueryFormat = "/solr/{0}/select?q={1}&wt={2}&indent=true"
                };

            return newSettings;
        }

        /// <summary>
        /// Determines whether this settings has default values.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this settings has default values; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual bool IsDefault()
        {
            return this.Equals(this.GetDefaults());
        }
    }
}
