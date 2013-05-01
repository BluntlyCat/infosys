namespace HSA.InfoSys.WebCrawler
{
    using System;
    using System.Collections.Generic;
    using SolrNet;
    using SolrNet.Attributes;
    using SolrNet.Commands.Parameters;
    using Microsoft.Practices.ServiceLocation;

    class SecurityIssues
    {

        [SolrUniqueKey("issue")]
        public int issue { get; set; }

        [SolrField("issueid")]
        public int issueID { get; set; }

        [SolrField("issuedate")]
        public DateTime issueDate { get; set; }

        [SolrField("titel")]
        public string titel { get; set; }

        [SolrField("threatlevel")]
        public int threatLevel { get; set; }

    }
}