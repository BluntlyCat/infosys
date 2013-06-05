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

    /// <summary>
    /// The Nutch Manager handles the WebCrawl
    /// </summary>
    public class NutchManager
    {
        /// <summary>
        /// The logger for NutchManager.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("NutchManager");

        /// <summary>
        /// The logger for nutch manager.
        /// </summary>
        private ILog log = Logger<string>.GetLogger("NutchTesting");

        /// <summary>
        /// The path to prefix file.
        /// </summary>
        private string prefixPath;

        /// <summary>
        /// The path for the regex url filter.
        /// </summary>
        private string regexPath;

        /// <summary>
        /// The path to URL file.
        /// </summary>
        private string urlPath;

        /// <summary>
        /// The seed file name.
        /// </summary>
        private string fileName = Properties.Settings.Default.SEED_FILENAME;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchManager"/> class.
        /// </summary>
        public NutchManager()
        {
#if !MONO
            this.urlPath = Properties.Settings.Default.URL_PATH_DOTNET;
            this.prefixPath = Properties.Settings.Default.PREFIX_PATH_DOTNET;
#else
            this.URLPath = Properties.Settings.Default.URL_PATH_MONO;
            this.PrefixPath = Properties.Settings.Default.PREFIX_PATH_MONO;
#endif
        }

        /// <summary>
        /// Starts the crawling.
        /// </summary>
        /// <param name="urlDir">The URL directory.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="topN">The top N.</param>
        public void StartCrawl(string urlDir, int depth, int topN)
        {
            Process nutch = new Process();

            string crawlRequest =
                string.Format(
                Properties.Settings.Default.NUTCH_CRAWL_REQUEST,
                urlDir,
                Properties.Settings.Default.SOLRSERVER,
                depth,
                topN);

            nutch.StartInfo.FileName = Properties.Settings.Default.NUTCH_COMMAND;
            nutch.StartInfo.Arguments = crawlRequest;

            nutch.Start();

            Log.Info(string.Format(Properties.Resources.CRAWL_REQUEST_SENT, crawlRequest));
        }

        /// <summary>
        /// Creates the user directory.
        /// </summary>
        /// <param name="user">The username.</param>
        public void CreateUserDir(string user)
        {
            string newDirectory = string.Format(Properties.Settings.Default.USER_DIR, this.urlPath, user);
            Directory.CreateDirectory(newDirectory);
            StreamWriter writer = File.CreateText(string.Format(Properties.Settings.Default.USER_DIR, newDirectory, this.fileName));

            writer.Close();
        }

        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="urls">The URLs.</param>
        /// <param name="user">The username.</param>
        public void AddURL(List<string> urls, string user)
        {
            string userURLPath = string.Format(Properties.Settings.Default.USER_URL_PATH, this.urlPath, user, this.fileName);

            List<string> prefixUrls = new List<string>();

            foreach (string url in urls)
            {
                string prefix = string.Format(Properties.Settings.Default.USER_DIR, Properties.Settings.Default.PREFIX, url);

                if (!this.GetFileContent(Properties.Settings.Default.PREFIX, this.prefixPath).Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                }
            }

            this.AddURLToFile(prefixUrls, this.prefixPath);
            this.AddURLToFile(urls, userURLPath);
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
