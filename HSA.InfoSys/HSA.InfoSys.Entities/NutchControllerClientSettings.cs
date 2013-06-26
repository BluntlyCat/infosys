// ------------------------------------------------------------------------
// <copyright file="NutchControllerClientSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// There we can store settings for nutch controller client
    /// </summary>
    [DataContract]
    [Serializable]
    public class NutchControllerClientSettings : Entity
    {
        /// <summary>
        /// Gets or sets the solr server.
        /// </summary>
        /// <value>
        /// The solr server.
        /// </value>
        [DataMember]
        public virtual string SolrServer { get; set; }

        /// <summary>
        /// Gets or sets the nutch clients.
        /// </summary>
        /// <value>
        /// The nutch clients.
        /// </value>
        [DataMember]
        public virtual string NutchClients { get; set; }

        /// <summary>
        /// Gets or sets the name of the seed file.
        /// </summary>
        /// <value>
        /// The name of the seed file.
        /// </value>
        [DataMember]
        public virtual string SeedFileName { get; set; }

        /// <summary>
        /// Gets or sets the base URL path.
        /// </summary>
        /// <value>
        /// The base URL path.
        /// </value>
        [DataMember]
        public virtual string BaseUrlPath { get; set; }

        /// <summary>
        /// Gets or sets the prefix path.
        /// </summary>
        /// <value>
        /// The prefix path.
        /// </value>
        [DataMember]
        public virtual string PrefixPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the prefix file.
        /// </summary>
        /// <value>
        /// The name of the prefix file.
        /// </value>
        [DataMember]
        public virtual string PrefixFileName { get; set; }

        /// <summary>
        /// Gets or sets the prefix format.
        /// </summary>
        /// <value>
        /// The prefix format.
        /// </value>
        [DataMember]
        public virtual string PrefixFormat { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        [DataMember]
        public virtual string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the base crawl path.
        /// </summary>
        /// <value>
        /// The base crawl path.
        /// </value>
        [DataMember]
        public virtual string BaseCrawlPath { get; set; }

        /// <summary>
        /// Gets or sets the path format two.
        /// </summary>
        /// <value>
        /// The path format two.
        /// </value>
        [DataMember]
        public virtual string PathFormatTwo { get; set; }

        /// <summary>
        /// Gets or sets the path format three.
        /// </summary>
        /// <value>
        /// The path format three.
        /// </value>
        [DataMember]
        public virtual string PathFormatThree { get; set; }

        /// <summary>
        /// Gets or sets the path format four.
        /// </summary>
        /// <value>
        /// The path format four.
        /// </value>
        [DataMember]
        public virtual string PathFormatFour { get; set; }

        /// <summary>
        /// Gets or sets the nutch command.
        /// </summary>
        /// <value>
        /// The nutch command.
        /// </value>
        [DataMember]
        public virtual string NutchCommand { get; set; }

        /// <summary>
        /// Gets or sets the crawl request.
        /// </summary>
        /// <value>
        /// The crawl request.
        /// </value>
        [DataMember]
        public virtual string CrawlRequest { get; set; }

        /// <summary>
        /// Gets or sets the crawl depth.
        /// </summary>
        /// <value>
        /// The crawl depth.
        /// </value>
        [DataMember]
        public virtual int CrawlDepth { get; set; }

        /// <summary>
        /// Gets or sets the crawl top N.
        /// </summary>
        /// <value>
        /// The crawl top N.
        /// </value>
        [DataMember]
        public virtual int CrawlTopN { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.NUTCHCONTROLLERSETTINGS_TO_STRING,
                this.SolrServer,
                this.SeedFileName,
                this.BaseUrlPath,
                this.NutchCommand,
                this.CrawlRequest,
                this.SizeOf());
        }
    }
}
