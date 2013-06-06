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
        /// <param name="urlDir">The URL directory.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        void StartCrawl(string urlDir, int depth, int topN);

        /// <summary>
        /// Creates the user directory.
        /// </summary>
        /// <param name="user">The username.</param>
        void CreateUserDir(string user);

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="urls">The URLs.</param>
        /// <param name="user">The username.</param>
        void AddURL(List<string> urls, string user);
    }
}
