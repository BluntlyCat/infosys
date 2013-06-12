// ***********************************************************************
// Assembly         : HSA.InfoSys.SolrClient
// Author           : user
// Created          : 06-12-2013
//
// Last Modified By : user
// Last Modified On : 06-12-2013
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
        string content = null;
        /// <summary>
        /// The URL
        /// </summary>
        string url = null;
        /// <summary>
        /// The tstamp
        /// </summary>
        string tstamp = null;
    }
}
