// ------------------------------------------------------------------------
// <copyright file="IMembershipService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System.Web.Security;

    /// <summary>
    /// The interface for the membership implementation.
    /// </summary>
    public interface IMembershipService
    {
        /// <summary>
        /// Gets the length of the min password.
        /// </summary>
        /// <value>
        /// The length of the min password.
        /// </value>
        int MinPasswordLength { get; }

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
        bool ValidateUser(string userName, string password);

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
        MembershipCreateStatus CreateUser(string userName, string password, string email);

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
        bool ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// Sends the confirmation email.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        void SendConfirmationEmail(string userName);
    }
}
