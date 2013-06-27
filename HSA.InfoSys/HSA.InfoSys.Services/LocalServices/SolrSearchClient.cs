// ------------------------------------------------------------------------
// <copyright file="SolrSearchClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Logging;
    using HSA.InfoSys.Common.Services.WCFServices;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// This class connects to the Solr server and invokes the search.
    /// </summary>
    public class SolrSearchClient
    {
        /// <summary>
        /// The logger for SolrSearch.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SolrSearch");

        /// <summary>
        /// The database manager.
        /// </summary>
        private IDBManager dbManager = DBManager.ManagerFactory(Guid.NewGuid());

        /// <summary>
        /// The settings.
        /// </summary>
        private SolrSearchClientSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrSearchClient"/> class.
        /// </summary>
        public SolrSearchClient()
        {
            this.settings = this.dbManager.GetSettingsFor<SolrSearchClientSettings>();
            this.Host = this.settings.Host;
            this.Port = this.settings.Port;

            this.SolrResponse = string.Empty;
            this.Collection = this.settings.Collection;
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
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int Port { get; private set; }

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
        /// Gets the component GUID.
        /// </summary>
        /// <value>
        /// The component GUID.
        /// </value>
        public Guid ComponentGUID { get; private set; }

        /// <summary>
        /// Gets the response from solr.
        /// </summary>
        /// <returns>The response.</returns>
        public SolrResultPot GetResult()
        {
            try
            {
                var start = this.SolrResponse.IndexOf('{');
                var end = this.SolrResponse.LastIndexOf('}');
                var result = this.SolrResponse.Substring(start, end - start + 1);

                return this.ParseToResult(result);
            }
            catch
            {
                return new SolrResultPot(this.ComponentGUID);
            }
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        /// <param name="query">The query pattern for solr.</param>
        /// <param name="componentGUID">The component GUID.</param>
        public void StartSearch(string query, Guid componentGUID)
        {
            try
            {
                IPAddress ipa = IPAddress.Parse(this.Host);
                IPEndPoint ipe = new IPEndPoint(ipa, this.Port);

                this.SolrSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.SolrSocket.Connect(ipe);
                this.ComponentGUID = componentGUID;

                if (this.SolrSocket.Connected)
                {
                    Log.InfoFormat(Properties.Resources.SOLR_CLIENT_CONNECTION_ESTABLISHED, this.Host);
                    string solrQuery = this.BuildSolrQuery(query, SolrMimeType.json);
                    this.SolrResponse = this.InvokeSolrQuery(solrQuery);
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

            this.SolrSocket.Disconnect(false);
            this.SolrSocket.Close();

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_SOCKET_CLOSED, this.Host);
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
                this.settings.RequestFormat,
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
            SolrMimeType mimeType)
        {
            Guid queryTicket = Guid.NewGuid();

            var filter = this.settings.Filter.Split(',');
            var formatArgs = new List<string>();
            
            formatArgs.Add(queryString);

            foreach (var arg in filter)
            {
                formatArgs.Add(arg);
            }

            var filterQuery = this.settings.FilterQueryFormat;
            filterQuery = string.Format(this.settings.FilterQueryFormat, formatArgs.ToArray());

            string query = string.Format(
                this.settings.QueryFormat,
                this.Collection,
                filterQuery,
                mimeType);

            query = query.Replace(" ", "%20");

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_REQUEST_RECEIVED, query);

            return query;
        }

        /// <summary>
        /// Parses to result.
        /// </summary>
        /// <param name="jsonResult">The result in json format.</param>
        /// <returns>The results in a list.</returns>
        private SolrResultPot ParseToResult(string jsonResult)
        {
            var json = JsonConvert.DeserializeObject(jsonResult) as JObject;
            var resultPot = new SolrResultPot(this.ComponentGUID);

            var response = json["response"];
            var docs = response["docs"];

            foreach (var doc in docs)
            {
                var result = new Result();

                // todo: Länge von content begrenzen.
                var content = this.GetJsonValue(doc, "content").Replace(".", ".\r\n");

                if (content.Length > 300)
                {
                    result.Content = string.Format("{0}...", content.Substring(0, 300));
                }
                else
                {
                    result.Content = content;
                }

                result.URL = this.GetJsonValue(doc, "url");
                result.Title = this.RemoveSpecialChars(this.GetJsonValue(doc, "title"));

                try
                {
                    result.Time = DateTime.Parse(this.GetJsonValue(doc, "tstamp"));
                }
                catch
                {
                    result.Time = DateTime.Now;
                }

                resultPot.Results.Add(result);

                Log.InfoFormat("Search resualt was added [{0}]", result.Title);
            }

            return resultPot;
        }

        /// <summary>
        /// Gets the json value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value as string if exists or empty string.</returns>
        private string GetJsonValue(JToken token, string key)
        {
            try
            {
                var value = token[key].ToString();
                return value;
            }
            catch
            {
                return string.Empty;
            }
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
