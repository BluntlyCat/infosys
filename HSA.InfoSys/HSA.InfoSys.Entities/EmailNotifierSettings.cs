// ------------------------------------------------------------------------
// <copyright file="EmailNotifierSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System.Runtime.Serialization;

    [DataContract]
    public class EmailNotifierSettings : Entity
    {
        /// <summary>
        /// Gets or sets the SMTP server.
        /// </summary>
        /// <value>
        /// The SMTP server.
        /// </value>
        [DataMember]
        public virtual string SmtpServer { get; set; }

        /// <summary>
        /// Gets or sets the mail from.
        /// </summary>
        /// <value>
        /// The mail from.
        /// </value>
        public virtual string MailFrom { get; set; }
    }
}
