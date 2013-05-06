namespace HSA.InfoSys.WebCrawler
{
    using System;
    using SolrNet.Attributes;

    /// <summary>
    /// This class represents the issue.
    /// </summary>
    public class SecurityIssues
    {
        /// <summary>
        /// Gets or sets the issue.
        /// </summary>
        /// <value>
        /// The issue.
        /// </value>
        [SolrUniqueKey("issue")]
        public int Issue { get; set; }

        /// <summary>
        /// Gets or sets the issue ID.
        /// </summary>
        /// <value>
        /// The issue ID.
        /// </value>
        [SolrField("issueid")]
        public int IssueID { get; set; }

        /// <summary>
        /// Gets or sets the issue date.
        /// </summary>
        /// <value>
        /// The issue date.
        /// </value>
        [SolrField("issuedate")]
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [SolrField("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the threat level.
        /// </summary>
        /// <value>
        /// The threat level.
        /// </value>
        [SolrField("threatlevel")]
        public int ThreatLevel { get; set; }
    }
}