// ------------------------------------------------------------------------
// <copyright file="SolrClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
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

    /// <summary>
    ///  SolrClient deals as an API
    /// </summary>
    public class SolrClient
    {
        /// <summary>
        /// The collection where everything is stored in Solr.
        /// </summary>
        private const string Collection = "collection1";

        /// <summary>
        /// The logger for SolrClient.
        /// </summary>
        private static readonly ILog Log = Logging.GetLogger("SolrClient");

        /// <summary>
        /// The solr socket.
        /// </summary>
        private Socket solrSocket;

        /// <summary>
        /// The ip address to the Solr server.
        /// </summary>
        private string ipAddress;

        /// <summary>
        /// The port on which Solr is listening.
        /// </summary>
        private int port;

        /// <summary>
        /// Indicates if the thread is running.
        /// </summary>
        private bool running;

        /// <summary>
        /// The query ticket.
        /// </summary>
        private int queryTicket = 0;

        /// <summary>
        /// The messages send are contained in this dictionary
        /// </summary>
        private Dictionary<int, string> messagesSend = new Dictionary<int, string>();

        /// <summary>
        /// The messages received are contained in this dictionary
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
            this.solrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Commits to solr.
        /// </summary>
        public void CommitToSolr()
        { 
        }

        /// <summary>
        /// Query from Solr.
        /// </summary>
        /// <param name="queryString">The query string is the actual search term.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>The result of the query to Solr.</returns>
        public int SolrQuery(
            string queryString,
          //// string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            SolrOutputMimeType mimeType)
        {
            string query = "/solr/" + Collection + "/select?q=" + queryString + "&wt=" + mimeType;

            this.messagesSend.Add(this.queryTicket, query);
            
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_REQUEST_RECEIVED, query);

            return this.queryTicket++;
        }

        /// <summary>
        /// Gets the respond by key.
        /// </summary>
        /// <param name="key">The key you want the response for.</param>
        /// <returns>The respond.</returns>
        public string GetRespondByKey(int key)
        {
            string respondse = string.Empty;

            if (this.messagesReceived.ContainsKey(key))
            {
                respondse = this.messagesReceived[key];
                this.messagesReceived.Remove(key);
            }

            return respondse;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void Connect()
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(this.ipAddress);
                IPEndPoint ipe = new IPEndPoint(ipa, this.port);

                this.solrSocket.Connect(ipe);

                if (this.solrSocket.Connected)
                {
                    Log.InfoFormat(Properties.Resources.SOLR_CLIENT_CONNECTION_ESTABLISHED, this.ipAddress);
                }

                // Starting a Thread which runs the threadRoutine
                new Thread(new ThreadStart(this.ThreadRoutine)).Start();

                this.running = true;
            }
            catch (SocketException e)
            {
                Log.ErrorFormat(Properties.Resources.SOLR_CLIENT_UNABLE_TO_CONNECT, e.Message);
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection()
        {
            this.running = false;
        }

        /// <summary>
        /// Threads the routine.
        /// </summary>
        private void ThreadRoutine()
        {
            // Main Loop which is checking, whether there is an message for the server or not
            do
            {
                if (this.messagesSend.Count == 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                else
                {
                    int key = this.messagesSend.First().Key;
                    string request = this.messagesSend[key];

                    // waiting for the server's responde
                    this.messagesReceived.Add(key, this.SocketSendReceive(request));
                    this.messagesSend.Remove(key);
                }
            }
            while (this.running && this.solrSocket.Connected);

            // Closing Connection
            Log.Info(Properties.Resources.SOLR_CLIENT_CLOSE_CONNECTION);

            if (this.solrSocket.Connected)
            {
                this.solrSocket.Close();
                Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SOCKET_CLOSED, this.ipAddress);
            }  
        }

        /// <summary>
        /// Sockets the send receive.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string SocketSendReceive(string request)
        {
            string content = string.Empty;
            byte[] bytesReceived = new byte[256];
            byte[] bytesSend;
            int bytes = 0; 

            // Request send to the Server
            request = "GET " + request + " HTTP/1.1\r\n" +
                "Host: " + this.ipAddress + "\r\n" +
                 "Content-Length: 0\r\n" +
                 "\r\n";

            // Mince request into an byte Array
            bytesSend = new ASCIIEncoding().GetBytes(request);

            // Send request to Solr
            this.solrSocket.Send(bytesSend);
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_MESSAGE_SENT, request);

            // Receive solr server request
            do
            {
                bytes = this.solrSocket.Receive(bytesReceived, bytesReceived.Length, 0);
                content += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_RESULT_RECEIVED, this.ipAddress, content);

            return content;
        }
    }
}
