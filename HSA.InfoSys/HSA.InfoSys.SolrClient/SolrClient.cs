// ------------------------------------------------------------------------
// <copyright file="SolrClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.SolrClient
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using HSA.InfoSys.Common.Logging;
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
        private static readonly ILog Log = Logger<string>.GetLogger("SolrClient");

        /// <summary>
        /// The solr socket.
        /// </summary>
        private Socket solrSocket;

        /// <summary>
        /// The ip address to the Solr server.
        /// </summary>
        private string ipAddress;

        /// <summary>
        /// The solr response.
        /// </summary>
        private string solrResponse;

        /// <summary>
        /// The port on which Solr is listening.
        /// </summary>
        private int port;

        /// <summary>
        /// Indicates if the thread is running.
        /// </summary>
        private bool running;

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
        /// Gets the response from solr.
        /// </summary>
        /// <returns>The response.</returns>
        public string GetResponse()
        {
            return this.solrResponse;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <param name="query">The query pattern for solr.</param>
        public void StartSearch(string query)
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(this.ipAddress);
                IPEndPoint ipe = new IPEndPoint(ipa, this.port);

                this.solrSocket.Connect(ipe);

                if (this.solrSocket.Connected)
                {
                    Log.InfoFormat(Properties.Resources.SOLR_CLIENT_CONNECTION_ESTABLISHED, this.ipAddress);
                    string solrQuery = this.BuildSolrQuery(query, SolrOutputMimeType.xml);
                    this.running = true;

                    this.solrResponse = this.ThreadRoutine(solrQuery);
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

            this.solrSocket.Close();
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SOCKET_CLOSED, this.ipAddress);
        }

        /// <summary>
        /// Build the query string for Solr.
        /// </summary>
        /// <param name="queryString">The query string is the actual search term.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>
        /// The query string to send to solr..
        /// </returns>
        private string BuildSolrQuery(
            string queryString,
            //// string fq, string sort, int start, int rows, string fl, string df, string[] rawQueryParameters, 
            SolrOutputMimeType mimeType)
        {
            Guid queryTicket = Guid.NewGuid();

            string query = string.Format(
                "/solr/{0}/select?q={1}&wt={2}",
                Collection,
                queryString,
                mimeType);

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_REQUEST_RECEIVED, query);

            return query;
        }

        /// <summary>
        /// Threads the routine.
        /// </summary>
        /// <param name="solrQuery">The solr query.</param>
        /// <returns>The search result from solr.</returns>
        private string ThreadRoutine(string solrQuery)
        {
            string response = string.Empty;

            // Main Loop which is checking, whether there is an message for the server or not
            while (this.running && this.solrSocket.Connected)
            {
                // waiting for the server's responde
                response = this.InvokeSolrQuery(solrQuery);

                Thread.Sleep(100);
            }

            return response;
        }

        /// <summary>
        /// Sockets the send receive.
        /// </summary>
        /// <param name="solrQuery">The solr query.</param>
        /// <returns>The search result from solr.</returns>
        private string InvokeSolrQuery(string solrQuery)
        {
            string request = string.Empty;
            string response = string.Empty;
            
            byte[] bytesReceived = new byte[256];
            byte[] bytesSend;
            int bytes = 0;

            // Request send to the Server
            request = string.Format(
                "GET {0} HTTP/1.1\r\nHost: {1}\r\nContent-Length: 0\r\n\r\n",
                solrQuery,
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

            this.CloseConnection();

            return response;
        }
    }
}