using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HSA.InfoSys.Logging;
using log4net;

namespace HSA.InfoSys.WebCrawler
{
    class SolrClient
    {
        private static readonly ILog Log = Logging.Logging.GetLogger("SolrClient");
        private Socket solrSocket;
        private string ipAddress;
        private int port;
        private bool running; 

        //Constructor

        public SolrClient(int port, string ipAddress)
        {
            this.port = port;
            this.ipAddress = ipAddress;

            solrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
           

        }

        public void connect() {
            try
            {
                IPAddress ipa = IPAddress.Parse(ipAddress);
                IPEndPoint ipe = new IPEndPoint(ipa, port);
                solrSocket.Connect(ipe);
                Log.Info("Connection Established: "+ ipa);
                new Thread(new ThreadStart(threadRoutine)).Start(); running = true;
            }
            catch (SocketException e)
            {
                Log.Error("Unable to Connect\n" +e.Message);

            }
        }


        public void threadRoutine()
        {
            while (running && solrSocket.Connected)
            {

                //Warte auf anfrage vom CrawlerCore: Commit, Update, Select. etc.
                // sende an Server, Warte auf Responde, Schicken von Response an CrawlerCore
            }
            Log.Info("Connection shutdown");
            if (solrSocket.Connected)
            {
                solrSocket.Close();
                Log.Info("Socket to: "+ipAddress+" closed!");
            }
        }
    }
}
