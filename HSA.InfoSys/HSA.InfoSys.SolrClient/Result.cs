// ***********************************************************************
// <copyright file="Result.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// 
/////////////////////////////////////////////////////////////////////////

namespace HSA.InfoSys.Common.SolrClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class Result is one Result Solr found in a query
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        /// <value>
        /// The url.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        /// <value>
        /// The Title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Timestamp.
        /// </summary>
        /// <value>
        /// The Timestamp.
        /// </value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Issue was found:\"{0}\" Search result found at Website: {1}\nContent:\n{2}\nAt time:\n{3}",
                this.Title,
                this.Url,
                this.Content,
                this.Timestamp);
        }
    }
}
