namespace HSA.InfoSys.WCFTesting
{
    using System;
    using System.Threading;
    using log4net;
    using HSA.InfoSys.Logging;

    public class WCFTesting
    {
        static void Main(string[] args)
        {
            ILog log = Logging.GetLogger("WCFTesting");

            CrawlControllerClient client = new CrawlControllerClient();

            bool running = true;

            bool requestSent = false;

            string response = string.Empty;

            Console.WriteLine("");
            Console.WriteLine("Here you can test the WCF feautures to the WebCrawler.");
            Console.WriteLine("To see your options press h or press q for quit.");
            Console.WriteLine("");

            client.Open();

            while (running)
            {
                if (!string.IsNullOrEmpty(response) && requestSent)
                {
                    log.InfoFormat("Got response from solr: [{0}]", response);

                    requestSent = false;
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    log.InfoFormat("Key [{0}] was pressed.", keyInfo.Key);

                    switch(keyInfo.Key)
                    {
                        case ConsoleKey.A:
                            log.Info("Add new Component.");

                            DBManagerClient dbClient = new DBManagerClient();
                            var comp = dbClient.CreateComponent("Michis Special Component", "Funny Stuff");

                            dbClient.AddEntity(comp);
                            break;

                        case ConsoleKey.H:
                            log.Info("Print help text.");
                            PrintHelp();
                            break;

                        case ConsoleKey.Q:
                            log.Info("Quit application.");
                            running = false;
                            client.Close();
                            break;

                        case ConsoleKey.S:
                            log.Info("Send request to host.");

                            if(client.State == System.ServiceModel.CommunicationState.Opened)
                                try
                                {
                                    client.StartSearch("solr");
                                    
                                }
                                catch
                                {
                                    log.Error("Unable to communicate with host.");
                                }
                            break;
                    }
                }

                Thread.Sleep(500);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Press h to see this help text.");
            Console.WriteLine("Press q to quit this application.");
            Console.WriteLine("Press s to start a new search request to solr server.");

            Console.WriteLine("");
        }
    }
}
