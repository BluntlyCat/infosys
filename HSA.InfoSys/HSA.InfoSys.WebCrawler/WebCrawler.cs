namespace HSA.InfoSys.WebCrawler
{
    
    using System;
    using System.Net;
    using System.Threading;
    using HSA.InfoSys.Logging;
    using HSA.InfoSys.DBManager;
    using System.Linq;
    using System.Text;
    using log4net;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using NHibernate;
    using HSA.InfoSys.DBManager.Data;
  
   
    

    /// <summary>
    /// The WebCrawler searches the internet
    /// for security issues of several hardware
    /// </summary>
    public class WebCrawler
    {
        private static readonly ILog log = Logging.GetLogger("WebCrawler");

        /// <summary>
        /// The running flag for this server.
        /// </summary>
        private static bool running;

        /// <summary>
        /// The service host for communication between server and gui.
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebCrawler"/> class.
        /// </summary>
        public WebCrawler()
        {
            host = new ServiceHost(typeof(CrawlControler));
        }

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            //SolrClient c = new SolrClient(8983, "141.82.59.139");
            //c.connect();
            //Console.ReadLine();

            WebCrawler crawler = new WebCrawler();
            crawler.RunServer();
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        private void RunServer()
        {
            log.Debug("Starting server...");
            log.Info("Press q for quit.");

            host.Open();
            
            IDBManager dbm = new DBManager();
            //Beispiel
            string s = "29e16064-c283-4e63-9f69-a1b400b2ab54";

            var comp = dbm.getComponent(new Guid(s));
            //var comp = dbm.createComponent("Windows8", "TestWin");
            Console.WriteLine(comp.name.ToString());
           
            running = true;

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    log.InfoFormat("Got user input key {0}", keyInfo.Key);

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        log.Info("User exited the application.");
                        ShutdownCrawler();
                    }
                    else
                    {
                        log.Info("Unkown user input.");
                    }
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        private void ShutdownCrawler()
        {
            if (host != null)
            {
                running = false;
                host.Close();
            }
        }
    }
}
