// ------------------------------------------------------------------------
// <copyright file="EmailNotifierSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// There we can store settings for email notifier.
    /// </summary>
    [DataContract]
    [Serializable]
    public class EmailNotifierSettings : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailNotifierSettings"/> class.
        /// </summary>
        public EmailNotifierSettings()
        {
            this.SmtpServer = string.Empty;
            this.SmtpPort = 0;
            this.MailFrom = string.Empty;
        }

        /// <summary>
        /// Gets or sets the SMTP server.
        /// </summary>
        /// <value>
        /// The SMTP server.
        /// </value>
        [DataMember]
        public virtual string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the SMTP port.
        /// </summary>
        /// <value>
        /// The SMTP port.
        /// </value>
        [DataMember]
        public virtual int SmtpPort { get; set; }

        /// <summary>
        /// Gets or sets the mail from.
        /// </summary>
        /// <value>
        /// The mail from.
        /// </value>
        [DataMember]
        public virtual string MailFrom { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                Properties.Resources.MAILNOTIFIERSETTINGS_TO_STRING,
                this.SmtpServer,
                this.SmtpPort,
                this.MailFrom,
                this.SizeOf());
        }
    }
}
