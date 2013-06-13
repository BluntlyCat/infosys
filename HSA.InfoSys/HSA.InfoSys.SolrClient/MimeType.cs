// ------------------------------------------------------------------------
// <copyright file="MimeType.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.SolrClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// SolrOutputMimeType represents possible Output - MimeTypes, which solr can generate
    /// </summary>
    public enum MimeType
    {
        /// <summary>
        /// The XML
        /// </summary>
        xml,

        /// <summary>
        /// The json
        /// </summary>
        json,

        /// <summary>
        /// The python
        /// </summary>
        python,

        /// <summary>
        /// The ruby
        /// </summary>
        ruby,

        /// <summary>
        /// The PHP
        /// </summary>
        php,

        /// <summary>
        /// The CSV
        /// </summary>
        csv
    }
}
