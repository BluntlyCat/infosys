// ------------------------------------------------------------------------
// <copyright file="SolrSearchClient.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.LocalServices
{
    using System;
    using System.IO;
    using System.Net;
    using HSA.InfoSys.Common.Entities;
    using HSA.InfoSys.Common.Exceptions;
    using HSA.InfoSys.Common.Logging;
    using log4net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using WCFServices;

    /// <summary>
    /// This class connects to the Solr server and invokes the search.
    /// </summary>
    public class SolrSearchClient
    {
        /// <summary>
        /// The logger for SolrSearch.
        /// </summary>
        private static readonly ILog Log = Logger<string>.GetLogger("SolrSearchClient");

        /// <summary>
        /// The database manager.
        /// </summary>
        private readonly IDbManager dbManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrSearchClient"/> class.
        /// </summary>
        /// <param name="dbManager">The db manager.</param>
        /// <param name="componentGUID">The component GUID.</param>
        /// <param name="componentName">Name of the component.</param>
        public SolrSearchClient(IDbManager dbManager, Guid componentGUID, string componentName)
        {
            this.dbManager = dbManager;
            this.ComponentGUID = componentGUID;
            this.ComponentName = componentName;
        }

        /// <summary>
        /// Gets or sets the host of our solr server.
        /// </summary>
        /// <value>
        /// The solr host.
        /// </value>
        private string Host { get; set; }

        /// <summary>
        /// Gets or sets the port on whitch solr is listening.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        private int Port { get; set; }

        /// <summary>
        /// Gets or sets the collection.
        /// The collection is the database container
        /// of solr where we store our results.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        private string Collection { get; set; }

        /// <summary>
        /// Gets or sets the solr response.
        /// </summary>
        /// <value>
        /// The solr response.
        /// </value>
        private string SolrResponse { get; set; }

        /// <summary>
        /// Gets or sets the componentGUID.
        /// </summary>
        /// <value>
        /// The componentGUID.
        /// </value>
        private Guid ComponentGUID { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        /// <value>
        /// The name of the component.
        /// </value>
        private string ComponentName { get; set; }

        /// <summary>
        /// Gets the results from solr.
        /// </summary>
        /// <exception cref="SolrResponseBadRequestException">Thrown if we got a bad response from Solr.</exception>
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
            catch (Exception e)
            {
                throw new SolrResponseBadRequestException(e, string.Format("GetResult() for: {0}", this.ComponentName));
            }
        }

        /// <summary>
        /// Connects this client to solr and starts the search.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public void StartSearch(SolrSearchClientSettings settings)
        {
            try
            {
                this.SetConnectionProperties();

                if (settings.IsDefault())
                {
                    return;
                }

                Log.InfoFormat(Properties.Resources.SOLR_CLIENT_CONNECTION_ESTABLISHED, this.Host);
                var solrQuery = this.BuildSolrQuery(settings, this.ComponentName);
                this.SolrResponse = this.InvokeSolrQuery(solrQuery);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Properties.Resources.SOLR_CLIENT_UNABLE_TO_CONNECT, e.Message);
            }
        }

        /// <summary>
        /// Sets the connection properties.
        /// If settings have changed there we will get
        /// the updated values for searching.
        /// </summary>
        private void SetConnectionProperties()
        {
            var settings = this.dbManager.GetSolrClientSettings();

            if (settings.IsDefault())
            {
                return;
            }

            this.Host = settings.Host;
            this.Port = settings.Port;

            this.SolrResponse = string.Empty;
            this.Collection = settings.Collection;
        }

        /// <summary>
        /// Connects to the solr server, sends our query
        /// and receive the result.
        /// </summary>
        /// <param name="solrQuery">The solr query.</param>
        /// <returns>
        /// The search result from solr.
        /// </returns>
        private string InvokeSolrQuery(string solrQuery)
        {
            var url = string.Format("http://{0}:{1}{2}", this.Host, this.Port, solrQuery);
            var request = WebRequest.Create(url);

            request.Method = "GET";

            var response = request.GetResponse();
            var data = response.GetResponseStream();

            if (data != null)
            {
                var reader = new StreamReader(data);

                var responseFromSolr = reader.ReadToEnd();

                reader.Close();
                data.Close();
                response.Close();

                return responseFromSolr;
            }

            return string.Empty;
        }

        /// <summary>
        /// Build the query string for Solr.
        /// </summary>
        /// <param name="settings">The settings for Solr.</param>
        /// <param name="componentName">The query string is the actual search term.</param>
        /// <returns>
        /// The query string to send to solr..
        /// </returns>
        private string BuildSolrQuery(
            SolrSearchClientSettings settings,
            string componentName)
        {
            var filterQuery = string.Format(settings.FilterQuery, componentName);

            var query = string.Format(
                settings.QueryFormat,
                this.Collection,
                filterQuery);

            query = query.Replace(" ", "%20");

            Log.InfoFormat(Properties.Resources.SOLR_CLIENT_QUERY_BUILT, query);

            return query;
        }

        /// <summary>
        /// Parses to result.
        /// This method parses the json string we get from solr
        /// into a resultPot which stores all the results.
        /// </summary>
        /// <param name="jsonResult">The result in json format.</param>
        /// <returns>The results in a list.</returns>
        private SolrResultPot ParseToResult(string jsonResult)
        {
            var json = JsonConvert.DeserializeObject(jsonResult) as JObject;

            var resultPot = new SolrResultPot(this.ComponentGUID);

            if (json != null)
            {
                var response = json["response"];
                var docs = response["docs"];

                foreach (var doc in docs)
                {
                    var result = new Result();

                    var content = this.GetJsonValue(doc, "content").Replace(".", ".\r\n");

                    result.ContentHash = content.GetHashCode();
                    result.Content = content.Length > 300 ? string.Format(
                        "{0}...",
                        content.Substring(0, 300)) : content;

                    result.URL = this.GetJsonValue(doc, "url");
                    result.Title = this.RemoveSpecialChars(this.GetJsonValue(doc, "title"));
                    result.ComponentGUID = this.ComponentGUID;

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
            }

            return resultPot;
        }

        /// <summary>
        /// Gets the json value.
        /// Returns an empty string if the key
        /// does not exist.
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
        /// This is only a experiment to make the
        /// title more readable because it often has
        /// some curious characters...
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
