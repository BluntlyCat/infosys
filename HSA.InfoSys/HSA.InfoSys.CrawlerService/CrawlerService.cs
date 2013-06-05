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
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services;
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
        /// The crawl controller.
        /// </summary>
        private CrawlController crawlController;

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
            this.crawlController.StartServices();

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

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Initializes the controller host.
        /// </summary>
        private void InitializeControllerHost()
        {
            Addresses.Initialize();
            this.controllerHost = new CrawlControllerHost();

            this.crawlController = this.controllerHost.OpenWCFHost<CrawlController, ICrawlController>(new CrawlController());

            var solrController = this.controllerHost.OpenWCFHost<SolrController, ISolrController>(new SolrController());
            this.RegisterServices(solrController);

            var dbManager = this.controllerHost.OpenWCFHost<DBManager, IDBManager>(DBManager.Manager as DBManager);
            this.RegisterServices(dbManager);

            var scheduler = this.controllerHost.OpenWCFHost<Scheduler, IScheduler>(new Scheduler());
            this.RegisterServices(scheduler);
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="service">The service.</param>
        private void RegisterServices(Service service)
        {
            this.crawlController.RegisterService(service);
        }

        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        private void ShutdownCrawler()
        {
            if (this.controllerHost != null)
            {
                this.crawlController.StopServices(true);
                this.controllerHost.CloseWCFHosts();

                this.running = false;
            }
        }
    }
}
