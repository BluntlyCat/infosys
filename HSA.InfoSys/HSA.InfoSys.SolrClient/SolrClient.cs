// ------------------------------------------------------------------------
// <copyright file="Class1.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.SolrClient
{
    using System;
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
        /// The messages send are contained in this dictionary
        /// </summary>
        private Dictionary<Guid, string> requestSend = new Dictionary<Guid, string>();

        /// <summary>
        /// The messages received are contained in this dictionary
        /// </summary>
        private Dictionary<Guid, string> responseReceived = new Dictionary<Guid, string>();

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
        public Guid SolrQuery(
            string queryString,
            //// string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            SolrOutputMimeType mimeType)
        {
            Guid queryTicket = Guid.NewGuid();

            string query = string.Format(
                "/solr/{0}/select?q={1}&wt={2}",
                Collection, queryString, mimeType);

            this.requestSend.Add(queryTicket, query);

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_REQUEST_RECEIVED, query);

            return queryTicket;
        }

        /// <summary>
        /// Gets the respond by key.
        /// </summary>
        /// <param name="key">The key you want the response for.</param>
        /// <returns>The respond.</returns>
        public string GetResponseByTicket(Guid ticket)
        {
            string response = string.Empty;

            if (this.responseReceived.ContainsKey(ticket))
            {
                Log.InfoFormat("Response by key [{0}] exists.", ticket);

                response = this.responseReceived[ticket];
                this.responseReceived.Remove(ticket);
            }

            return response;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void StartSearch()
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(this.ipAddress);
                IPEndPoint ipe = new IPEndPoint(ipa, this.port);

                this.solrSocket.Connect(ipe);

                if (this.solrSocket.Connected)
                {
                    Log.InfoFormat(Properties.Resources.SOLR_CLIENT_CONNECTION_ESTABLISHED, this.ipAddress);

                    this.running = true;
                    ThreadRoutine();
                }
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
            Log.Info(Properties.Resources.SOLR_CLIENT_CLOSE_CONNECTION);

            this.running = false;

            solrSocket.Close();
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SOCKET_CLOSED, this.ipAddress);
        }

        /// <summary>
        /// Threads the routine.
        /// </summary>
        private void ThreadRoutine()
        {   
            // Main Loop which is checking, whether there is an message for the server or not
            while (this.running && this.solrSocket.Connected)
            {
                if (this.requestSend.Count > 0)
                {
                    Guid ticket = this.requestSend.First().Key;
                    string query = this.requestSend[ticket];

                    // waiting for the server's responde
                    string response = this.InvokeSolrQuery(query);
                    
                    this.responseReceived.Add(ticket, response);
                    this.requestSend.Remove(ticket);
                }
                
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Sockets the send receive.
        /// </summary>
        /// <param name="query">The search query to solr.</param>
        /// <returns></returns>
        private string InvokeSolrQuery(string query)
        {
            string request = string.Empty;
            string response = string.Empty;
            
            byte[] bytesReceived = new byte[256];
            byte[] bytesSend;
            int bytes = 0;

            // Request send to the Server
            request = string.Format(
                "GET {0} HTTP/1.1\r\nHost: {1}\r\nContent-Length: 0\r\n\r\n",
                query,
                this.ipAddress);

            // Mince request into an byte Array
            bytesSend = new ASCIIEncoding().GetBytes(request);

            // Send request to Solr
            this.solrSocket.Send(bytesSend);
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_MESSAGE_SENT, request);

            // Receive solr server response
            do
            {
                bytes = this.solrSocket.Receive(bytesReceived, bytesReceived.Length, 0);
                response += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_RESULT_RECEIVED, this.ipAddress, response);

            CloseConnection();

            return response;
        }
    }
}