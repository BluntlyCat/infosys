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
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Tstamp { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "Search result found at {0}\nContent:\n{1}\nAt time:\n{2}",
                this.Url,
                this.Content,
                this.Tstamp);
        }
    }
}
