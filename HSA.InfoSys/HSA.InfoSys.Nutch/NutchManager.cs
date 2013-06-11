// ------------------------------------------------------------------------
// <copyright file="NutchManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Nutch
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using Renci.SshNet;
    using System;

    /// <summary>
    /// The Nutch Manager handles the WebCrawl
    /// </summary>
    public class NutchManager : INutchManager
    {
        /// <summary>
        /// The logger for NutchManager.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("NutchManager");

        /// <summary>
        /// The nutch manager
        /// </summary>
        private static INutchManager nutchManager;

        /// <summary>
        /// The path to prefix file.
        /// </summary>
        private string prefixPath = Properties.Settings.Default.PREFIX_PATH;

        /// <summary>
        /// The path to URL file.
        /// </summary>
        private string baseUrlPath = Properties.Settings.Default.BASEURL_PATH;

        /// <summary>
        /// The seed file name.
        /// </summary>
        private string fileName = Properties.Settings.Default.SEED_FILENAME;

        /// <summary>
        /// The home directory.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// Prevents a default instance of the <see cref="NutchManager"/> class from being created.
        /// </summary>
        private NutchManager()
        {
#if !MONO
            this.homeDir = Environment.GetEnvironmentVariable("HOMEPATH");
#else
            this.homeDir = Environment.GetEnvironmentVariable("HOME");
#endif
        }

        /// <summary>
        /// Gets the NutchManager and ensures that the configuration
        /// will be executed only once and that there is only one NutchManager.
        /// </summary>
        /// <returns>
        /// The NutchManager
        /// </returns>
        public static INutchManager ManagerFactory
        {
            get
            {
                if (nutchManager == null)
                {
                    Log.Debug(Properties.Resources.NUTCHMANAGER_NO_MANAGER_FOUND);
                    nutchManager = new NutchManager();
                }

                return nutchManager;
            }
        }

        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        public void StartCrawl(string userName, int depth, int topN)
        {
            this.Start(userName, depth, topN);
        }

        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        /// <param name="urls">The urls.</param>
        public void StartCrawl(string userName, int depth, int topN, params string[] urls)
        {
            this.AddURL(userName, urls);
            this.Start(userName, depth, topN);
        }

        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        public void Start(string userName, int depth, int topN)
        {
            Process nutch = new Process();

            var urlPath = string.Format(
                Properties.Settings.Default.PATH_FORMAT_THREE,
                this.homeDir,
                this.baseUrlPath,
                userName);

            string crawlRequest =
                string.Format(
                Properties.Settings.Default.NUTCH_CRAWL_REQUEST,
                urlPath,
                Properties.Settings.Default.SOLRSERVER,
                depth,
                topN);

            this.CreateUserDir(userName);

            nutch.StartInfo.FileName = Properties.Settings.Default.NUTCH_COMMAND;
            nutch.StartInfo.Arguments = crawlRequest;

            nutch.Start();

            Log.Info(string.Format(Properties.Resources.CRAWL_REQUEST_SENT, crawlRequest));
        }

        /// <summary>
        /// Creates the user directory.
        /// </summary>
        /// <param name="user">The username.</param>
        private void CreateUserDir(string user)
        {
            string newDirectory = string.Format(
                Properties.Settings.Default.PATH_FORMAT_THREE,
                this.homeDir,
                this.baseUrlPath,
                user);

            Directory.CreateDirectory(newDirectory);

            File.CreateText(string.Format(
                Properties.Settings.Default.PATH_FORMAT_TWO,
                newDirectory,
                this.fileName));
        }

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="urls">The URLs.</param>
        public void AddURL(string user, params string[] urls)
        {
            this.CreateUserDir(user);

            string userURLPath = string.Format(
                Properties.Settings.Default.PATH_FORMAT_FOUR,
                this.homeDir,
                this.baseUrlPath,
                user,
                this.fileName);

            List<string> prefixUrls = new List<string>();

            foreach (string url in urls)
            {
                string prefix = string.Format(
                    Properties.Settings.Default.PREFIX_FORMAT,
                    Properties.Settings.Default.PREFIX,
                    url);

                if (!this.GetFileContent(Properties.Settings.Default.PREFIX, this.prefixPath).Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                }
            }

            this.AddURLToFile(this.prefixPath, prefixUrls.ToArray());
            this.AddURLToFile(userURLPath, urls);
        }

        /// <summary>
        /// Adds the url in the corresponding file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="urls">The array of url.</param>
        private void AddURLToFile(string path, params string[] urls)
        {
            foreach (string url in urls)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(url);
                }
            }
        }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>A list containing the file content.</returns>
        private List<string> GetFileContent(string pattern, string filePath)
        {
            List<string> content = new List<string>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                var line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(pattern))
                    {
                        content.Add(line);
                    }
                }
            }

            return content;
        }
    }
}
