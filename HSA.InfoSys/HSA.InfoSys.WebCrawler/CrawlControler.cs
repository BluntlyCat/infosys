namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.Logging;
    using log4net;

    public class CrawlControler : ICrawlControler
    {
        ILog log = Logging.GetLogger("CrawlControler");

        public string StartSearch()
        {
            log.Info("New search started...");
            return "New search started...";
        }
    }
}
