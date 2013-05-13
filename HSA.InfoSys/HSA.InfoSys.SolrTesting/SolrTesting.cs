namespace HSA.InfoSys.SolrTesting
{
    using System;
    using System.Threading;
    using HSA.InfoSys.WebCrawler;

    public class SolrTesting
    {
        static void Main(string[] args)
        {
            SolrClient client = new SolrClient(8983, "141.82.59.139");
            bool running = true;

            Console.WriteLine("Here you can test the Solr feautures.");
            Console.WriteLine("To see your options press h or press q for quit.");

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    switch (keyInfo.Key)
                    {
                        case ConsoleKey.H:
                            PrintHelp();
                            break;

                        case ConsoleKey.Q:
                            running = false;
                            break;

                        case ConsoleKey.S:
                            client.SolrQuery("solr", SolrOutputMimeType.xml);
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
