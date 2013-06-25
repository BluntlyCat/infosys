// ------------------------------------------------------------------------
// <copyright file="WCFControllerHostSettings.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Entities
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WCFControllerHostSettings : Entity
    {
        /// <summary>
        /// Gets or sets the certificate path.
        /// </summary>
        /// <value>
        /// The certificate path.
        /// </value>
        [DataMember]
        public virtual string CertificatePath { get; set; }

        /// <summary>
        /// Gets or sets the certificate password.
        /// </summary>
        /// <value>
        /// The certificate password.
        /// </value>
        [DataMember]
        public virtual string CertificatePassword { get; set; }
    }
}
