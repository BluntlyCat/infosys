// ------------------------------------------------------------------------
// <copyright file="NutchManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Nutch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    /// <summary>
    /// The Nutch Manager handles the WebCrawl
    /// </summary>
    public class NutchManager
    {
        /// <summary>
        /// The logger for nutch manager.
        /// </summary>
        private ILog log = Logger<string>.GetLogger("NutchTesting");

        /// <summary>
        /// The path for the regex url filter.
        /// </summary>
        private string regexPath;

        /// <summary>
        /// The path for the url.
        /// </summary>
        private string urlPath;

        /// <summary>
        /// The name of the file in which the url are written.
        /// </summary>
        private string fileName = "seed.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchManager"/> class.
        /// </summary>
        public NutchManager()
        {
            this.urlPath = "C:/Users/A/Desktop/urls";
            this.regexPath = "C:/Users/A/Desktop/conf/regex-urlfilter.txt";
        }

        /// <summary>
        /// Adds a the url in the user url file.
        /// </summary>
        /// <param name="urls">The list of the url.</param>
        /// <param name="user">The user name.</param>
        public void AddURL(List<string> urls, string user)
        {
            string userURLPath = string.Format("{0}/{1}/{2}", this.urlPath, user, this.fileName);

            List<string> prefixUrls = new List<string>();

            foreach (string url in urls)
            {
                string prefix = string.Format("{0}{1}", Properties.Settings.Default.PREFIX, url);

                if (!this.GetFileContent(Properties.Settings.Default.PREFIX, this.regexPath).Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                }
            }

            this.AddURLToFile(prefixUrls, this.regexPath);
            this.AddURLToFile(urls, userURLPath);
        }

        /// <summary>
        /// Start the web crawl.
        /// </summary>
        /// <param name="urlDir">The path of the url directory.</param>
        /// <param name="depth">The depth of the crawl.</param>
        /// <param name="topN">The top of the crawl.</param>
        public void StartCrawl(string urlDir, int depth, int topN)
        {
            string crawlRequest = string.Format("crawl {0} -solr {1} -depth {2} -topN {3}", urlDir, Properties.Settings.Default.SOLRSERVER, depth, topN);
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = "nutch";
            process.Arguments = crawlRequest;
            this.log.Info(string.Format("Crawl request was send: {0}", crawlRequest));
            Process.Start(process);
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

        /// <summary>
        /// Creates a new url directory for an user.
        /// </summary>
        /// <param name="user">The user.</param>
        private void MkUserDir(string user)
        {
            string newDirectory = string.Format("{0}/{1}", this.urlPath, user);
            Directory.CreateDirectory(newDirectory);
            StreamWriter myWriter = File.CreateText(string.Format("{0}/{1}", newDirectory, this.fileName));
            myWriter.Close();
        }

        /// <summary>
        /// Adds all the url in a list which are in the regex file.
        /// </summary>
        /// <param name="pattern">The prefix pattern.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns a List with the already existing url.</returns>
        private List<string> GetFileContent(string pattern, string filePath)
        {
            List<string> content = new List<string>();
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;

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
