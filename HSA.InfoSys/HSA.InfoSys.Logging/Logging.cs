﻿namespace HSA.InfoSys.Logging
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
        private static ICollection config;

        private static string baseLogger = "Logging";

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
                logger[baseLogger].DebugFormat("Logger already exists [{0}]", name);
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

            logger[baseLogger].Debug("Base logger successfully created");
        }

        /// <summary>
        /// Adds the logger.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        private static void AddLogger(string name)
        {
            logger.Add(name, LogManager.GetLogger(name));
            logger[baseLogger].DebugFormat("Added new logger [{0}]", name);
        }
    }
}