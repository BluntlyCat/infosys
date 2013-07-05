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
        /// Gets or sets the home path of the user we want
        /// use to establish a connection over SSH.
        /// </summary>
        /// <value>
        /// The home path.
        /// </value>
        [DataMember]
        public virtual string HomePath { get; set; }

        /// <summary>
        /// Gets or sets the path where nutch is installed on
        /// the clients. Must be the same on all clients we want
        /// use for crawling.
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
        /// Gets or sets the clients addresses on which
        /// we want start nutch for crawling.
        /// </summary>
        /// <value>
        /// The nutch clients.
        /// </value>
        [DataMember]
        public virtual string NutchClients { get; set; }

        /// <summary>
        /// Gets or sets the default URLs.
        /// There will be a couple of urls we want crawl by default
        /// without defining for evry OrgUnit each time. This urls
        /// will be set by default if an OrgUnit is created.
        /// </summary>
        /// <value>
        /// The default URLs.
        /// </value>
        [DataMember]
        public virtual string DefaultURLs { get; set; }

        /// <summary>
        /// Gets or sets the crawl depth.
        /// This value defines how deep we want dive into
        /// the jungle of links on a webpage recursively.
        /// </summary>
        /// <value>
        /// The crawl depth.
        /// </value>
        [DataMember]
        public virtual int CrawlDepth { get; set; }

        /// <summary>
        /// Gets or sets the crawl top N.
        /// This value means the amount of links on a webpage
        /// we take care about on each level for crawling. 
        /// </summary>
        /// <value>
        /// The crawl top N.
        /// </value>
        [DataMember]
        public virtual int CrawlTopN { get; set; }

        /// <summary>
        /// Gets or sets the solr server.
        /// There we want send our results of the latest crawl.
        /// </summary>
        /// <value>
        /// The solr server.
        /// </value>
        [DataMember]
        public virtual string SolrServer { get; set; }

        /// <summary>
        /// Gets or sets the java home.
        /// Simply the directory where java is installed.
        /// </summary>
        /// <value>
        /// The java home.
        /// </value>
        [DataMember]
        public virtual string JavaHome { get; set; }

        /// <summary>
        /// Gets or sets the certificate path.
        /// The place where the certificate for client
        /// authentification on our remote hosts is stored.
        /// </summary>
        /// <value>
        /// The certificate path.
        /// </value>
        [DataMember]
        public virtual string CertificatePath { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// The prefix defines a regex pattern which will be
        /// used by nutch if it crawls on a webpage. Nutch
        /// should only crawl urls which fits to this pattern.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        [DataMember]
        public virtual string Prefix { get; set; }

        #region No user settings
        /// <summary>
        /// Gets or sets the crawl request.
        /// This is only a pattern for building the nutch
        /// command to execute on the remote clients.
        /// </summary>
        /// <value>
        /// The crawl request.
        /// </value>
        [DataMember]
        public virtual string CrawlRequest { get; set; }

        /// <summary>
        /// Gets or sets the name of the seed file.
        /// The file where the urls are stored for crawling.
        /// Will be different on each client because we split
        /// the work if there are more than one client defined.
        /// </summary>
        /// <value>
        /// The name of the seed file.
        /// </value>
        [DataMember]
        public virtual string SeedFileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the prefix file.
        /// The file where the prefixes are stored.
        /// </summary>
        /// <value>
        /// The name of the prefix file.
        /// </value>
        [DataMember]
        public virtual string PrefixFileName { get; set; }

        /// <summary>
        /// Gets or sets the base URL path.
        /// The path where nutch finds the urls file.
        /// </summary>
        /// <value>
        /// The base URL path.
        /// </value>
        [DataMember]
        public virtual string BaseUrlPath { get; set; }

        /// <summary>
        /// Gets or sets the base crawl path.
        /// This is a subfolder in BaseUrlPath where we store
        /// the seed.txt for nutch. Its the lagacy of our first
        /// implementation when we called nutch on the local
        /// machine but from different users and may can be removed
        /// but maybe not, so we kept this property.
        /// </summary>
        /// <value>
        /// The base crawl path.
        /// </value>
        [DataMember]
        public virtual string BaseCrawlPath { get; set; }

        /// <summary>
        /// Gets or sets the path format two.
        /// A patern for formatting a string containing two folders.
        /// </summary>
        /// <value>
        /// The path format two.
        /// </value>
        [DataMember]
        public virtual string PathFormatTwo { get; set; }

        /// <summary>
        /// Gets or sets the path format three.
        /// /// A patern for formatting a string containing three folders.
        /// </summary>
        /// <value>
        /// The path format three.
        /// </value>
        [DataMember]
        public virtual string PathFormatThree { get; set; }

        /// <summary>
        /// Gets or sets the path format four.
        /// /// A patern for formatting a string containing four folders.
        /// </summary>
        /// <value>
        /// The path format four.
        /// </value>
        [DataMember]
        public virtual string PathFormatFour { get; set; }

        /// <summary>
        /// Gets or sets the prefix format.
        /// A pattern to format the prefix and the given url
        /// into one string.
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

        /// <summary>
        /// Sets the default values of this
        /// setting or if we instanciate a new
        /// instance of a setting object because
        /// NHibernate needs virtual members but 
        /// it is better to do not call a virtual
        /// member in the constructor if there are
        /// other derived classes which inherits from
        /// this setting. To avoid this problem we
        /// simply call this method after instanciating.
        /// </summary>
        public virtual void SetDefaults()
        {
            var newSettings = GetDefaults() as NutchControllerClientSettings;

            //// Public settings
            if (newSettings != null)
            {
                this.HomePath = newSettings.HomePath;
                this.NutchPath = newSettings.NutchPath;
                this.NutchCommand = newSettings.NutchCommand;
                this.NutchClients = newSettings.NutchClients;
                this.DefaultURLs = newSettings.DefaultURLs;
                this.CrawlDepth = newSettings.CrawlDepth;
                this.CrawlTopN = newSettings.CrawlTopN;
                this.SolrServer = newSettings.SolrServer;
                this.JavaHome = newSettings.JavaHome;
                this.CertificatePath = newSettings.CertificatePath;
                this.Prefix = newSettings.Prefix;

                //// Non public settings
                this.CrawlRequest = newSettings.CrawlRequest;
                this.SeedFileName = newSettings.SeedFileName;
                this.PrefixFileName = newSettings.PrefixFileName;
                this.BaseUrlPath = newSettings.BaseUrlPath;
                this.BaseCrawlPath = newSettings.BaseCrawlPath;
                this.PathFormatTwo = newSettings.PathFormatTwo;
                this.PathFormatThree = newSettings.PathFormatThree;
                this.PathFormatFour = newSettings.PathFormatFour;
                this.PrefixFormat = newSettings.PrefixFormat;
            }
        }

        /// <summary>
        /// Gets the settings with default values.
        /// </summary>
        /// <returns>A new settings object with its default values.</returns>
        public virtual Settings GetDefaults()
        {
            var newSettings = new NutchControllerClientSettings
                {
                    //// Public settings
                    HomePath = string.Empty,
                    NutchPath = string.Empty,
                    NutchCommand = string.Empty,
                    NutchClients = string.Empty,
                    DefaultURLs = "http://nvd.nist.gov/,http://www.heise.de/security/",
                    CrawlDepth = 0,
                    CrawlTopN = 0,
                    SolrServer = string.Empty,
                    JavaHome = string.Empty,
                    CertificatePath = string.Empty,
                    Prefix = string.Empty,

                    //// Non public settings
                    CrawlRequest = "crawl {0} -solr {1} -depth {2} -topN {3}",
                    SeedFileName = "seed.txt",
                    PrefixFileName = "conf/regex-urlfilter.txt",
                    BaseUrlPath = ".nutch/urls",
                    BaseCrawlPath = "crawler",
                    PathFormatTwo = "{0}/{1}",
                    PathFormatThree = "{0}/{1}/{2}",
                    PathFormatFour = "{0}/{1}/{2}/{3}",
                    PrefixFormat = "{0}{1}"
                };

            return newSettings;
        }

        /// <summary>
        /// Determines whether this settings has default values.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this settings has default values; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsDefault()
        {
            return this.Equals(this.GetDefaults() as NutchControllerClientSettings);
        }
    }
}
