// ------------------------------------------------------------------------
// <copyright file="INutchManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Nutch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The interface for using the NutchManager
    /// </summary>
    public interface INutchManager
    {
        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        void StartCrawl(string userName, int depth, int topN);

        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        /// <param name="urls">The URLs.</param>
        void StartCrawl(string userName, int depth, int topN, params string[] urls);

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="urls">The URLs.</param>
        void AddURL(string user, params string[] urls);
    }
}
