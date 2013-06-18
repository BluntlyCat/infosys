// ------------------------------------------------------------------------
// <copyright file="AccountValidation.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System.Web.Security;

    /// <summary>
    /// This class returns an error message according to the error during account validation.
    /// </summary>
    public static class AccountValidation
    {
        /// <summary>
        /// Returns the error message.
        /// </summary>
        /// <param name="createStatus">The create status.</param>
        /// <returns>The error message.</returns>
        public static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // Unter "http://go.microsoft.com/fwlink/?LinkID=177550" finden Sie
            // eine vollständige Liste mit Statuscodes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return Properties.Resources.VALIDATE_ERROR_USER_EXISTS;

                case MembershipCreateStatus.DuplicateEmail:
                    return Properties.Resources.VALIDATE_ERROR_MAIL_EXISTS;

                case MembershipCreateStatus.InvalidPassword:
                    return Properties.Resources.VALIDATE_ERROR_PASSWORD_INVALID;

                case MembershipCreateStatus.InvalidEmail:
                    return Properties.Resources.VALIDATE_ERROR_MAIL_INVALID;

                case MembershipCreateStatus.InvalidAnswer:
                    return Properties.Resources.VALIDATE_ERROR_PASSWORD_CALL_INVALID;

                case MembershipCreateStatus.InvalidQuestion:
                    return Properties.Resources.VALIDATE_ERROR_QUESTION_INVALID;

                case MembershipCreateStatus.InvalidUserName:
                    return Properties.Resources.VALIDATE_ERROR_USERNAME_INVALID;

                case MembershipCreateStatus.ProviderError:
                    return Properties.Resources.VALIDATE_ERROR_PROVIDER_ERROR;

                case MembershipCreateStatus.UserRejected:
                    return Properties.Resources.VALIDATE_ERROR_USER_REJECTED;

                default:
                    return Properties.Resources.VALIDATE_ERROR_UNKOWN_ERROR;
            }
        }
    }
}