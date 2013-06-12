// ***********************************************************************
// <copyright file="Result.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSA.InfoSys.Common.SolrClient
{
    /// <summary>
    /// Class Result is one Result Solr found in a query
    /// </summary>
    class Result
    {
        /// <summary>
        /// The content
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// The URL
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// The tstamp
        /// </summary>
        public DateTime tstamp { get; set; }
    }
}
