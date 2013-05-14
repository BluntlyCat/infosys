namespace HSA.InfoSys.SolrTesting
{
    using System;
    using System.Threading;
    using HSA.InfoSys.SolrClient;
    using HSA.InfoSys.Logging;
    using log4net;

    public class SolrTesting
    {
        static void Main(string[] args)
        {
            ILog log = Logging.GetLogger("SolrTesting");

            SolrClient client = new SolrClient(Properties.Settings.Default.SOLR_PORT, Properties.Settings.Default.SOLR_HOST);
            bool running = true;

            Console.WriteLine("");
            Console.WriteLine("Here you can test the Solr feautures.");
            Console.WriteLine("To see your options press h or press q for quit.");
            Console.WriteLine("");

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    log.InfoFormat("Key [{0}] was pressed.", keyInfo.Key);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.H:
                            log.Info("Print help text.");
                            PrintHelp();
                            break;

                        case ConsoleKey.Q:
                            log.Info("Quit application.");
                            running = false;
                            break;

                        case ConsoleKey.S:
                            log.Info("Send request to solr.");
                            client.StartSearch("solr");
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
