﻿// ------------------------------------------------------------------------
// <copyright file="SolrClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.SolrClient
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    ///  SolrClient deals as an API
    /// </summary>
    public class SolrClient
    {
        /// <summary>
        /// The logger for SolrClient.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SolrClient");

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrClient"/> class.
        /// </summary>
        /// <param name="port">The port .</param>
        /// <param name="ipAddress">The ip address.</param>
        public SolrClient(int port, string ipAddress)
        {
            this.Port = port;
            this.Host = ipAddress;
            this.SolrResponse = string.Empty;
            this.Collection = Properties.Settings.Default.SOLR_COLLECTION;
        }

        /// <summary>
        /// Gets the socket.
        /// </summary>
        /// <value>
        /// The socket.
        /// </value>
        public Socket SolrSocket { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; private set; }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        public string Collection { get; private set; }

        /// <summary>
        /// Gets the solr response.
        /// </summary>
        /// <value>
        /// The solr response.
        /// </value>
        public string SolrResponse { get; private set; }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SolrClient"/> is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if running; otherwise, <c>false</c>.
        /// </value>
        public bool Running { get; private set; }

        /// <summary>
        /// Gets the response from solr.
        /// </summary>
        /// <returns>The response.</returns>
        public List<Result> GetResult()
        {
            var start = this.SolrResponse.IndexOf('{');
            var end = this.SolrResponse.LastIndexOf('}');

            try
            {
                var result = this.SolrResponse.Substring(start, end - start + 1);
                return this.ParseToResult(result);
            }
            catch
            {
                return new List<Result>();
            }
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <param name="query">The query pattern for solr.</param>
        public void StartSearch(string query)
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(this.Host);
                IPEndPoint ipe = new IPEndPoint(ipa, this.Port);

                this.SolrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.SolrSocket.Connect(ipe);

                if (this.SolrSocket.Connected)
                {
                    Log.InfoFormat(Properties.Resources.SOLR_CLIENT_CONNECTION_ESTABLISHED, this.Host);
                    string solrQuery = this.BuildSolrQuery(query, MimeType.json);
                    this.Running = true;

                    this.SolrResponse = this.ThreadRoutine(solrQuery);
                }
            }
            catch (Exception e)
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

            this.Running = false;

            this.SolrSocket.Disconnect(false);
            this.SolrSocket.Close();
            
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SOCKET_CLOSED, this.Host);
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
            MimeType mimeType)
        {
            Guid queryTicket = Guid.NewGuid();

            string query = string.Format(
                Properties.Settings.Default.SOLR_QUERY_FORMAT,
                this.Collection,
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
            while (this.Running && this.SolrSocket.Connected)
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
                Properties.Settings.Default.SOLR_REQUEST_FORMAT,
                solrQuery,
                "\r\n",
                this.Host,
                "\r\n",
                "\r\n\r\n");

            // Mince request into an byte Array
            bytesSend = new ASCIIEncoding().GetBytes(request);

            // Send request to Solr
            this.SolrSocket.Send(bytesSend);
            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_MESSAGE_SENT, request);

            // Receive solr server response
            do
            {
                bytes = this.SolrSocket.Receive(bytesReceived, bytesReceived.Length, 0);
                response += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_RESULT_RECEIVED, this.Host, response);

            this.CloseConnection();

            return response;
        }

        /// <summary>
        /// Parses to result.
        /// </summary>
        /// <param name="jsonResult">The result in json format.</param>
        /// <returns>The results in a list.</returns>
        private List<Result> ParseToResult(string jsonResult)
        {
            var json = JsonConvert.DeserializeObject(jsonResult) as JObject;
            var results = new List<Result>();

            var response = json["response"];
            var docs = response["docs"];

            foreach (var doc in docs)
            {
                var result = new Result();

                result.Content = doc["content"].ToString();
                result.URL = doc["url"].ToString();
                result.Title = this.RemoveSpecialChars(doc["title"].ToString());
                result.Time = (DateTime)doc["tstamp"];

                results.Add(result);

                Log.Info(string.Format("Search resualt was added! ", result.Title));
            }

            return results;
        }

        /// <summary>
        /// Removes the special chars.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>A hopefully better looking title.</returns>
        private string RemoveSpecialChars(string text)
        {
            string tmp = string.Empty;

            text = text.Replace("\r", string.Empty);
            text = text.Replace("\n", string.Empty);
            text = text.Replace("\"", string.Empty);

            foreach (var c in text)
            {
                if ('!' <= c && c <= '~')
                {
                    tmp += c;
                }
                else
                {
                    tmp += '#';
                }
            }

            tmp = tmp.Replace('#', ' ').Trim();

            return tmp;
        }
    }
}