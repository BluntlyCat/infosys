// ------------------------------------------------------------------------
// <copyright file="WebCrawler.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using HSA.InfoSys.Logging;
    using log4net;

    /// <summary>
    /// The WebCrawler searches the internet
    /// for security issues of several hardware
    /// </summary>
    public class WebCrawler
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("WebCrawler");

        /// <summary>
        /// The running flag for this server.
        /// </summary>
        private static bool running;

        /// <summary>
        /// The controller for the crawler.
        /// </summary>
        private CrawlController controller;

        /// <summary>
        /// Main function.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            SolrClient c = new SolrClient(8983, "141.82.59.139");
            c.connect();
            int key = c.solrQuery("ipod", SolrOutputMimeType.xml);
            Thread.Sleep(30*1000);
            Console.WriteLine(c.getRespondByKey(key));

           // WebCrawler crawler = new WebCrawler();
           // crawler.RunServer();
        }

        /// <summary>
        /// Runs the server.
        /// </summary>
        private void RunServer()
        {
            Log.Debug(Properties.Resources.WEB_CRAWLER_START_SERVER);
            Log.Info(Properties.Resources.WEB_CRAWLER_QUIT_MESSAGE);

            this.controller = new CrawlController();

            this.controller.StartServices();
            this.controller.OpenWCFHost();

            running = true;

            while (running)
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

                Console.Write(".");

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Shutdown this instance.
        /// </summary>
        private void ShutdownCrawler()
        {
            if (this.controller != null)
            {
                this.controller.CloseWCFHost();
                this.controller.StopServices();

                running = false;
            }
        }
    }
}
