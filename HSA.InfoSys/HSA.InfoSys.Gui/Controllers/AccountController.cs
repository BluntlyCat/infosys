// ------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using HSA.InfoSys.Gui.Models;

    /// <summary>
    /// The controller for authentication.
    /// </summary>
    [HandleError]
    public class AccountController : Controller
    {
        /// <summary>
        /// Gets or sets the forms service.
        /// </summary>
        /// <value>
        /// The forms service.
        /// </value>
        public IFormsAuthenticationService FormsService { get; set; }

        /// <summary>
        /// Gets or sets the membership service.
        /// </summary>
        /// <value>
        /// The membership service.
        /// </value>
        public IMembershipService MembershipService { get; set; }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        /// <summary>
        /// User log in.
        /// </summary>
        /// <returns>The result of the log in process.</returns>
        public ActionResult LogOn()
        {
            //// Testing
            //// MembershipUser user = Membership.GetUser("kokoloko");
            //// user.Comment = "";
            //// Membership.UpdateUser(user);

            this.ViewData["navid"] = "login";
            return this.View();
        }

        /// <summary>
        /// User log on.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>The result of the log on process.</returns>
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            this.ViewData["navid"] = "login";

            this.ViewData["invalidLoginMsg"] = "Die Anmeldung war nicht erfolgreich. Korrigieren Sie die Fehler, und wiederholen Sie den Vorgang.";

            if (ModelState.IsValid)
            {
                if (this.MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    this.FormsService.SignIn(model.UserName, model.RememberMe);

                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }
                    else
                    {
                        return this.RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Der angegebene Benutzername oder das angegebene Kennwort ist ungültig.");
                }
            }

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten; Formular erneut anzeigen.
            return this.View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        /// <summary>
        /// User log off.
        /// </summary>
        /// <returns>The result of the log off process.</returns>
        public ActionResult LogOff()
        {
            this.FormsService.SignOut();

            return this.RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        /// <summary>
        /// User registration.
        /// </summary>
        /// <returns>The result of the registration process.</returns>
        public ActionResult Register()
        {
            this.ViewData["navid"] = "register";
            this.ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return this.View();
        }

        /// <summary>
        /// The registration process.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="captchaValid">if set to <c>true</c> [captcha valid].</param>
        /// <returns>The result of the registration process.</returns>
        [MvcReCaptcha.CaptchaValidator]
        [HttpPost]
        public ActionResult Register(RegisterModel model, bool captchaValid)
        {
            this.ViewData["navid"] = "register";

            this.ViewData["invalidRegisterMsg"] = "Das Konto konnte nicht erstellt werden. Korrigieren Sie die Fehler, und wiederholen Sie den Vorgang.";
            
            if (!captchaValid)
            {
                ModelState.AddModelError(string.Empty, "You did not type the verification word correctly. Please try again.");
            }

            // Sobald dem ModelState ein ModelError geadded wird, ist ModelState nicht mehr Valid
            if (ModelState.IsValid)
            {
                // Versuch, den Benutzer zu registrieren
                MembershipCreateStatus createStatus = this.MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    //// User will not be logged in until the admin accepted it
                    //// FormsService.SignIn(model.UserName, false /* createPersistentCookie */);

                    this.MembershipService.SendConfirmationEmail(model.UserName);

                    return this.RedirectToAction("Confirmation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, AccountValidation.ErrorCodeToString(createStatus));
                }
            }

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten; Formular erneut anzeigen.
            this.ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return this.View(model);
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <returns>The result of the password change process.</returns>
        [Authorize]
        public ActionResult ChangePassword()
        {
            this.ViewData["navid"] = "changepw";
            this.ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;

            return this.View();
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The result of the change password process.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            this.ViewData["navid"] = "changepw";
            this.ViewData["invalidMsg"] = "Das Kennwort konnte nicht geändert werden. Korrigieren Sie die Fehler, und wiederholen Sie den Vorgang.";

            if (ModelState.IsValid)
            {
                if (this.MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                {
                    this.ViewData["success"] = "Das Kennwort wurde erfolgreich geändert.";
                    return this.View();
                    //// return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Das aktuelle Kennwort ist nicht korrekt, oder das Kennwort ist ungültig.");
                }
            }

            // Wurde dieser Punkt erreicht, ist ein Fehler aufgetreten; Formular erneut anzeigen.
            this.ViewData["PasswordLength"] = this.MembershipService.MinPasswordLength;
            return this.View(model);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        /// <summary>
        /// Changes the password success.
        /// </summary>
        /// <returns>The result of the password changing process on success.</returns>
        public ActionResult ChangePasswordSuccess()
        {
            return this.View();
        }

        /// <summary>
        /// Show the confirmation page.
        /// </summary>
        /// <returns>The result of the confirmation process.</returns>
        public ActionResult Confirmation()
        {
            return this.View();
        }

        /// <summary>
        /// Verifies the specified ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The result of the verification process.</returns>
        public ActionResult Verify(string id)
        {
            // if (string.IsNullOrEmpty(ID) || (!Regex.IsMatch(ID, @"[0-9a-f]{8}\-([0-9a-f]{4}\-){3}[0-9a-f]{12}")))
            if (string.IsNullOrEmpty(id))
            {
                this.TempData["tempMessage"] = "The user account is not valid. Please try clicking the link in your email again.";
                return this.RedirectToAction("LogOn");
            }
            else
            {
                //// MembershipUser user = Membership.GetUser(new Guid(ID));
                MembershipUser user = Membership.GetUser(Convert.ToInt32(id));

                if (!user.IsApproved)
                {
                    user.IsApproved = true;
                    Membership.UpdateUser(user);
                    //// FormsService.SignIn(user.UserName, false);
                    return this.RedirectToAction("LogOn");
                }
                else
                {
                    //// FormsService.SignOut();
                    this.TempData["tempMessage"] = "user is already active";
                    return this.RedirectToAction("LogOn");
                }
             }
        }

        /// <summary>
        /// Initializes data which may will be no available while the constructor is called.
        /// </summary>
        /// <param name="requestContext">The HTTP-context and the route data.</param>
        protected override void Initialize(RequestContext requestContext)
        {
            if (this.FormsService == null)
            { 
                this.FormsService = new FormsAuthenticationService();
            }

            if (this.MembershipService == null)
            { 
                this.MembershipService = new AccountMembershipService();
            }

            base.Initialize(requestContext);
        }
    }
}
