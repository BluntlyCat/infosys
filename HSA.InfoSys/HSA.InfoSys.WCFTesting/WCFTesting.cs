namespace HSA.InfoSys.WCFTesting
{
    using System;
    using System.Threading;

    public class WCFTesting
    {
        static void Main(string[] args)
        {
            CrawlControllerClient client = new CrawlControllerClient();

            bool running = true;

            Console.WriteLine("Here you can test the WCF feautures to the WebCrawler.");
            Console.WriteLine("To see your options press h or press q for quit.");

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    switch(keyInfo.Key)
                    {
                        case ConsoleKey.H:
                            PrintHelp();
                            break;

                        case ConsoleKey.Q:
                            running = false;
                            break;

                        case ConsoleKey.S:
                            client.StartSearch();
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
