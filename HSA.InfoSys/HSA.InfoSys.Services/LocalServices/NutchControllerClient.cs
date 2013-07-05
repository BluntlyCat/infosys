// ------------------------------------------------------------------------
// <copyright file="NutchControllerClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Extensions;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using Renci.SshNet;
    using Renci.SshNet.Common;

    /// <summary>
    /// The NutchControllerClient starts a crawl on the specified host.
    /// </summary>
    public class NutchControllerClient
    {
        /// <summary>
        /// The logger for NutchControllerClient.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("NutchControllerClient");

        /// <summary>
        /// The connection string contains
        /// the username and host like user@host
        /// </summary>
        private readonly string connectionString;

        /// <summary>
        /// The SSH client.
        /// </summary>
        private SshClient client;

        /// <summary>
        /// Gets or sets the home path of the user we want
        /// use to establish a connection over SSH.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// The username with which we want connect.
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

            this.URLs = new List<string>();
            this.connectionString = connectionString;
            this.IsCrawling = false;

            this.InitializeClient(settings);
        }

        /// <summary>
        /// Gets the URLs on which this client crawls.
        /// </summary>
        /// <value>
        /// The URLs.
        /// </value>
        public IList<string> URLs { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [client can crawl]
        /// Can be false if nutch or java is not installed on this
        /// client or on the wrong path or if any error happens
        /// while we establish the connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [can crawl]; otherwise, <c>false</c>.
        /// </value>
        public bool IsClientUsable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this client is crawling.
        /// </summary>
        /// <value>
        /// <c>true</c> if this client is crawling; otherwise, <c>false</c>.
        /// </value>
        private bool IsCrawling { get; set; }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        /// <value>
        /// The hostname.
        /// </value>
        public string Hostname { get; private set; }

        /// <summary>
        /// Gets or sets the name of the prefix file.
        /// The file where the prefixes are stored.
        /// </summary>
        /// <value>
        /// The name of the prefix file.
        /// </value>
        private string PrefixFile { get; set; }

        /// <summary>
        /// Gets or sets the name of the seed file.
        /// The file where the urls are stored for crawling.
        /// Will be different on each client because we split
        /// the work if there are more than one client defined.
        /// </summary>
        /// <value>
        /// The name of the seed file.
        /// </value>
        private string SeedFile { get; set; }

        /// <summary>
        /// Gets or sets the base URL path.
        /// The path where nutch finds the urls file.
        /// </summary>
        /// <value>
        /// The base URL path.
        /// </value>
        private string URLPath { get; set; }

        /// <summary>
        /// Gets or sets the nutch command.
        /// </summary>
        /// <value>
        /// The nutch command.
        /// </value>
        private string CrawlCommand { get; set; }

        /// <summary>
        /// Initializes the client before its next crawl
        /// to be sure it has the correct values if settings
        /// have changed since the last crawl.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void InitializeClient(NutchControllerClientSettings settings)
        {
            if (settings.IsDefault() == false)
            {
                this.homeDir = settings.HomePath;

                this.URLPath = string.Format(
                    settings.PathFormatThree,
                    this.homeDir,
                    settings.BaseUrlPath,
                    settings.BaseCrawlPath);

                this.PrefixFile = string.Format(
                    settings.PathFormatTwo,
                    settings.NutchPath,
                    settings.PrefixFileName);

                this.SeedFile = string.Format(
                    settings.PathFormatTwo,
                    this.URLPath,
                    settings.SeedFileName);

                if (this.sshConnectionInfo == null)
                {
                    this.InitializeSSHClient(settings);
                }
            }
        }

        /// <summary>
        /// Starts the crawl.
        /// If the crawl finishes in any way it returns to the controller.
        /// to tell that this client finished. If the crawl fails it sends
        /// an email to all users who want to crawl the urls this client
        /// was crawling. Furthermore it fetches the log from the client
        /// of nutch and writes it into our own logfile.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="dbManager">The db manager.</param>
        public void StartCrawl(NutchControllerClientSettings settings, DbManager dbManager)
        {
            if (this.IsClientUsable)
            {
                Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_START_CRAWL, this.Hostname);

                try
                {
                    if (this.IsFileExistent("-f", "crawler.log"))
                    {
                        this.RunCommand("mv crawler.log crawler-$(date +%Y-%m-%d-%H-%M).log");
                    }

                    this.IsCrawling = true;
                    var command = this.RunCommand(string.Format("{0} > crawler.log", this.CrawlCommand));
                    this.IsCrawling = false;

                    if (command.Error.Equals(string.Empty) == false)
                    {
                        Log.ErrorFormat(
                            Properties.Resources.NUTCH_CONTROLLER_CLIENT_NUTCH_COMMAND_ERROR,
                            this.Hostname,
                            command.Error);

                        var hadoopLog = this.GetLogfileContent(100, string.Format("{0}{1}", settings.NutchPath, "/logs/hadoop.log"));

                        Log.ErrorFormat(
                            Properties.Resources.NUTCH_CONTROLLER_CLIENT_HADOOP_LOG,
                            this.Hostname,
                            hadoopLog);

                        var mail = new EmailNotifier(dbManager);
                        mail.CrawlFailed(hadoopLog, this.URLs);
                    }

                    var crawlerLog = this.GetLogfileContent(100, "crawler.log");
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
        /// Sets the crawl process before each crawl to be
        /// sure that we have the current settings if they
        /// were updated since the last time.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void SetCrawlProcess(NutchControllerClientSettings settings)
        {
            if (this.IsClientUsable)
            {
                this.CreateUserDir(settings);
                this.AddURL(settings);

                var urlPath = string.Format(
                    settings.PathFormatThree,
                    this.homeDir,
                    settings.BaseUrlPath,
                    settings.BaseCrawlPath);

                string crawlRequest =
                    string.Format(
                    settings.CrawlRequest,
                    urlPath,
                    settings.SolrServer,
                    settings.CrawlDepth,
                    settings.CrawlTopN);

                var setJavaHome = string.Format("export JAVA_HOME='{0}'", settings.JavaHome);
                this.CrawlCommand = string.Format("{0} && {1} {2}", setJavaHome, settings.NutchCommand, crawlRequest);

                Log.DebugFormat(
                    Properties.Resources.NUTCH_CONTROLLER_CLIENT_CRAWL_PROCESS_CREATED,
                    this.CrawlCommand,
                    this.Hostname);
            }
        }

        /// <summary>
        /// Checks the client for usage.
        /// Returns false if nutch or java is not installed
        /// or not on this location we expect or if any error
        /// happens while connecting.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void CheckClientForUsage(NutchControllerClientSettings settings)
        {
            this.IsClientUsable = this.IsNutchInstalled(settings)
                && this.IsFileExistent("-f", this.PrefixFile)
                && this.IsFileExistent("-d", settings.JavaHome);

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
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.CRAWL_CONTROLLER_CLIENT_TO_STRING,
                this.Hostname,
                this.IsClientUsable,
                this.IsCrawling,
                this.URLs.ElementsToString(),
                this.URLPath,
                this.SeedFile,
                this.CrawlCommand);
        }

        /// <summary>
        /// Initializes the SSH client before the next crawl because
        /// settings may have changed...
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void InitializeSSHClient(NutchControllerClientSettings settings)
        {
            try
            {
                var connectionArgs = this.connectionString.Split('@');
                this.Hostname = connectionArgs[1];
                this.username = connectionArgs[0];

                var key = new PrivateKeyFile(settings.CertificatePath);

                this.sshConnectionInfo = new PrivateKeyConnectionInfo(this.Hostname, this.username, key);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
            }
        }

        /// <summary>
        /// Adds the URLs to the seed.txt file this client must crawl.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void AddURL(NutchControllerClientSettings settings)
        {
            string userURLPath = string.Format(
                settings.PathFormatThree,
                this.homeDir,
                settings.BaseUrlPath,
                settings.BaseCrawlPath);

            Log.InfoFormat(
                Properties.Resources.NUTCH_CONTROLLER_CLIENT_USER_PATH_IS,
                this.Hostname,
                userURLPath);

            var prefixUrls = this.GetKnownPrefixes(settings, this.URLs);

            this.AddURLToFile(this.PrefixFile, FileMode.Append, prefixUrls.ToArray());
            this.AddURLToFile(this.SeedFile, FileMode.Create, this.URLs);
        }

        /// <summary>
        /// Gets the known prefixes.
        /// This method fetches the prefix file from the client
        /// and searches for unkown urls. If we have any new url
        /// it adds this to a list and returns this for writing 
        /// into the prefix file on this client.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="urls">The URLs.</param>
        /// <returns>
        /// A list of already known prefixes.
        /// </returns>
        private IEnumerable<string> GetKnownPrefixes(NutchControllerClientSettings settings, IEnumerable<string> urls)
        {
            var prefixUrls = new List<string>();
            var knownPrefixes = this.GetFileContent(settings.Prefix);

            foreach (string url in urls)
            {
                string prefix = string.Format(
                    settings.PrefixFormat,
                    settings.Prefix,
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
        /// Creates the user directory on the client
        /// and proofs if this directory exists after creation.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void CreateUserDir(NutchControllerClientSettings settings)
        {
            string newDirectory = string.Format(
                settings.PathFormatThree,
                this.homeDir,
                settings.BaseUrlPath,
                settings.BaseCrawlPath);

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
        private void AddURLToFile(
            string file,
            FileMode mode,
            IList<string> urls)
        {
            if (urls.Count > 0)
            {
                var insertString = urls.Aggregate(string.Empty, (current, url) => current + string.Format("{0}", url));

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
            var content = new List<string>();

            try
            {
                var fileContent = this.RunCommand(string.Format("cat {0}", this.PrefixFile)).Result;
                Log.DebugFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_GOT_FILE_CONTENT, this.Hostname, fileContent);

                using (var sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(fileContent))))
                {
                    string line;

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
        /// <param name="settings">The settings.</param>
        /// <returns>
        ///   <c>true</c> if [is nutch installed]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNutchInstalled(NutchControllerClientSettings settings)
        {
            bool installed = false;

            try
            {
                installed = this.RunCommand(settings.NutchCommand).ExitStatus == 1;
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
        /// Gets the content of the log file.
        /// </summary>
        /// <param name="n">The n lines of the log file.</param>
        /// <param name="logFile">The log file.</param>
        /// <returns>
        /// The last n lines of the specified log file.
        /// </returns>
        private string GetLogfileContent(int n, string logFile)
        {
            var cmd = this.RunCommand(string.Format("tail -n {0} {1} | cat -n", n, logFile));

            if (cmd.Error.Equals(string.Empty))
            {
                return cmd.Result;
            }

            return cmd.Error;
        }

        /// <summary>
        /// Runs the command on this client.
        /// If an error happens it writes a log
        /// entry for further information of whats going wrong.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The SSH command.</returns>
        private SshCommand RunCommand(string command)
        {
            using (this.client = new SshClient(this.sshConnectionInfo))
            {
                SshCommand sshCommand;

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

                    throw;
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.Hostname,
                        ae);

                    throw;
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.Hostname,
                        ce);

                    throw;
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.Hostname,
                        toe);

                    throw;
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);

                    throw;
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
