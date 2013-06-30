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
            this.PrintStartupMessage();

            //// Initialize the database.
            var dbManager = InitializeDataBase();

            //// Initialize the controller host.
            this.InitializeControllerHost(dbManager);

            //// Create CrawlerService.
            this.controllerHost.OpenWCFHost<CrawlerService, ICrawlerService>(this);

            //// Initialize other services
            this.InitializeOtherServices(dbManager);
        }

        /// <summary>
        /// Gets the CrawlerService. Create one if none exist.
        /// </summary>
        /// <returns>The CrawlerService</returns>
        public static CrawlerService CrawlerFactory
        {
            get
            {
                if (crawlerService == null)
                {
                    Log.Debug(Properties.Resources.WEB_CRAWLER_NO_SERVICE_FOUND);
                    crawlerService = new CrawlerService(Guid.NewGuid());
                }

                return crawlerService;
            }
        }

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            try
            {
                CrawlerService crawler = CrawlerService.CrawlerFactory;
                crawler.StartService();
            }
            catch (Exception e)
            {
                Log.FatalFormat(Properties.Resources.CRAWLER_SERVICE_FATAL_ERROR, e);
            }
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
                this.StopService();
            }
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        protected override void Run()
        {
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
        /// Initializes the data base.
        /// </summary>
        /// <returns>The database manager.</returns>
        private static DBManager InitializeDataBase()
        {
            DBManager dbManager = null;

            try
            {
                //// Create DBManager.
                dbManager = DBManager.ManagerFactory as DBManager;
                dbManager.StartService();
            }
            catch (Exception e)
            {
                throw e;
            }

            return dbManager;
        }

        /// <summary>
        /// Prints the startup message.
        /// </summary>
        private void PrintStartupMessage()
        {
            Log.Debug(Properties.Resources.WEB_CRAWLER_START_SERVER);
            Log.Info(Properties.Resources.WEB_CRAWLER_QUIT_MESSAGE);
        }

        /// <summary>
        /// Initializes the controller host.
        /// </summary>
        /// <param name="dbManager">The database manager.</param>
        private void InitializeControllerHost(DBManager dbManager)
        {
            try
            {
                var settings = dbManager.GetWCFSettings();

                //// Create WCFControllerHost.
                this.controllerHost = new WCFControllerHost(settings);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Initializes the controller host.
        /// </summary>
        /// <param name="dbManager">The database manager.</param>
        private void InitializeOtherServices(DBManager dbManager)
        {
            try
            {
                //// Open DBManagers WCF Service.
                this.controllerHost.OpenWCFHost<DBManager, IDBManager>(dbManager);

                //// Create CrawlController.
                this.crawlController = this.controllerHost.OpenWCFHost<CrawlController, ICrawlController>(
                    CrawlController.ControllerFactory);

                //// Create NutchController.
                var nutchController = NutchController.NutchFactory(dbManager);

                //// Create SolrController.
                var solrController = this.controllerHost.OpenWCFHost<SolrController, ISolrController>(
                    SolrController.SolrFactory(dbManager));

                //// Create Scheduler.
                var scheduler = this.controllerHost.OpenWCFHost<Scheduler, IScheduler>(Scheduler.SchedulerFactory(dbManager));
            
                //// Register services.
                this.crawlController.RegisterService(typeof(Scheduler), scheduler);
                this.crawlController.RegisterService(typeof(SolrController), solrController);
#warning enable nutchController before building on mono, must be disabled to avoid starting many crawls on the hosts while developing.
                //// this.crawlController.RegisterService(typeof(NutchController), nutchController);
                this.crawlController.RegisterService(typeof(DBManager), dbManager);

                this.servicesRunning = this.crawlController.StartServices();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
