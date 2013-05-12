// ------------------------------------------------------------------------
// <copyright file="CrawlController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.ServiceModel;
    using HSA.InfoSys.DBManager;
    using HSA.InfoSys.DBManager.Data;
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
#warning Only for testing, remove me when finished.
        public void Test()
        {
            // Beispiel
            string s = "054778a5-b151-465a-9c48-9b4e7e09a8a3";

            //var newComp = this.dbManager.CreateComponent("Windows8", "TestWin");
            //this.dbManager.AddNewObject(newComp);
            //var existingComp = this.dbManager.GetEntity<Component>(new Guid(s));
            SolrClient c = new SolrClient(8983, "141.82");
        }

        /// <summary>
        /// Opens the WCF host.
        /// </summary>
        public void OpenWCFHost()
        {
            log.Info("WCF service host opened...");
            this.host.Open();
        }

        /// <summary>
        /// Closes the WCF host.
        /// </summary>
        public void CloseWCFHost()
        {
            log.Info("WCF service host closed...");
            this.host.Close();
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
            this.host = new ServiceHost(typeof(CrawlController));
            this.dbManager = DBManager.GetDBManager();
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
