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
    using System.Net.Sockets;
    using System.Text;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using Renci.SshNet;

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
        /// The crawl process.
        /// </summary>
        private Process crawlProcess;

        /// <summary>
        /// The SSH client.
        /// </summary>
        private SshClient sshClient;

        /// <summary>
        /// The home directory.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchControllerClient" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="connectionString">The connection string.</param>
        public NutchControllerClient(NutchControllerClientSettings settings, string connectionString)
        {
            this.settings = settings;
#if !MONO
            this.homeDir = Environment.GetEnvironmentVariable("HOMEPATH");
#else
            this.homeDir = Environment.GetEnvironmentVariable("HOME");
#endif
            this.URLs = new List<string>();

            try
            {
                var connectionArgs = connectionString.Split('@');
#warning Password must be decrypted if saved encrypted in database
                var connectionInfo = new PasswordConnectionInfo(connectionArgs[1], 22, connectionArgs[0], this.settings.NutchClientsPassword);

                this.sshClient = new SshClient(connectionInfo);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
        }

        /// <summary>
        /// Invoked if nutch is not found.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="nutchFound">if set to <c>true</c> [nutch found].</param>
        public delegate void NutchNotFoundHandler(object sender, bool nutchFound);

        /// <summary>
        /// Occurs when [on nutch not found].
        /// </summary>
        public event NutchNotFoundHandler OnNutchNotFound;

        /// <summary>
        /// Gets the URLs.
        /// </summary>
        /// <value>
        /// The URLs.
        /// </value>
        public IList<string> URLs { get; private set; }

        /// <summary>
        /// Starts the crawl.
        /// </summary>
        public void StartCrawl()
        {
            try
            {
                var command = this.sshClient.CreateCommand("sleep 5");

                this.sshClient.Connect();
                command.Execute();
                //// this.crawlProcess.Start();
                //// this.crawlProcess.WaitForExit();
            }
            catch (SocketException se)
            {
                Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_NUTCH_NOT_FOUND, se);
                this.OnNutchNotFound(this, false);
            }
            catch (Exception e)
            {
                Log.DebugFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
            finally
            {
                this.sshClient.Disconnect();
            }
        }

        /// <summary>
        /// Creates the crawl process.
        /// </summary>
        public void SetCrawlProcess()
        {
            Process nutch = new Process();

            this.CreateUserDir(this.settings.BaseCrawlPath);
            this.AddURL(this.settings.BaseCrawlPath, this.URLs);

            var urlPath = string.Format(
                this.settings.PathFormatThree,
                this.homeDir,
                this.settings.BaseUrlPath,
                this.settings.BaseCrawlPath);

            string crawlRequest =
                string.Format(
                this.settings.CrawlRequest,
                urlPath,
                this.settings.SolrServer,
                this.settings.CrawlDepth,
                this.settings.CrawlTopN);

            nutch.StartInfo.FileName = this.settings.NutchCommand;
            nutch.StartInfo.Arguments = crawlRequest;

            Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CRAWL_PROCESS_CREATED, crawlRequest);

            this.crawlProcess = nutch;
        }

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="urls">The URLs.</param>
        private void AddURL(string folder, IList<string> urls)
        {
            string userURLPath = string.Format(
                this.settings.PathFormatThree,
                this.homeDir,
                this.settings.BaseUrlPath,
                folder);

            var prefixUrls = this.GetKnownPrefixes(urls);

            this.AddURLToFile(this.settings.PrefixPath, this.settings.PrefixFileName, FileMode.Append, prefixUrls.ToArray());
            this.AddURLToFile(userURLPath, this.settings.SeedFileName, FileMode.Create, urls);
        }

        /// <summary>
        /// Gets the known prefixes.
        /// </summary>
        /// <param name="urls">The URLs.</param>
        /// <returns>A list of already known prefixes.</returns>
        private IList<string> GetKnownPrefixes(IList<string> urls)
        {
            var prefixUrls = new List<string>();
            var knownPrefixes = this.GetFileContent(
                this.settings.Prefix,
                this.settings.PrefixPath,
                this.settings.PrefixFileName);

            foreach (string url in urls)
            {
                string prefix = string.Format(
                    this.settings.PrefixFormat,
                    this.settings.Prefix,
                    url);

                if (!knownPrefixes.Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                    Log.DebugFormat(Properties.Resources.LOG_PREFIX_ADDED, prefix);
                }
            }

            return prefixUrls;
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
        /// <param name="mode">The mode.</param>
        /// <param name="urls">The array of url.</param>
        private void AddURLToFile(string path, string fileName, FileMode mode, IList<string> urls)
        {
            var file = string.Format(
                this.settings.PathFormatTwo,
                path,
                fileName);

            try
            {
                using (FileStream fs = File.Open(file, mode, FileAccess.Write))
                {
                    foreach (string url in urls)
                    {
                        var bytes = Encoding.ASCII.GetBytes(string.Format("{0}{1}", url, "\r\n"));

                        fs.Write(bytes, 0, bytes.Length);
                        Log.DebugFormat(Properties.Resources.LOG_FILE_WRITING_SUCCESS, url);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                this.CreateFile(path, fileName);
                this.AddURLToFile(path, fileName, mode, urls);

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
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// A list containing the file content.
        /// </returns>
        private List<string> GetFileContent(string pattern, string filePath, string fileName)
        {
            List<string> content = new List<string>();

            var prefixFile = string.Format(
                this.settings.PathFormatTwo,
                filePath,
                fileName);

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
                this.CreateFile(this.settings.PrefixPath, this.settings.PrefixFileName, true);
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
                    this.settings.PathFormatTwo,
                    path,
                    fileName));

                file.Close();
            }
        }
    }
}
