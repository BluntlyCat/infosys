// ------------------------------------------------------------------------
// <copyright file="SystemConfig.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.DBManager.Data
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This represents the SystemConfiguration
    /// </summary>
    [DataContract]
    public class SystemConfig : Entity
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        [DataMember]
        public virtual string URL { get; set; }

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
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether email notification is enabled or not.
        /// </summary>
        /// <value>
        /// The EmailNotification.
        /// </value>
        [DataMember]
        public virtual bool EmailNotification { get; set; }

        /// <summary>
        /// Gets or sets the scheduler.
        /// </summary>
        /// <value>
        /// The scheduler.
        /// </value>
        [DataMember]
        public virtual Scheduler Scheduler { get; set; }
        
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
                "{0}, {1}, {2}, {3}, {4}, {5}, {6}",
                this.EntityId,
                this.URL,
                this.URLActive,
                this.Email,
                this.EmailNotification,
                this.Scheduler,
                this.SchedulerActive);
        }
    }
}
