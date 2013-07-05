// ------------------------------------------------------------------------
// <copyright file="SolrResultPot.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using HSA.InfoSys.Common.Entities;

    /// <summary>
    /// Is a container for search results from Solr.
    /// </summary>
    public class SolrResultPot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolrResultPot" /> class.
        /// </summary>
        /// <param name="componentID">The entity id.</param>
        public SolrResultPot(Guid componentID)
        {
            this.ComponentID = componentID;

            this.Results = new List<Result>();
        }

        /// <summary>
        /// Gets the componentGUID to which this pot belongs.
        /// </summary>
        /// <value>
        /// The componentGUID.
        /// </value>
        public Guid ComponentID { get; private set; }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public List<Result> Results { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has results.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has results; otherwise, <c>false</c>.
        /// </value>
        public bool HasResults
        {
            get
            {
                return this.Results.Count > 0;
            }
        }
    }
}
