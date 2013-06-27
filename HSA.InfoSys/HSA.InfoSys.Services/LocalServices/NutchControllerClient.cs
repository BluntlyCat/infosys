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
    using System.Threading;
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
        /// The crawl process.
        /// </summary>
        private Process crawlProcess;

        /// <summary>
        /// The home directory.
        /// </summary>
        private string homeDir;

        /// <summary>
        /// The hostname.
        /// </summary>
        private string hostname;

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
                this.hostname = connectionArgs[1];
                this.username = connectionArgs[0];

                var key = new PrivateKeyFile("Certificates/devteam.id.rsa");

                this.sshConnectionInfo = new PrivateKeyConnectionInfo(this.hostname, this.username, key);
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
            using (var client = new SshClient(this.sshConnectionInfo))
            {
                try
                {
                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_START_CRAWL, this.hostname);

                    client.Connect();
                    var command = client.CreateCommand("sleep 5");
                    command.Execute();

                    Log.InfoFormat(Properties.Resources.NUTCH_CONTROLLER_CRAWL_FINISHED, this.hostname);
                }
                catch (SocketException se)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_SOCKET_EXCEPTION,
                        this.hostname,
                        se);

                    this.OnNutchNotFound(this, false);
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.hostname,
                        ae);
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.hostname,
                        ce);
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.hostname,
                        toe);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
                finally
                {
                    client.Disconnect();
                }
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
            using (var client = new SshClient(this.sshConnectionInfo))
            {
                try
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
                catch (SocketException se)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_SOCKET_EXCEPTION,
                        this.hostname,
                        se);

                    this.OnNutchNotFound(this, false);
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.hostname,
                        ae);
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.hostname,
                        ce);
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.hostname,
                        toe);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
                finally
                {
                    client.Disconnect();
                }
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
            using (var client = new SshClient(this.sshConnectionInfo))
            {
                var file = string.Format(
                    this.settings.PathFormatTwo,
                    path,
                    fileName);

                try
                {
                    client.Connect();

                    var insertString = string.Empty;

                    foreach (string url in urls)
                    {
                        insertString += string.Format("{0}{1}", url, "\n");
                    }

                    switch (mode)
                    {
                        case FileMode.Append:
                            client.RunCommand(string.Format("echo '{0}' >> {1}", insertString, file));
                            break;
                        case FileMode.Create:
                            client.RunCommand(string.Format("echo '{0}' > {1}", insertString, file));
                            break;
                    }
                }
                catch (SocketException se)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_SOCKET_EXCEPTION,
                        this.hostname,
                        se);

                    this.OnNutchNotFound(this, false);
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.hostname,
                        ae);
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.hostname,
                        ce);
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.hostname,
                        toe);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
                finally
                {
                    client.Disconnect();
                }
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
            using (var client = new SshClient(this.sshConnectionInfo))
            {
                List<string> content = new List<string>();

                var prefixFile = string.Format(
                    this.settings.PathFormatTwo,
                    filePath,
                    fileName);

                try
                {
                    client.Connect();
                    var cmd = client.RunCommand(string.Format("cat {0}", prefixFile));

                    var fileContent = cmd.Result;

                    using (StreamReader sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(fileContent))))
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
                catch (SocketException se)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_SOCKET_EXCEPTION,
                        this.hostname,
                        se);

                    this.OnNutchNotFound(this, false);
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.hostname,
                        ae);
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.hostname,
                        ce);
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.hostname,
                        toe);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
                finally
                {
                    client.Disconnect();
                }

                return content;
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
            using (var client = new SshClient(this.sshConnectionInfo))
            {
                try
                {
                    var info = new DirectoryInfo(path);

                    if (info.Exists && (!info.GetFiles().Contains(new FileInfo(fileName)) || createNew))
                    {
                        var file = string.Format(
                            this.settings.PathFormatTwo,
                            path,
                            fileName);

                        client.Connect();
                        client.RunCommand(string.Format("touch {0}", file));
                    }
                }
                catch (SocketException se)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_SOCKET_EXCEPTION,
                        this.hostname,
                        se);

                    this.OnNutchNotFound(this, false);
                }
                catch (SshAuthenticationException ae)
                {
                    Log.ErrorFormat(
                        Properties.Resources.NUTCH_CONTROLLER_CLIENT_AUTHENTICATION_EXCEPTION,
                        this.hostname,
                        ae);
                }
                catch (SshConnectionException ce)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_CONNECTION_EXEPTION,
                        this.hostname,
                        ce);
                }
                catch (SshOperationTimeoutException toe)
                {
                    Log.ErrorFormat(Properties.Resources.NUTCH_CONTROLLER_CLIENT_SSH_TIMEOUT_EXCEPTION,
                        this.hostname,
                        toe);
                }
                catch (Exception e)
                {
                    Log.ErrorFormat(Properties.Resources.LOG_COMMON_ERROR, e);
                }
                finally
                {
                    client.Disconnect();
                }
            }
        }
    }
}
