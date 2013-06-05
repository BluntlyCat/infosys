// ------------------------------------------------------------------------
// <copyright file="NutchManager.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------

namespace HSA.InfoSys.Common.Nutch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    /// <summary>
    /// Manages the Nutch filesystem
    /// </summary>
    public class NutchManager
    {

        private string PrefixPath;

        private string URLPath;

        private string FileName = "seed.txt";

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchManager"/> class.
        /// </summary>
        public NutchManager()
        {
            this.URLPath = "C:/Users/A/Desktop/urls";
            this.PrefixPath = "C:/Users/A/Desktop/conf/regex-urlfilter.txt";
        }



        /// <summary>
        /// Makes the user dir.
        /// </summary>
        /// <param name="User">The user.</param>
        public void MkUserDir(string user)
        {
            string NewDirectory = string.Format("{0}/{1}", this.URLPath, user);
            Directory.CreateDirectory(NewDirectory);
            StreamWriter myWriter = File.CreateText(string.Format("{0}/{1}",NewDirectory, this.FileName));
            myWriter.Close();
        }


        /// <summary>
        /// Gets the content of the file.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        public List<string> GetFileContent(string pattern, string filePath)
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



        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="Prefix">if set to <c>true</c> [prefix].</param>
        /// <param name="URLs">The UR ls.</param>
        /// <param name="User">The user.</param>
        public void AddURL(List<string> urls, string user)
        {
            string userURLPath = string.Format("{0}/{1}/{2}", this.URLPath, user, this.FileName);

            List<string> prefixUrls = new List<string>();

            foreach (string url in urls)
            {
                string prefix = string.Format("{0}{1}", Properties.Settings.Default.PREFIX, url);

                if(!GetFileContent(Properties.Settings.Default.PREFIX, this.PrefixPath).Contains(prefix))
                {
                    prefixUrls.Add(prefix);
                }
            }
            AddURLToFile(prefixUrls, this.PrefixPath);
            AddURLToFile(urls, userURLPath);
        }




        /// <summary>
        /// Adds the URL to file.
        /// </summary>
        /// <param name="prefixUrls">The prefix urls.</param>
        /// <param name="urls">The urls.</param>
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
