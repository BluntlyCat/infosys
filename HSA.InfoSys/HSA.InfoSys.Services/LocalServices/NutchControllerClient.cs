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
    using Renci.SshNet.Common;

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
        /// The SSH client.
        /// </summary>
        private SshClient client;

        /// <summary>
        /// The home directory.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// The username.
        /// </summary>
        private string username;

        /// <summary>
        /// The SSH connection info.
        /// </summary>
        private PrivateKeyConnectionInfo sshConnectionInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchControllerClient" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="connectionString">The connection string.</param>
        public NutchControllerClient(NutchControllerClientSettings settings, string connectionString)
        {
            Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CREATE_NEW, connectionString);

            this.settings = settings;

            this.homeDir = this.settings.HomePath;
            this.URLs = new List<string>();

            try
            {
                var connectionArgs = connectionString.Split('@');
                this.Hostname = connectionArgs[1];
                this.username = connectionArgs[0];

                var key = new PrivateKeyFile(this.settings.CertificatePath);

                this.sshConnectionInfo = new PrivateKeyConnectionInfo(this.Hostname, this.username, key);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }

            this.URLPath = string.Format(
                this.settings.PathFormatThree,
                this.homeDir,
                this.settings.BaseUrlPath,
                this.settings.BaseCrawlPath);

            this.PrefixFile = string.Format(
                this.settings.PathFormatTwo,
                this.settings.PrefixPath,
                this.settings.PrefixFileName);

            this.SeedFile = string.Format(
                this.settings.PathFormatTwo,
                this.URLPath,
                this.settings.SeedFileName);
        }

        /// <summary>
        /// Gets the URLs.
        /// </summary>
        /// <value>
        /// The URLs.
        /// </value>
        public IList<string> URLs { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [nutch found].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [nutch found]; otherwise, <c>false</c>.
        /// </value>
        public bool IsClientUsable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [prefix file found].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [prefix file found]; otherwise, <c>false</c>.
        /// </value>
        public bool PrefixFileFound { get; private set; }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname { get; private set; }

        /// <summary>
        /// Gets the prefix file.
        /// </summary>
        /// <value>
        /// The prefix file.
        /// </value>
        public string PrefixFile { get; private set; }

        /// <summary>
        /// Gets the seed file.
        /// </summary>
        /// <value>
        /// The seed file.
        /// </value>
        public string SeedFile { get; private set; }

        /// <summary>
        /// Gets the URL path.
        /// </summary>
        /// <value>
        /// The URL path.
        /// </value>
        public string URLPath { get; private set; }

        /// <summary>
        /// Gets the crawl command.
        /// </summary>
        /// <value>
        /// The crawl command.
        /// </value>
        public string CrawlCommand { get; private set; }

        /// <summary>
        /// Starts the crawl.
        /// </summary>
        public void StartCrawl()
        {
            if (this.IsClientUsable)
            {
                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_START_CRAWL, this.Hostname);

                try
                {
                    this.RunCommand("echo '' > crawler.log");

                    var command = this.RunCommand(string.Format("{0} >> crawler.log", this.CrawlCommand));

                    if (!command.Error.Equals(string.Empty))
                    {
                        Log.ErrorFormat(
                            Properties.Resources.NUTCH_CONTROLLER_CLIENT_NUTCH_COMMAND_ERROR,
                            this.Hostname,
                            command.Error);
                    }

                    var crawlerLog = this.RunCommand("cat crawler.log").Result;
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CRAWLER_LOG, this.Hostname, crawlerLog);

                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CRAWL_FINISHED, this.Hostname);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_SSH_ERROR, e);
                }
            }
        }

        /// <summary>
        /// Creates the crawl process.
        /// </summary>
        public void SetCrawlProcess()
        {
            if (this.IsClientUsable)
            {
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

                var setJavaHome = string.Format("export JAVA_HOME='{0}'", this.settings.JavaHome);
                this.CrawlCommand = string.Format("{0} && {1} {2}", setJavaHome, this.settings.NutchCommand, crawlRequest);

                Log.DebugFormat(
                    Properties.Resources.NUTCH_CONTROLLER_CLIENT_CRAWL_PROCESS_CREATED,
                    this.CrawlCommand,
                    this.Hostname);
            }
        }

        /// <summary>
        /// Checks the client for usage.
        /// </summary>
        public void CheckClientForUsage()
        {
            this.IsClientUsable = this.IsNutchInstalled()
                && this.IsFileExistent("-f", this.PrefixFile)
                && this.IsFileExistent("-d", this.settings.JavaHome);

            if (this.IsClientUsable)
            {
                Log.InfoFormat(
                Properties.Resources.NUTCH_CONTROLLER_CLIENT_HOST_IS_USABLE,
                this.Hostname,
                this.IsClientUsable);
            }
            else
            {
                Log.WarnFormat(
                Properties.Resources.NUTCH_CONTROLLER_CLIENT_HOST_IS_USABLE,
                this.Hostname,
                this.IsClientUsable);
            }
        }

        /// <summary>
        /// Disconnects this client.
        /// </summary>
        public void Disconnect()
        {
            if (this.client != null && this.client.IsConnected)
            {
                this.client.Disconnect();
                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_DISCONNECT, this.Hostname);
            }
            else
            {
                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_NOT_CONNECTED, this.Hostname);
            }
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

            Log.InfoFormat(
                Properties.Resources.NUTCH_CONTROLLER_CLIENT_USER_PATH_IS,
                this.Hostname,
                userURLPath);

            var prefixUrls = this.GetKnownPrefixes(urls);

            this.AddURLToFile(this.PrefixFile, FileMode.Append, prefixUrls.ToArray());
            this.AddURLToFile(this.SeedFile, FileMode.Create, urls);
        }

        /// <summary>
        /// Gets the known prefixes.
        /// </summary>
        /// <param name="urls">The URLs.</param>
        /// <returns>A list of already known prefixes.</returns>
        private IList<string> GetKnownPrefixes(IList<string> urls)
        {
            var prefixUrls = new List<string>();
            var knownPrefixes = this.GetFileContent(this.settings.Prefix);

            foreach (string url in urls)
            {
                string prefix = string.Format(
                    this.settings.PrefixFormat,
                    this.settings.Prefix,
                    url);

                if (!knownPrefixes.Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                    Log.InfoFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_PREFIX_ADDED,
                        prefix,
                        this.Hostname);
                }
                else
                {
                    Log.InfoFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_PREFIX_EXISTS,
                        prefix,
                        this.Hostname);
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

            Log.InfoFormat(
                Properties.Resources.NUTCH_CONTROLLER_CLIENT_CREATE_DIRECTORY,
                newDirectory,
                this.Hostname);

            try
            {
                this.RunCommand(string.Format("mkdir -p {0}", newDirectory));

                if (!this.IsFileExistent("-d", newDirectory))
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_DIRECTORY_CREATION_ERROR,
                        newDirectory,
                        this.Hostname);
                }
                else
                {
                    Log.InfoFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_DIRECTORY_CREATION_SUCCESS,
                        newDirectory,
                        this.Hostname);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_SSH_ERROR, e);
            }
        }

        /// <summary>
        /// Adds the url in the corresponding file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="urls">The array of url.</param>
        private void AddURLToFile(string file, FileMode mode, IList<string> urls)
        {
            var insertString = string.Empty;

            foreach (string url in urls)
            {
                insertString += string.Format("{0}{1}", url, "\n");
            }

            Log.DebugFormat(
                Properties.Resources.NUTCH_CONTROLLER_CLIENT_WRITE_FILE,
                insertString,
                file,
                mode,
                this.Hostname);

            try
            {
                switch (mode)
                {
                    case FileMode.Append:
                        this.RunCommand(string.Format("echo '{0}' >> {1}", insertString, file));
                        break;

                    case FileMode.Create:
                        this.RunCommand(string.Format("echo '{0}' > {1}", insertString, file));
                        break;
                }

                Log.InfoFormat(
                    Properties.Resources.NUTCH_CONTROLLER_CLIENT_WRITING_SUCCESS,
                    insertString,
                    this.Hostname);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_FILE_WRITING_ERROR, file, this.Hostname, e);
            }
        }

        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>
        /// A list containing the file content.
        /// </returns>
        private List<string> GetFileContent(string pattern)
        {
            List<string> content = new List<string>();

            try
            {
                var fileContent = this.RunCommand(string.Format("cat {0}", this.PrefixFile)).Result;
                Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_GOT_FILE_CONTENT, this.Hostname, fileContent);

                using (StreamReader sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(fileContent))))
                {
                    var line = string.Empty;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(pattern))
                        {
                            content.Add(line);
                            Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_ADD_PREFIX, line);
                        }
                    }

                    Log.DebugFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_FILE_READING_SUCCESS,
                        this.PrefixFile,
                        this.Hostname);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_SSH_ERROR, e);
            }

            return content;
        }

        /// <summary>
        /// Determines whether [is nutch installed].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is nutch installed]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNutchInstalled()
        {
            bool installed = false;

            try
            {
                installed = this.RunCommand("nutch").ExitStatus == 1;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_SSH_ERROR, e);
            }

            if (installed)
            {
                Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_NUTCH_INSTALLED, this.Hostname);
            }
            else
            {
                Log.WarnFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_NUTCH_NOT_INSTALLED, this.Hostname);
            }

            return installed;
        }

        /// <summary>
        /// Determines whether [is file existent] [the specified mode].
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="file">The file.</param>
        /// <returns>
        ///   <c>true</c> if [is file existent] [the specified mode]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFileExistent(string mode, string file)
        {
            bool exists = false;

            try
            {
                exists = this.RunCommand(string.Format("test {0} {1}", mode, file)).ExitStatus == 0;
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_SSH_ERROR, e);
            }

            if (exists)
            {
                Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_FILE_EXISTS, file, mode, this.Hostname);
            }
            else
            {
                Log.WarnFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_FILE_NOT_EXISTS, file, mode, this.Hostname);
            }

            return exists;
        }

        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The SSH command.</returns>
        private SshCommand RunCommand(string command)
        {
            SshCommand sshCommand = null;

            using (this.client = new SshClient(this.sshConnectionInfo))
            {
                try
                {
                    this.client.Connect();
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECT, this.Hostname);

                    sshCommand = this.client.RunCommand(command);
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_COMMAND_EXECUTED, command, this.Hostname);
                }
                catch (SocketException se)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_SOCKET_EXCEPTION,
                        this.Hostname,
                        se);

                    throw se;
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.Hostname,
                        ae);

                    throw ae;
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.Hostname,
                        ce);

                    throw ce;
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.Hostname,
                        toe);

                    throw toe;
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                    throw e;
                }
                finally
                {
                    this.Disconnect();
                }

                return sshCommand;
            }
        }
    }
}
