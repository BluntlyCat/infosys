namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using log4net;
    using HSA.InfoSys.Logging;

    public class CrawlControler : ICrawlControler
    {
        private static ILog log = Logging.GetLogger("CrawlControler");

        private WebCrawler crawler;

        public CrawlControler()
        {

        }

        public CrawlControler(WebCrawler crawler)
        {
            this.crawler = crawler;
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        /// <returns></returns>
        public string StartSearch()
        {
            log.Info("Search started from GUI");
            return "Hello";
        }

        /// <summary>
        /// Shuts down web crawler.
        /// </summary>
        /// <returns>true on success.</returns>
        public bool ShutdownServices()
        {
            log.Info("Shutdown Services");
            return true;
        }
    }
}
