namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.DBManager;
    using HSA.InfoSys.Logging;
    using log4net;

    /// <summary>
    /// This class is the controller for the crawler
    /// it implements an interface for communication
    /// between the crawler and the gui by using wcf.
    /// </summary>
    public class CrawlController : ICrawlController
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static ILog log = Logging.GetLogger("CrawlController");

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// The db manager.
        /// </summary>
        private IDBManager dbManager;

        /// <summary>
        /// Tests this instance.
        /// </summary>
#warning //only for testing remove me when finished
        public void Test()
        {
            // Beispiel
            string s = "29e16064-c283-4e63-9f69-a1b400b2ab54";

            //var newComp = dbManager.CreateComponent("Windows8", "TestWin");
            //var existingComp = dbManager.GetComponent(new Guid(s));
        }

        /// <summary>
        /// Opens the WCF host.
        /// </summary>
        public void OpenWCFHost()
        {
            log.Info("WCF service host opened...");
            host.Open();
        }

        /// <summary>
        /// Closes the WCF host.
        /// </summary>
        public void CloseWCFHost()
        {
            log.Info("WCF service host closed...");
            host.Close();
        }

        /// <summary>
        /// Starts a new search.
        /// </summary>
        public void StartSearch()
        {
            log.Info("Search started from GUI");
        }

        /// <summary>
        /// Starts the services.
        /// </summary>
        public void StartServices()
        {
            host = new ServiceHost(typeof(CrawlController));
            dbManager = DBManager.GetDBManager();
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        public void StopServices()
        {
            log.Info("Shutdown Services");
        }
    }
}
