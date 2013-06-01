
namespace HSA.InfoSys.Testing.NutchTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    class RegexManager
    {

        /// <summary>
        /// The path to your nutch - regex - urlfilter file
        /// </summary>
        private const string Path = "C:/Users/A/Dropbox/Semester 6/Projekt/Tortoise/conf/regex-urlfilter.txt";
        private const string UrlPrefix = "+^http://([a-z0-9]*\\.)*";

        public RegexManager()
        {
         if(!File.Exists(Path)){
             throw new FileNotFoundException();
            }
           
        }
        


        /// <summary>
        /// Adds the URL to the filter file.
        /// </summary>
        /// <param name="NewUrl">The new URL.</param>
        public void AddUrl(string NewUrl)
        {
            string Regex = string.Format("{0}{1}",UrlPrefix,NewUrl);
            StreamWriter sw = File.AppendText(Path);
            sw.WriteLine(Regex);
            sw.Close();
        }

    }
}
