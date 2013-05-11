namespace HSA.InfoSys.WebCrawler
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using HSA.InfoSys.Logging;
    using log4net;
    using System;

   


    /// <summary>
    ///  SolrClient deals as an API
    /// </summary>
    public class SolrClient
    {
        private  const string collection = "collection1";

        private static readonly ILog Log = Logging.GetLogger("SolrClient");
        private Socket solrSocket;
        private string ipAddress;
        private int port;
        private bool running;
        private int queryTicket = 0;

        /// <summary>
        /// The messages send are contained in this dictionarry
        /// </summary>
        private Dictionary<int, string> messagesSend = new Dictionary<int, string>();
        /// <summary>
        /// The messages received are contained in thes dictionarry
        /// </summary>
        private Dictionary<int, string> messagesReceived = new Dictionary<int, string>();


        /// <summary>
        /// Initializes a new instance of the <see cref="SolrClient"/> class.
        /// </summary>
        /// <param name="port">The port .</param>
        /// <param name="ipAddress">The ip address.</param>
        public SolrClient(int port, string ipAddress)
        {
            this.port = port;
            this.ipAddress = ipAddress;
            solrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }



        /// <summary>
        /// Commits to solr.
        /// </summary>
        public void commitToSolr( )
        {
           
            
        }



        /// <summary>
        /// Solrs the query.
        /// </summary>
        /// <param name="queryString">The query string is the actual search term.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns></returns>
        public int solrQuery (string queryString,
          //  string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            SolrOutputMimeType mimeType)
        {
            string query = "/solr/"+collection+"/select?q="+queryString+"&wt="+mimeType;
            messagesSend.Add(queryTicket, query);
            Log.Info("Received a request for a solr query: "+ query);
            return queryTicket++;
        }


        /// <summary>
        /// Gets the respond by key.
        /// </summary>
        /// <param name="key">The key you want the response for.</param>
        /// <returns></returns>
        public string getRespondByKey(int key)
        {
            string respondse = "";
            if(messagesReceived.ContainsKey(key)){
                respondse = messagesReceived[key];
                messagesReceived.Remove(key);
            }
            return respondse;
        }


        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void connect() {
            try
            {
                IPAddress ipa = IPAddress.Parse(ipAddress);
                IPEndPoint ipe = new IPEndPoint(ipa, port);
                solrSocket.Connect(ipe);
                if (solrSocket.Connected)
                {
                    Log.Info("Connection Established: " + ipAddress);
                }

                //Starting a Thread which runs the threadRoutine
                new Thread(new ThreadStart(threadRoutine)).Start(); running = true;
            }
            catch (SocketException e)
            {
                Log.Error("Unable to Connect\n" +e.Message);
            }
        }


        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void closeConnection()
        {
            running = false;
        }



        /// <summary>
        /// Threads the routine.
        /// </summary>
        private void threadRoutine()
        {

            // Main Loop which is checking, whether there is an message for the server or not
            do{
                if (messagesSend.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                else
                {
                    int key = messagesSend.First().Key;
                    string request = messagesSend[key];

                    //waiting for the server's responde
                    messagesReceived.Add(key, socketSendReceive(request));
                    messagesSend.Remove(key);
                   
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



        /// <summary>
        /// Sockets the send receive.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
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
