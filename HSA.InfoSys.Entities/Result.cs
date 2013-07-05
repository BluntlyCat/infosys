// ------------------------------------------------------------------------
// <copyright file="Result.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents the search result to an issue.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Result : Entity
    {
        /// <summary>
        /// Gets or sets the content we found.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [DataMember]
        public virtual string Content { get; set; }

        /// <summary>
        /// Gets or sets the content hash.
        /// Is a hash of the content for comparing
        /// to avoid redundancy in the database.
        /// </summary>
        /// <value>
        /// The content hash.
        /// </value>
        [DataMember]
        public virtual int ContentHash { get; set; }

        /// <summary>
        /// Gets or sets the componentGUID.
        /// Is the GUID of the component this
        /// result belongs to.
        /// </summary>
        /// <value>
        /// The componentGUID.
        /// </value>
        [DataMember]
        public virtual Guid ComponentGUID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// The title of the found content.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// The url as source where we found the issue.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember]
        public virtual string URL { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// The time when this issue was found.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        public virtual DateTime Time { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.RESULT_TO_STRING,
                this.EntityId,
                this.ComponentGUID,
                this.Title,
                this.URL,
                this.Content,
                this.ContentHash,
                this.Time,
                this.SizeOf());
        }
    }
}
