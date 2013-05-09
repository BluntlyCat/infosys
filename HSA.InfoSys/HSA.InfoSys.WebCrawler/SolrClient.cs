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
        
        string collection = "collection1";

        private static readonly ILog Log = Logging.Logging.GetLogger("SolrClient");

        private Socket solrSocket;
        private string ipAddress;
        private int port;
        private bool running;
        private List<string> messegesSend = new List<string>();
        private List<string> messagesReceived = new List<string>();
       


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
            sendMessageToServer(request);
        }


        // Method to send a query to solr
        public string solrQuery (string queryString,
          //  string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            string wt)
        {
            
            string query = "/solr/"+collection+"/select?q="+queryString+"&wt="+wt;
            sendMessageToServer(query);
            Log.Info("Received a request for a solr query: "+ query);
       //     string response = messages.First();
         //   messages.Remove(response);
            return "";
        }


        private  void sendMessageToServer(string message)
        {
            messegesSend.Add(message);
        }

        //Method is establishing server connection
        public void connect() {
            try
            {
                IPAddress ipa = IPAddress.Parse(ipAddress);
                IPEndPoint ipe = new IPEndPoint(ipa, port);
                solrSocket.Connect(ipe);
                if (solrSocket.Connected)
                {
                    Log.Info("Connection Established: " + ipa);
                }
                new Thread(new ThreadStart(threadRoutine)).Start(); running = true;
            }
            catch (SocketException e)
            {
                Log.Error("Unable to Connect\n" +e.Message);
            }
        }

        // public method to shut down connection
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

        //Method runs in its own thread

        private void threadRoutine()
        {

            // Main Loop which is checking, 
            do{
                if (messegesSend.Count == 0)
                {
                    continue;
                }
                else
                {
                    messagesReceived.Add(socketSendReceive(messegesSend.First()));
                    messegesSend.Remove(messegesSend.First());
                }
            } while (running && solrSocket.Connected);


            //Closing Connection
            Log.Info("Connection shutdown");
            if (solrSocket.Connected)
            {
                solrSocket.Close();
                Log.Info("Socket to: "+ipAddress+" closed!");
            }

           
        }

        //Method sends a request to the solr server and waits for response

        private string socketSendReceive(string request) {

            string content = "";
            byte[] bytesReceived = new byte[256];
            int bytes = 0; 
            // Request send to the Server


            //Der request braucht anscheind eine gewisse form, siehe unten
            solrSocket.Send(stringToByteArray("GET / HTTP/1.1\r\nHost: " + ipAddress +
            "\r\nConnection: Close\r\n\r\n"));


            //Eigendlicher Requeust, nur nimmt ihn der sever nicht so ganz an

          //  solrSocket.Send(stringToByteArray(request), request.Length, 0);




            Log.Info("Message was send: " + request);

            //Receive solr server request
            do
            {
              
                bytes = solrSocket.Receive(bytesReceived, bytesReceived.Length, 0);
               
                content += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
               

            } while (bytes > 0);


            Log.Info("Message from " + ipAddress + ": " + content);

            return content;

        }
    }
}
