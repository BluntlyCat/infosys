namespace HSA.InfoSys.DBTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HSA.InfoSys.DBManager.Data;
    using HSA.InfoSys.DBManager;
    using System.Threading;

    public class DBTesting
    {
        static void Main(string[] args)
        {
            IDBManager dbManager = DBManager.GetDBManager();
            string s = "99cee797-3ec0-458c-a016-a1bd0001bf34";

            bool running = true;

            Console.WriteLine("Here you can test the WCF feautures to the WebCrawler.");
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
                            var newComp = dbManager.CreateComponent("Windows8", "TestWin");
                            dbManager.AddNewObject(newComp);
                            var existingComp = dbManager.GetEntity<Component>(new Guid(s));
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
            Console.WriteLine("Press s to start a new request to db server.");

            Console.WriteLine("");
        }
    }
}
