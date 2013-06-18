// ------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    //// Hinweis: Anweisungen zum Aktivieren des klassischen Modus von IIS6 oder IIS7 
    //// finden Sie unter "http://go.microsoft.com/?LinkId=9394801".

    /// <summary>
    /// This is the main file of this MVC project.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Routenname
                "{controller}/{action}/{id}", // URL mit Parametern
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // Parameterstandardwerte
        }

        /// <summary>
        /// Application main entrance.
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
        }
    }
}