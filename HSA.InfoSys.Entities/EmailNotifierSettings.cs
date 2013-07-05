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
    public class EmailNotifierSettings : Settings
    {
        /// <summary>
        /// Gets or sets the SMTP server address.
        /// </summary>
        /// <value>
        /// The SMTP server address.
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
        /// Gets or sets the mail address from which account we want to send mails.
        /// </summary>
        /// <value>
        /// The mail address of the sending account.
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

        /// <summary>
        /// Sets the default values.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void SetDefaults()
        {
            var newSettings = this.GetDefaults() as EmailNotifierSettings;

            if (newSettings != null)
            {
                this.SmtpServer = newSettings.SmtpServer;
                this.SmtpPort = newSettings.SmtpPort;
                this.MailFrom = newSettings.MailFrom;
            }
        }

        /// <summary>
        /// Gets the settings with default values.
        /// </summary>
        /// <returns>A new settings object with its default values.</returns>
        public virtual Settings GetDefaults()
        {
            var newSettings = new EmailNotifierSettings
                {
                    SmtpServer = "localhost",
                    SmtpPort = 25,
                    MailFrom = "your.name@your_server"
                };

            return newSettings;
        }

        /// <summary>
        /// Determines whether this settings has default values.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this settings has default values; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsDefault()
        {
            return this.Equals(this.GetDefaults());
        }
    }
}
