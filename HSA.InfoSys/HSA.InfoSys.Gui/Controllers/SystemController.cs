// ------------------------------------------------------------------------
// <copyright file="SystemController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System.Web.Mvc;
    using System.Web.Security;

    /// <summary>
    /// The controller for the system.
    /// </summary>
    [HandleError]
    public class SystemController : Controller
    {
        /// <summary>
        /// Called when the home page is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Index()
        {
            this.ViewData["navid"] = "mysystems";
            return this.View();
        }

        /// <summary>
        /// Called when page components is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult Components()
        {
            this.ViewData["navid"] = "mysystems";
            return this.View();
        }

        /// <summary>
        /// Called when page search is loading.
        /// </summary>
        /// <returns>The result of this action.</returns>
        [Authorize]
        public ActionResult SearchConfig()
        {
            this.ViewData["navid"] = "mysystems";

            MembershipUser user = Membership.GetUser();
            this.ViewData["useremail"] = user.Email;

            return this.View();
        }
    }
}
