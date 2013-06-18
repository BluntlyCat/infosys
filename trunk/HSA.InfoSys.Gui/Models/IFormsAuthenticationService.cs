// ------------------------------------------------------------------------
// <copyright file="IFormsAuthenticationService.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Models
{
    /// <summary>
    /// This class checks the values of the web form.
    /// </summary>
    public interface IFormsAuthenticationService
    {
        /// <summary>
        /// User log in.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createPersistentCookie">if set to <c>true</c> [create persistent cookie].</param>
        /// <exception cref="System.ArgumentException">The value must not be empty.</exception>
        void SignIn(string userName, bool createPersistentCookie);

        /// <summary>
        /// User log out.
        /// </summary>
        void SignOut();
    }
}
