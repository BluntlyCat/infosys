﻿// ------------------------------------------------------------------------
// <copyright file="NutchControllerClientSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System.Runtime.Serialization;

    [DataContract]
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
    }
}