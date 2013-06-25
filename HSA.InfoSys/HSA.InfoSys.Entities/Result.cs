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
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [DataMember]
        public virtual string Content { get; set; }

        /// <summary>
        /// Gets or sets the component.
        /// </summary>
        /// <value>
        /// The component.
        /// </value>
        [DataMember]
        public virtual Guid ComponentGUID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DataMember]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember]
        public virtual string URL { get; set; }

        /// <summary>
        /// Gets or sets the time.
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
                this.Time,
                this.SizeOf());
        }
    }
}
