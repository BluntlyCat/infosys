namespace HSA.InfoSys.DBManager.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Issue
    {

        /// <summary>
        /// Gets or sets the issue GUID.
        /// </summary>
        /// <value>
        /// The issue GUID.
        /// </value>
        public virtual Guid issueGUID { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public virtual string text { get; set; }

        /// <summary>
        /// Gets or sets the titel.
        /// </summary>
        /// <value>
        /// The titel.
        /// </value>
        public virtual string titel { get; set; }

        /// <summary>
        /// Gets or sets the threat level.
        /// </summary>
        /// <value>
        /// The threat level.
        /// </value>
        public virtual int threatLevel { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public virtual DateTime date { get; set; }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        public virtual Component component { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public virtual Source source { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}",
                issueGUID, text, titel, threatLevel, date, component, source);
        }
    }
}
