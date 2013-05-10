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

    //enum  represents possible types in which a solr - query - respond can be represent by
    public enum MimeType { 
          xml, json, python, ruby, php, csv
    }

    //SolrClient deals as an API

    class SolrClient
    {
        
        string collection = "collection1";
        private static readonly ILog Log = Logging.Logging.GetLogger("SolrClient");
        private Socket solrSocket;
        private string ipAddress;
        private int port;
        private bool running;
        private int queryTicket = 0;

        private Dictionary<int, string> messagesSend = new Dictionary<int, string>();
        private Dictionary<int, string> messagesReceived = new Dictionary<int, string>();
        
       


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
           
            
        }


        // Method to send a query to solr
        public int solrQuery (string queryString,
          //  string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            MimeType mimeType)
        {
            string query = "/solr/"+collection+"/select?q="+queryString+"&wt="+mimeType;
            messagesSend.Add(queryTicket, query);
            Log.Info("Received a request for a solr query: "+ query);
            return queryTicket++;
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

        //Method runs in its own thread

        private void threadRoutine()
        {

            // Main Loop which is checking, 
            do{
                if (messagesSend.Count == 0)
                {
                    continue;
                }
                else
                {
                    messagesReceived.Add(messagesSend.First().Key, socketSendReceive(messagesSend.First().Value));
                   // int key = messagesSend.First().Key;
                    messagesSend.Remove(messagesSend.First().Key);

                   // Log.Error("! KEY --> "+ key);

                    Thread.Sleep(100);

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
            byte[] bytesSend;
            int bytes = 0; 

            // Request send to the Server
            request = "GET "+ request +" HTTP/1.1\r\n" +
                "Host: "+ipAddress+"\r\n" +
                 "Content-Length: 0\r\n" +
                 "\r\n"; ;

            //Mince request into an byte Array
            bytesSend = new ASCIIEncoding().GetBytes(request);

            //Send request to Solr
            solrSocket.Send(bytesSend);
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
