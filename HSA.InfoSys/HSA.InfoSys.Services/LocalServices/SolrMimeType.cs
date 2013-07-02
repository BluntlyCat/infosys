// ------------------------------------------------------------------------
// <copyright file="SolrMimeType.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    /// <summary>
    /// SolrOutputMimeType represents possible Output - MimeTypes, which solr can generate
    /// </summary>
    public enum SolrMimeType
    {
        /// <summary>
        /// The XML
        /// </summary>
        Xml,

        /// <summary>
        /// The json
        /// </summary>
        Json,

        /// <summary>
        /// The python
        /// </summary>
        Python,

        /// <summary>
        /// The ruby
        /// </summary>
        Ruby,

        /// <summary>
        /// The PHP
        /// </summary>
        Php,

        /// <summary>
        /// The CSV
        /// </summary>
        Csv
    }
}
