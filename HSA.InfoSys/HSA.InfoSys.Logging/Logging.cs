// ------------------------------------------------------------------------
// <copyright file="Logging.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Logging
{
    using System.Collections;
    using System.Collections.Generic;
    using log4net;
    using log4net.Config;

    /// <summary>
    /// Handles requests for logger for a specific class,
    /// creates the configuration if there is none.
    /// </summary>
    public class Logging
    {
        /// <summary>
        /// The configuration for this logging instance.
        /// </summary>
        private static ICollection config;

        /// <summary>
        /// The base logger.
        /// </summary>
        private static string baseLogger = "Logging";

        /// <summary>
        /// The logger dictionary.
        /// </summary>
        private static Dictionary<string, ILog> logger = new Dictionary<string, ILog>();

        /// <summary>
        /// Gets the logger and add a new logger if not exist.
        /// </summary>
        /// <param name="name">The name of the new logger.</param>
        /// <returns>The ILog logger.</returns>
        public static ILog GetLogger(string name)
        {
            if (config == null)
            {
                CreateBaseLogger();
            }

            if (string.IsNullOrEmpty(name))
            {
                return logger[baseLogger];
            }

            if (logger.ContainsKey(name) == false)
            {
                AddLogger(name);
            }
            else
            {
                logger[baseLogger].DebugFormat(Properties.Resources.LOGGING_LOGGER_EXISTS, name);
            }

            return logger[name];
        }

        /// <summary>
        /// Creates the base logger and config.
        /// </summary>
        private static void CreateBaseLogger()
        {
            config = XmlConfigurator.Configure();
            AddLogger(baseLogger);

            logger[baseLogger].Debug(Properties.Resources.LOGGING_BASELOGGER_CREATED);
        }

        /// <summary>
        /// Adds the logger.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        private static void AddLogger(string name)
        {
            logger.Add(name, LogManager.GetLogger(name));
            logger[baseLogger].DebugFormat(Properties.Resources.LOGGING_NEW_LOGGER_ADDED, name);
        }
    }
}