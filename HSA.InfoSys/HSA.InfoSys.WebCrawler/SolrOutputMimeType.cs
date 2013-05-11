using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSA.InfoSys.WebCrawler
{
    /// <summary>
    /// SolrOutputMimeType represents possible Ouput - MimeTypes, which solr can generate
    /// </summary>
    public enum SolrOutputMimeType
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
