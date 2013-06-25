// ------------------------------------------------------------------------
// <copyright file="NutchControllerClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

    /// <summary>
    /// The Nutch Manager handles the WebCrawl
    /// </summary>
    public class NutchControllerClient
    {
        /// <summary>
        /// The logger for NutchManager.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("NutchControllerClient");

        /// <summary>
        /// The settings.
        /// </summary>
        private NutchControllerClientSettings settings;

        /// <summary>
        /// The home directory.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchControllerClient"/> class.
        /// </summary>
        public NutchControllerClient()
        {
            this.settings = DBManager.ManagerFactory(Guid.NewGuid()).GetSettingsFor<NutchControllerClientSettings>();
#if !MONO
            this.homeDir = Environment.GetEnvironmentVariable("HOMEPATH");
#else
            this.homeDir = Environment.GetEnvironmentVariable("HOME");
#endif
        }

        /// <summary>
        /// Creates the crawl process.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        /// <param name="urls">The URLs.</param>
        /// <returns>
        /// A new crawl process.
        /// </returns>
        public Process CreateCrawlProcess(string folder, int depth, int topN, params string[] urls)
        {
            Process nutch = new Process();

            this.CreateUserDir(folder);
            this.AddURL(folder, urls);

            var urlPath = string.Format(
                this.settings.PathFormatThree,
                this.homeDir,
                this.settings.BaseUrlPath,
                folder);

            string crawlRequest =
                string.Format(
                this.settings.CrawlRequest,
                urlPath,
                this.settings.SolrServer,
                depth,
                topN);

            nutch.StartInfo.FileName = this.settings.NutchCommand;
            nutch.StartInfo.Arguments = crawlRequest;

            Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CRAWL_PROCESS_CREATED, crawlRequest);

            return nutch;
        }

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="urls">The URLs.</param>
        private void AddURL(string folder, params string[] urls)
        {
            string userURLPath = string.Format(
                this.settings.PathFormatThree,
                this.homeDir,
                this.settings.BaseUrlPath,
                folder);

            var prefixUrls = new List<string>();

            this.AddURLToFile(userURLPath, this.settings.SeedFileName, urls);
        }

        /// <summary>
        /// Creates the user directory.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private void CreateUserDir(string folder)
        {
            string newDirectory = string.Format(
                this.settings.PathFormatThree,
                this.homeDir,
                this.settings.BaseUrlPath,
                folder);

            var info = Directory.CreateDirectory(newDirectory);

            if (!info.Exists)
            {
                Log.ErrorFormat(Properties.Resources.LOG_DIRECTORY_CREATION_ERROR, newDirectory);
            }
        }

        /// <summary>
        /// Adds the url in the corresponding file.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="urls">The array of url.</param>
        private void AddURLToFile(string path, string fileName, params string[] urls)
        {
            var file = string.Format(
                this.settings.PathFormatTwo,
                path,
                fileName);

            try
            {
                using (StreamWriter sw = File.CreateText(file))
                {
                    foreach (string url in urls)
                    {
                        sw.WriteLine(url);
                        Log.DebugFormat(Properties.Resources.LOG_FILE_WRITING_SUCCESS, url);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                this.CreateFile(path, fileName);
                this.AddURLToFile(path, fileName, urls);

                Log.DebugFormat(
                    Properties.Resources.LOG_FILE_CREATION_SUCCESS,
                    fileName,
                    path);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_FILE_WRITING_ERROR, file, e);
            }
        }

        /// <summary>
        /// Creates the file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="createNew">if set to <c>true</c> [create new].</param>
        private void CreateFile(string path, string fileName, bool createNew = false)
        {
            var info = new DirectoryInfo(path);

            if (info.Exists && (!info.GetFiles().Contains(new FileInfo(fileName)) || createNew))
            {
                var file = File.Create(
                    string.Format(
                    this.settings.PathFormatTwo,
                    path,
                    fileName));

                file.Close();
            }
        }
    }
}
