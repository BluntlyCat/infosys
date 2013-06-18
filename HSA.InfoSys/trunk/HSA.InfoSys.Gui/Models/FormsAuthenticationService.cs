// ------------------------------------------------------------------------
// <copyright file="FormsAuthenticationService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    using System;
    using System.Web.Security;

    /// <summary>
    /// This class checks the values of the web form.
    /// </summary>
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        /// <summary>
        /// User log in.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <exception cref="System.ArgumentException">The value must not be empty.</exception>
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(
                    Properties.Resources.VALIDATE_USER_ARG_EXCEPTION,
                    Properties.Resources.VALIDATE_USER_ARG_USERNAME);
            }

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        /// <summary>
        /// User log out.
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}