// ------------------------------------------------------------------------
// <copyright file="CrawlerService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.CrawlerService
{
    using System;
    using System.Threading;
    using HSA.InfoSys.Common.CrawlController;
    using HSA.InfoSys.Common.DBManager;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.SolrClient;
    using HSA.InfoSys.Scheduling;
    using log4net;

    /// <summary>
    /// The WebCrawler searches the internet
    /// for security issues of several hardware
    /// </summary>
    public class CrawlerService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlerService");

        /// <summary>
        /// The running flag for this server.
        /// </summary>
        private bool running;

        /// <summary>
        /// The WCF controller for the crawler service.
        /// </summary>
        private CrawlControllerHost controllerHost;

        /// <summary>
        /// The controller for the crawler service.
        /// </summary>
        private CrawlController controller;

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            CrawlerService crawler = new CrawlerService();
            crawler.RunServer();
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        private void RunServer()
        {
            Log.Debug(Properties.Resources.WEB_CRAWLER_START_SERVER);
            Log.Info(Properties.Resources.WEB_CRAWLER_QUIT_MESSAGE);

            this.InitializeControllerHost();
            this.InitializeController();

            this.running = true;

            while (this.running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    Log.InfoFormat(Properties.Resources.WEB_CRAWLER_GOT_USERINPUT, keyInfo.Key);

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        Log.Info(Properties.Resources.WEB_CRAWLER_EXITED_BY_USER);
                        this.ShutdownCrawler();
                    }
                    else
                    {
                        Log.Info(Properties.Resources.WEB_CRAWLER_UNKOWN_USERINPUT);
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Initializes the controller host.
        /// </summary>
        private void InitializeControllerHost()
        {
            Addresses.Initialize();
            this.controllerHost = new CrawlControllerHost();

            this.controllerHost.OpenWCFHost<DBManager, IDBManager>();
            this.controllerHost.OpenWCFHost<CrawlController, ICrawlController>();
            this.controllerHost.OpenWCFHost<SolrController, ISolrController>();
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        private void InitializeController()
        {
            this.controller = new CrawlController();
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        private void RegisterServices()
        {
            this.controller.RegisterService(new Scheduler());
        }

        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        private void ShutdownCrawler()
        {
            if (this.controllerHost != null)
            {
                this.controllerHost.CloseWCFHosts();

                this.running = false;
            }
        }
    }
}
