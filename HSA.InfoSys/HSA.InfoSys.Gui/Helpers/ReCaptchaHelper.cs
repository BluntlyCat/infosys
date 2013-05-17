// ------------------------------------------------------------------------
// <copyright file="ReCaptchaHelper.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace MvcReCaptcha.Helpers
{
    using System.Configuration;
    using System.IO;
    using System.Web.Mvc;
    using System.Web.UI;
    using Recaptcha;

    /// <summary>
    /// Generates a captcha for spam prevention.
    /// </summary>
    public static class ReCaptchaHelper
    {
        /// <summary>
        /// Html Helper to build and render the Captcha control
        /// </summary>
        /// <param name="helper">HtmlHelper class provides a set of helper methods whose purpose is to help you create HTML controls programmatically</param>
        /// <returns>The generated captcha.</returns>
        public static string GenerateCaptcha(this HtmlHelper helper)
        {
            var captchaControl = new RecaptchaControl
                {
                    ID = "recaptcha",
                    Theme = "clean", // http://wiki.recaptcha.net/index.php/Theme
                    PublicKey = ConfigurationManager.AppSettings["ReCaptchaPublicKey"],
                    PrivateKey = ConfigurationManager.AppSettings["ReCaptchaPrivateKey"]
                };

            var htmlWriter = new HtmlTextWriter(new StringWriter());
            captchaControl.RenderControl(htmlWriter);
            return htmlWriter.InnerWriter.ToString();
        }
    }
}