// ------------------------------------------------------------------------
// <copyright file="AccountMembershipService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System;
    using System.Net.Mail;
    using System.Web;
    using System.Web.Security;

    // Der FormsAuthentication-Typ ist versiegelt und enthält statische Member, weshalb
    // Komponententests des Codes, von dem die Member aufgerufen werden, nicht ganz einfach sind. Von der Schnittstellen- und Helper-Klasse weiter unten wird veranschaulicht,
    // wie ein abstrakter Wrapper für einen solchen Typ erstellt wird, um dafür zu sorgen, dass für den AccountController-
    // Code Komponententests ausgeführt werden können.

    /// <summary>
    /// This class provides services for membership control.
    /// </summary>
    public class AccountMembershipService : IMembershipService
    {
        /// <summary>
        /// The membership provider.
        /// </summary>
        private readonly MembershipProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        public AccountMembershipService() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountMembershipService"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public AccountMembershipService(MembershipProvider provider)
        {
            this.provider = provider ?? Membership.Provider;
        }

        /// <summary>
        /// Gets the length of the min password.
        /// </summary>
        /// <value>
        /// The length of the min password.
        /// </value>
        public int MinPasswordLength
        {
            get
            {
                return this.provider.MinRequiredPasswordLength;
            }
        }

        /// <summary>
        /// Validates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// True if validated.
        /// </returns>
        /// <exception cref="System.ArgumentException">The value must not be empty.;userName
        /// or
        /// The value must not be empty.;password</exception>
        public bool ValidateUser(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_USERNAME);
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_PASSWORD);
            }

            return this.provider.ValidateUser(userName, password);
        }

        /// <summary>
        /// Creates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <returns>The status of the creation process.</returns>
        /// <exception cref="System.ArgumentException">The value must not be empty.;userName
        /// or
        /// The value must not be empty.;password</exception>
        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_USERNAME);
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_PASSWORD);
            }

            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_MAIL);
            }

            MembershipCreateStatus status;
            //// ORIGINAL: 6th Parameter is IsApproved property - which defaults to true
            //// provider.CreateUser(userName, password, email, null, null, true, null, out status);
            //// MODIFICATION: Set the IsApproved property to false
            this.provider.CreateUser(userName, password, email, null, null, false, null, out status);
            return status;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if password is changed.</returns>
        /// <exception cref="System.ArgumentException">The value must not be empty.;userName
        /// or
        /// The value must not be empty.;oldPassword
        /// or
        /// The value must not be empty.;newPassword</exception>
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_USERNAME);
            }

            if (string.IsNullOrEmpty(oldPassword))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_OLD_PASSWORD);
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_NEW_PASSWORD);
            }

            // In bestimmten Fehlerszenarios wird von der zugrunde liegenden ChangePassword()-Methode
            // nicht "false" zurückgegeben, sondern eine Ausnahme ausgelöst.
            try
            {
                MembershipUser currentUser = this.provider.GetUser(userName, true /* userIsOnline */);
                return currentUser.ChangePassword(oldPassword, newPassword);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends the confirmation email.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public void SendConfirmationEmail(string userName)
        {
            MembershipUser user = Membership.GetUser(userName);
            string confirmationGuid = user.ProviderUserKey.ToString();
            string verifyUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) +
                             "/account/verify?ID=" + confirmationGuid;

            MembershipUser admin = Membership.GetUser("webmaster");

            var message = new MailMessage("infosysss13@web.de", admin.Email)
            {
                Subject = "activate user",
                Body = verifyUrl
            };

            var client = new SmtpClient();
            //// client.EnableSsl = true;

            client.Send(message);
        }
    }
}
