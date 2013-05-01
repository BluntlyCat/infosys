namespace HSA.InfoSys.WebCrawler
{
    
    using System;
    using System.Net;
    using System.Threading;
    using HSA.InfoSys.Logging;
    using HSA.InfoSys.DBManager;
    using log4net;

    /// <summary>
    /// The WebCrawler searches the internet
    /// for security issues of several hardware
    /// </summary>
    public class WebCrawler
    {
        private static readonly ILog log = Logging.GetLogger("WebCrawler");
        // returns  the urls website - content 



        static void Main(string[] args)
        {
            bool running = true;

            log.Debug("Starting server...");
            log.Info("Press q for quit.");

            DBManager dbm = new DBManager();

            while(running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    log.InfoFormat("Got user input key {0}", keyInfo.Key);

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        log.Info("User exited the application.");
                        running = false;
                    }
                    else
                    {
                        log.Info("Unkown user input.");
                    }
                }

                Thread.Sleep(100);
            }
        }
    }
}
