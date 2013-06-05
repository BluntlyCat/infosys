// ------------------------------------------------------------------------
// <copyright file="OrgUnitConfig.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using HSA.InfoSys.Common.NetDataContractSerializer;

    /// <summary>
    /// This represents the OrgUnitConfiguration
    /// </summary>
    [DataContract]
    public class OrgUnitConfig : Entity
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember]
        public virtual string URLS { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the URL is active or not.
        /// </summary>
        /// <value>
        /// The URLActive.
        /// </value>
        [DataMember]
        public virtual bool URLActive { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        /// <value>
        /// The Email.
        /// </value>
        [DataMember]
        public virtual string Emails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether email notification is enabled or not.
        /// </summary>
        /// <value>
        /// The EmailNotification.
        /// </value>
        [DataMember]
        public virtual bool EmailActive { get; set; }

        /// <summary>
        /// Gets or sets the Days.
        /// </summary>
        /// <value>
        /// The Days.
        /// </value>
        [DataMember]
        public virtual int Days { get; set; }

        /// <summary>
        /// Gets or sets the Time.
        /// </summary>
        /// <value>
        /// The Time for crawling.
        /// </value>
        [DataMember]
        public virtual int Time { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [DataMember]
        public virtual DateTime NextSearch { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the scheduler is active or not.
        /// </summary>
        /// <value>
        /// The SchedulerActive.
        /// </value>
        [DataMember]
        public virtual bool SchedulerActive { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                "ID: {0}, URLs: {1}, Urls active: {2}, Mails: {3}, Mails active: {4}, Schedule every {5} days at ({6}), Scheduler active: {7}",
                this.EntityId,
                this.URLS,
                this.URLActive,
                this.Emails,
                this.EmailActive,
                this.Days,
                this.Time,
                this.SchedulerActive);
        }
    }
}
