// ------------------------------------------------------------------------
// <copyright file="CrawlerService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.CrawlerService
{
    using System;
    using System.ServiceModel;
    using System.Threading;
    using HSA.InfoSys.Common.Exceptions;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services;
    using HSA.InfoSys.Common.Services.LocalServices;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;

    /// <summary>
    /// The WebCrawler searches the internet
    /// for security issues of several hardware
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CrawlerService : Service, ICrawlerService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("CrawlerService");

        /// <summary>
        /// The crawler service.
        /// </summary>
        private static CrawlerService crawlerService;

        /// <summary>
        /// The services running flag.
        /// </summary>
        private bool servicesRunning = false;

        /// <summary>
        /// The WCF controller for the crawler service.
        /// </summary>
        private WCFControllerHost controllerHost;

        /// <summary>
        /// The crawl controller.
        /// </summary>
        private CrawlController crawlController;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrawlerService"/> class.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        private CrawlerService(Guid serviceGUID) : base(serviceGUID)
        {
        }

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            CrawlerService crawler = CrawlerService.CrawlerFactory(Guid.NewGuid());
            crawler.StartService();
        }

        /// <summary>
        /// Gets the CrawlerService. Create one if none exist.
        /// </summary>
        /// <param name="serviceGUID">The service GUID.</param>
        /// <returns>The CrawlerService</returns>
        public static CrawlerService CrawlerFactory(Guid serviceGUID)
        {
            if (crawlerService == null)
            {
                Log.Debug(Properties.Resources.WEB_CRAWLER_NO_SERVICE_FOUND);
                crawlerService = new CrawlerService(serviceGUID);
            }

            return crawlerService;
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        public void StopServices()
        {
            if (this.servicesRunning)
            {
                this.servicesRunning = this.crawlController.StopServices(true);
            }
            else
            {
                this.servicesRunning = this.crawlController.StartServices();
            }
        }

        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        public void ShutdownCrawler()
        {
            if (this.controllerHost != null)
            {
                this.crawlController.StopServices(true);
                this.controllerHost.CloseWCFHosts();
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
            Log.Debug(Properties.Resources.WEB_CRAWLER_START_SERVER);
            Log.Info(Properties.Resources.WEB_CRAWLER_QUIT_MESSAGE);

            this.InitializeControllerHost();
            this.servicesRunning = this.crawlController.StartServices();

            while (this.Running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    Log.InfoFormat(Properties.Resources.WEB_CRAWLER_GOT_USERINPUT, keyInfo.Key);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.Q:
                            Log.Info(Properties.Resources.WEB_CRAWLER_EXITED_BY_USER);
                            this.ShutdownCrawler();
                            break;

                        case ConsoleKey.S:
                            this.StopServices();
                            break;

                        default:
                            Log.Info(Properties.Resources.WEB_CRAWLER_UNKOWN_USERINPUT);
                            break;
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
            try
            {
                WCFControllerAddresses.Initialize();

                //// Create DBManager.
                var dbManager = DBManager.ManagerFactory(Guid.NewGuid()) as DBManager;

                //// Create WCFControllerHost.
                this.controllerHost = new WCFControllerHost(dbManager);

                //// Open DBManagers WCF Service.
                this.controllerHost.OpenWCFHost<DBManager, IDBManager>(dbManager);

                //// Create CrawlController.
                this.crawlController = this.controllerHost.OpenWCFHost<CrawlController, ICrawlController>(
                    CrawlController.ControllerFactory(Guid.NewGuid()));

                //// Create NutchController.
                var nutchController = NutchController.NutchFactory(Guid.NewGuid(), dbManager);

                //// Create SolrController.
                var solrController = this.controllerHost.OpenWCFHost<SolrController, ISolrController>(
                    SolrController.SolrFactory(Guid.NewGuid(), dbManager));

                //// Create Scheduler.
                var scheduler = this.controllerHost.OpenWCFHost<Scheduler, IScheduler>(
                    Scheduler.SchedulerFactory(Guid.NewGuid(), dbManager));

                //// Create CrawlerService.
                this.controllerHost.OpenWCFHost<CrawlerService, ICrawlerService>(this);

                //// Register services.
                this.crawlController.RegisterService(scheduler);
                this.crawlController.RegisterService(solrController);
                this.crawlController.RegisterService(nutchController);
                this.crawlController.RegisterService(dbManager);
                this.crawlController.RegisterService(this);
            }
            catch (OpenWCFHostException ohe)
            {
                Log.FatalFormat(Properties.Resources.WEB_CRAWLER_CAN_NOT_OPEN_WCF_HOST, ohe);
                this.StopService(true);
            }
        }
    }
}
