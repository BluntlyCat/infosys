// ------------------------------------------------------------------------
// <copyright file="ResultPot.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.SolrClient
{
    using System;
    using System.Collections.Generic;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// Is a container for search results from Solr.
    /// </summary>
    public class ResultPot
    {
        /// <summary>
        /// The logger for database manager.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("ResultPot");

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultPot"/> class.
        /// </summary>
        public ResultPot()
        {
            this.EntityId = Guid.Empty;
            this.Results = new List<Result>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultPot"/> class.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        public ResultPot(Guid entityId)
        {
            this.EntityId = entityId;
            this.Results = new List<Result>();
        }

        /// <summary>
        /// Gets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public Guid EntityId { get; private set; }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public List<Result> Results { get; private set; }
    }
}
