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
        public List<Result> results = new List<Result>();

        public SolrJsonParser()
        {
            //this.results = new List<Result>();
        }

        public void ParsetoString(string result)
        { 
            var json = JsonConvert.DeserializeObject(result) as JObject;
            var response = json["response"];
            var docs = response["docs"];
            Result r;
            foreach (var doc in docs)
            {
                r = new Result();
                r.content = doc["content"].ToString();
                r.url = doc["url"].ToString();
                r.tstamp = (DateTime)doc["tstamp"];

                this.results.Add(r);
            }
        }
        public List<Result> getResults()
        {
            return this.results;
        }

        
    }
}
