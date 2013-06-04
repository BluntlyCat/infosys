

namespace HSA.InfoSys.Testing.NutchTesting
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.Common.Logging;
    using log4net;

    class NutchUrlManager
    {

        ILog log = Logger<string>.GetLogger("NutchTesting");

        private string  NutchSeedUrlPath;

        private NutchRegexFilerManager NutchRegexFilerManager;

        private List<string> Urls;

        /// <summary>
        /// Initializes a new instance of the <see cref="NutchUrlManager"/> class.
        /// </summary>
        /// <param name="Path_RegexFile">The path_ regex file.</param>
        /// <param name="Path_UrlSeedFile">The path_ URL seed file.</param>
        public NutchUrlManager(string Path_RegexFile, string Path_UrlSeedFile)
        {
            Urls = new List<string>();
            ValidatePath(Path_UrlSeedFile);
            NutchSeedUrlPath = Path_UrlSeedFile;
            NutchRegexFilerManager = new NutchRegexFilerManager(Path_RegexFile);
            using (StreamReader sr = new StreamReader(NutchSeedUrlPath))
            {
                string line;
                while((line = sr.ReadLine())!= null)
                {
                    Urls.Add(line);   
                }
            }
        }



        /// <summary>
        /// Adds the URL.
        /// </summary>
        /// <param name="Url">The URL.</param>
        public void AddUrl(string Url)
        {
            if (!Urls.Contains(Url))
            {
                using (StreamWriter sw = File.AppendText(NutchSeedUrlPath))
                {
                    sw.WriteLine(Url);
                    log.Info(string.Format("Url Added: {0}", Url));
                }
                NutchRegexFilerManager.AddUrl(Url);
            }
        }



        /// <summary>
        /// Validates the path.
        /// </summary>
        /// <param name="Path">The path.</param>
        private void ValidatePath(string Path)
        {
            if (!File.Exists(Path))
            {
                log.Error(string.Format("Your Path is not correctly set! Actual Path: {0}", Path));
                throw new FileNotFoundException();
            }
        }


        /// <summary>
        /// Gets the URLS.
        /// </summary>
        /// <returns></returns>
        public List<string> GetUrls()
        {
            return Urls;
        }

    }
}
