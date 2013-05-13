namespace HSA.InfoSys.DBTesting
{
    using System;
    using System.Threading;
    using HSA.InfoSys.DBManager;
    using HSA.InfoSys.DBManager.Data;
    using HSA.InfoSys.Logging;
    using log4net;

    public class DBTesting
    {
        static void Main(string[] args)
        {
            ILog log = Logging.GetLogger("WCFTesting");

            IDBManager dbManager = DBManager.GetDBManager();

            bool running = true;

            Console.WriteLine("");
            Console.WriteLine("Here you can test the DB Nhibernate feautures to the DB Server.");
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
                            log.Info("Send request to database.");

                            string s = "99cee797-3ec0-458c-a016-a1bd0001bf34";
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
