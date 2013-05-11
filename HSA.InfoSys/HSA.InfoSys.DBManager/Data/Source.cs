namespace HSA.InfoSys.DBManager.Data
{
    using System;

    /// <summary>
    /// This class represents the source of an issue.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// Gets or sets the source GUID.
        /// </summary>
        /// <value>
        /// The source GUID.
        /// </value>
        public virtual Guid SourceGUID { get; set; }

        /// <summary>
        /// Gets or sets the source URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public virtual string URL { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", SourceGUID, URL);
        }
    }
}
