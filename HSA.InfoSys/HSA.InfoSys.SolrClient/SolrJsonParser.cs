// ------------------------------------------------------------------------
// <copyright file="SolrJsonParser.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
//------------------------------------------------------------------------- 
namespace HSA.InfoSys.Common.SolrClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// SolrJsonParser parses the response of SolrClient
    /// </summary>
    public class SolrJsonParser
    {
        /// <summary>
        /// The results list in which all Results can be found
        /// </summary>
        private List<Result> results = new List<Result>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SolrJsonParser"/> class.
        /// </summary>
        public SolrJsonParser()
        {
            this.results = new List<Result>();
        }

        /// <summary>
        /// Parses to string.
        /// </summary>
        /// <param name="result">The result.</param>
        public void ParseToString(string result)
        {
            var json = JsonConvert.DeserializeObject(result) as JObject;
            var response = json["response"];
            var docs = response["docs"];
            Result r;
            foreach (var doc in docs)
            {
                r = new Result();
                r.Content = doc["content"].ToString();
                r.Url = doc["url"].ToString();
                r.Tstamp = (DateTime)doc["tstamp"];
                this.results.Add(r);
            }
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        /// <returns>The results</returns>
        public List<Result> GetResults()
        {
            return this.results;
        }
    }
}
