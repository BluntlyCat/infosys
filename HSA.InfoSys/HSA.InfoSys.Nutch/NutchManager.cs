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
            this.homeDir = Properties.Settings.Default.USER_DIR_DOTNET;
#else
            this.homeDir = Properties.Settings.Default.USER_DIR_MONO;
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

#warning Only for testing, remove the !MONO part when finished but keep the else part!!! Also remove the reference to RenciSSH
#if !MONO
        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        public void StartCrawl(string userName, int depth, int topN)
        {
            this.homeDir = Properties.Settings.Default.USER_DIR_MONO;

            var keyFile = new FileStream("Certificates/devteam.id.rsa", FileMode.Open);
            byte[] keyBytes = new byte[keyFile.Length];
            keyFile.Read(keyBytes, 0, (int)keyFile.Length);
            keyFile.Close();

            PrivateKeyFile key = new PrivateKeyFile(new MemoryStream(keyBytes));
            PrivateKeyConnectionInfo info = new PrivateKeyConnectionInfo("infosys.informatik.hs-augsburg.de", 22, "devteam", key);
            SshClient client = new SshClient(info);

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

            client.Connect();

            var command = client.CreateCommand(string.Format(
                "{0} && {1}",
                "export JAVA_HOME='/usr/lib/jvm/java-6-openjdk'",
                crawlRequest));

            command.BeginExecute(
                x =>
                {
                    if (x.IsCompleted)
                    {
                        Log.DebugFormat("Result from Nutch: {0}", command.Result);
                        client.Disconnect();
                    }
                });

            Log.Info(string.Format(Properties.Resources.CRAWL_REQUEST_SENT, crawlRequest));
        }
#else
        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        public void StartCrawl(string userName, int depth, int topN)
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

            nutch.StartInfo.FileName = Properties.Settings.Default.NUTCH_COMMAND;
            nutch.StartInfo.Arguments = crawlRequest;

            nutch.Start();

            Log.Info(string.Format(Properties.Resources.CRAWL_REQUEST_SENT, crawlRequest));
        }
#endif

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="urls">The URLs.</param>
        /// <param name="user">The username.</param>
        public void AddURL(List<string> urls, string user)
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

            this.AddURLToFile(prefixUrls, this.prefixPath);
            this.AddURLToFile(urls, userURLPath);
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

        /// <summary>
        /// Adds the url in the corresponding file.
        /// </summary>
        /// <param name="urls">The list of url.</param>
        /// <param name="path">The path of the file.</param>
        private void AddURLToFile(List<string> urls, string path)
        {
            foreach (string url in urls)
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(url);
                }
            }
        }
    }
}
