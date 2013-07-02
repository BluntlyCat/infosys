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
    public class NutchControllerClientSettings : Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NutchControllerClientSettings"/> class.
        /// </summary>
        public NutchControllerClientSettings()
        {
            //// Public settings
            this.HomePath = string.Empty;
            this.NutchPath = string.Empty;
            this.NutchCommand = string.Empty;
            this.NutchClients = string.Empty;
            this.DefaultURLs = "http://nvd.nist.gov/,http://www.heise.de/security/";
            this.CrawlDepth = 0;
            this.CrawlTopN = 0;
            this.SolrServer = string.Empty;
            this.JavaHome = string.Empty;
            this.CertificatePath = string.Empty;
            this.Prefix = string.Empty;

            //// Non public settings
            this.CrawlRequest = "crawl {0} -solr {1} -depth {2} -topN {3}";
            this.SeedFileName = "seed.txt";
            this.PrefixFileName = "conf/regex-urlfilter.txt";
            this.BaseUrlPath = ".nutch/urls";
            this.BaseCrawlPath = "crawler";
            this.PathFormatTwo = "{0}/{1}";
            this.PathFormatThree = "{0}/{1}/{2}";
            this.PathFormatFour = "{0}/{1}/{2}/{3}";
            this.PrefixFormat = "{0}/{1}";
        }

        /// <summary>
        /// Gets or sets the home path.
        /// </summary>
        /// <value>
        /// The home path.
        /// </value>
        [DataMember]
        public virtual string HomePath { get; set; }

        /// <summary>
        /// Gets or sets the nutch path.
        /// </summary>
        /// <value>
        /// The nutch path.
        /// </value>
        [DataMember]
        public virtual string NutchPath { get; set; }

        /// <summary>
        /// Gets or sets the nutch command.
        /// </summary>
        /// <value>
        /// The nutch command.
        /// </value>
        [DataMember]
        public virtual string NutchCommand { get; set; }

        /// <summary>
        /// Gets or sets the nutch clients.
        /// </summary>
        /// <value>
        /// The nutch clients.
        /// </value>
        [DataMember]
        public virtual string NutchClients { get; set; }

        /// <summary>
        /// Gets or sets the default URLs.
        /// </summary>
        /// <value>
        /// The default URLs.
        /// </value>
        [DataMember]
        public virtual string DefaultURLs { get; set; }

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
        /// Gets or sets the solr server.
        /// </summary>
        /// <value>
        /// The solr server.
        /// </value>
        [DataMember]
        public virtual string SolrServer { get; set; }

        /// <summary>
        /// Gets or sets the java home.
        /// </summary>
        /// <value>
        /// The java home.
        /// </value>
        [DataMember]
        public virtual string JavaHome { get; set; }

        /// <summary>
        /// Gets or sets the certificate path.
        /// </summary>
        /// <value>
        /// The certificate path.
        /// </value>
        [DataMember]
        public virtual string CertificatePath { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        [DataMember]
        public virtual string Prefix { get; set; }

        #region No user settings
        /// <summary>
        /// Gets or sets the crawl request.
        /// </summary>
        /// <value>
        /// The crawl request.
        /// </value>
        [DataMember]
        public virtual string CrawlRequest { get; set; }

        /// <summary>
        /// Gets or sets the name of the seed file.
        /// </summary>
        /// <value>
        /// The name of the seed file.
        /// </value>
        [DataMember]
        public virtual string SeedFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the prefix file.
        /// </summary>
        /// <value>
        /// The name of the prefix file.
        /// </value>
        [DataMember]
        public virtual string PrefixFileName { get; set; }

        /// <summary>
        /// Gets or sets the base URL path.
        /// </summary>
        /// <value>
        /// The base URL path.
        /// </value>
        [DataMember]
        public virtual string BaseUrlPath { get; set; }

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
        /// Gets or sets the prefix format.
        /// </summary>
        /// <value>
        /// The prefix format.
        /// </value>
        [DataMember]
        public virtual string PrefixFormat { get; set; }
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
                Properties.Resources.NUTCHCONTROLLERSETTINGS_TO_STRING,
                this.HomePath,
                this.NutchPath,
                this.NutchCommand,
                this.NutchClients,
                this.CrawlDepth,
                this.CrawlTopN,
                this.SolrServer,
                this.JavaHome,
                this.CertificatePath,
                this.Prefix,
                this.CrawlRequest,
                this.SeedFileName,
                this.PrefixFileName,
                this.BaseUrlPath,
                this.BaseCrawlPath,
                this.PathFormatTwo,
                this.PathFormatThree,
                this.PathFormatFour,
                this.PrefixFormat,
                this.SizeOf());
        }
    }
}
