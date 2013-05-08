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
       private enum solrRequestHandler { 
        
               select     
        }

        
        string collection = "collection1";

        private static readonly ILog Log = Logging.Logging.GetLogger("SolrClient");

        private Socket solrSocket;
        private string ipAddress;
        private int port;
        private bool running;
        private List<byte[]> messages = new List<byte[]>();
        private List<string> responses = new List<string>();


        //Constructor

        public SolrClient(int port, string ipAddress)
        {
            this.port = port;
            this.ipAddress = ipAddress;
            solrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        //public method which will be used to make a commit to Solr

        public void commitToSolr( )
        {
            string request = "";
            sendMessageToServer(stringToByteArray(request));
        }



        public string solrQuerry (string queryString,
          //  string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            string wt)
        {
            
            string query = "/solr/"+collection+"/"+solrRequestHandler.select+"?q="+queryString+"&wt="+wt;
            sendMessageToServer(stringToByteArray(query));
            Log.Info("Query was send to " + ipAddress + " " + query);
            Console.WriteLine(query);

            string response = "";



            return response;
        }


        private  void sendMessageToServer(byte[] message)
        {
            messages.Add(message);
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


        public void closeConnection()
        {
            running = false;
        }



        // Method to tranform string into byte array
        private byte[] stringToByteArray(string message)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(message);
        }

        public void threadRoutine()
        {
            while (running && solrSocket.Connected)
            {

                if (messages.Capacity == 0)
                {
                    continue;
                }
                else
                {
                    byte[] message = messages[0];
                    solrSocket.Send(message);
                    messages.Remove(message);





                }
 

               
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
