// ------------------------------------------------------------------------
// <copyright file="OrgUnitConfig.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This represents the OrgUnitConfiguration
    /// </summary>
    [DataContract]
    [Serializable]
    public class OrgUnitConfig : Entity
    {
        /// <summary>
        /// Gets or sets the URL which should be
        /// used for crawling for the OrgUnit.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember]
        public virtual string URLs { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// The email addresses we want send the results.
        /// </summary>
        /// <value>
        /// The Email.
        /// </value>
        [DataMember]
        public virtual string Emails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether email notification is enabled or not.
        /// If active emails will be send to the owners.
        /// </summary>
        /// <value>
        /// The EmailNotification.
        /// </value>
        [DataMember]
        public virtual bool EmailActive { get; set; }

        /// <summary>
        /// Gets or sets the Days.
        /// Defines for example that the search
        /// starts every third day.
        /// </summary>
        /// <value>
        /// The Days.
        /// </value>
        [DataMember]
        public virtual int Days { get; set; }

        /// <summary>
        /// Gets or sets the Time.
        /// Defines the time the search must start.
        /// </summary>
        /// <value>
        /// The Time for crawling.
        /// </value>
        [DataMember]
        public virtual int Time { get; set; }
        
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
                Properties.Resources.ORGUNITCONFIG_TO_STRING,
                this.EntityId,
                this.URLs,                                                                                                                                                          
                this.Emails,
                this.EmailActive,
                this.Days,
                this.Time,
                this.SchedulerActive,
                this.SizeOf());
        }
    }
}
