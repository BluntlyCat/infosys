// ------------------------------------------------------------------------
// <copyright file="NutchManager.cs" company="HSA.InfoSys">
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
    using HSA.InfoSys.Common.Logging;
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
        /// The path to prefix file.
        /// </summary>
        private string prefixPath;

        /// <summary>
        /// The home directory.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// Prevents a default instance of the <see cref="NutchControllerClient"/> class from being created.
        /// </summary>
        public NutchControllerClient()
        {
#if !MONO
            this.homeDir = Environment.GetEnvironmentVariable("HOMEPATH");
#else
            this.homeDir = Environment.GetEnvironmentVariable("HOME");
#endif
            this.prefixPath = string.Format(
                Properties.Settings.Default.PATH_FORMAT_TWO,
                this.homeDir,
                Properties.Settings.Default.PREFIX_PATH);
        }

        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        /// <param name="urls">The URLs.</param>
        public Process GetCrawlProcess(string userName, int depth, int topN, params string[] urls)
        {
            Process nutch = new Process();

            this.CreateUserDir(userName);
            this.AddURL(userName, urls);

            var urlPath = string.Format(
                Properties.Settings.Default.PATH_FORMAT_THREE,
                this.homeDir,
                Properties.Settings.Default.BASEURL_PATH,
                userName);

            string crawlRequest =
                string.Format(
                Properties.Settings.Default.NUTCH_CRAWL_REQUEST,
                urlPath,
                Properties.Settings.Default.SOLRSERVER,
                depth,
                topN);

            nutch.StartInfo.FileName = Properties.Settings.Default.NUTCH_COMMAND;
            nutch.StartInfo.Arguments = crawlRequest;

            Log.DebugFormat(Properties.Resources.CRAWL_REQUEST_SENT, crawlRequest);

            return nutch;
        }

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="user">The username.</param>
        /// <param name="urls">The URLs.</param>
        private void AddURL(string user, params string[] urls)
        {
            string userURLPath = string.Format(
                Properties.Settings.Default.PATH_FORMAT_THREE,
                this.homeDir,
                Properties.Settings.Default.BASEURL_PATH,
                user);

            var prefixUrls = new List<string>();
            var knownPrefixes = this.GetFileContent(Properties.Settings.Default.PREFIX, this.prefixPath);

            foreach (string url in urls)
            {
                string prefix = string.Format(
                    Properties.Settings.Default.PREFIX_FORMAT,
                    Properties.Settings.Default.PREFIX,
                    url);

                if (!knownPrefixes.Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                    Log.DebugFormat(Properties.Resources.LOG_PREFIX_ADDED, prefix);
                }
            }

            this.AddURLToFile(this.prefixPath, Properties.Settings.Default.PREFIX_FILENAME, prefixUrls.ToArray());
            this.AddURLToFile(userURLPath, Properties.Settings.Default.SEED_FILENAME, urls);
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
                Properties.Settings.Default.BASEURL_PATH,
                user);

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
                Properties.Settings.Default.PATH_FORMAT_TWO,
                path,
                fileName);

            try
            {
                using (StreamWriter sw = File.AppendText(file))
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
        /// Gets the content of the file.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>A list containing the file content.</returns>
        private List<string> GetFileContent(string pattern, string filePath)
        {
            List<string> content = new List<string>();

            var prefixFile = string.Format(
                Properties.Settings.Default.PATH_FORMAT_TWO,
                filePath,
                Properties.Settings.Default.PREFIX_FILENAME);

            try
            {
                using (StreamReader sr = new StreamReader(prefixFile))
                {
                    var line = string.Empty;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(pattern))
                        {
                            content.Add(line);
                        }
                    }

                    Log.DebugFormat(Properties.Resources.LOG_FILE_READING_SUCCESS, filePath);
                }
            }
            catch (FileNotFoundException)
            {
                Log.DebugFormat(Properties.Resources.LOG_PREFIX_FILE_NOT_FOUND);
                this.CreateFile(this.prefixPath, Properties.Settings.Default.PREFIX_FILENAME, true);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_FILE_READING_ERROR, filePath, e);
            }

            return content;
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
                    Properties.Settings.Default.PATH_FORMAT_TWO,
                    path,
                    fileName));

                file.Close();
            }
        }
    }
}
