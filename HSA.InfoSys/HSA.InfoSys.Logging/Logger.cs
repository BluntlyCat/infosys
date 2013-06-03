// ------------------------------------------------------------------------
// <copyright file="logger.cs" company="HSA.InfoSys">
//     Copyright statement. All right reserved
// </copyright>
// ------------------------------------------------------------------------
namespace HSA.InfoSys.Common.Logging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using log4net;
    using log4net.Config;

    /// <summary>
    /// Handles requests for logger for a specific class,
    /// creates the configuration if there is none.
    /// </summary>
    /// <typeparam name="T">The type of the key for the logger.</typeparam>
    public static class Logger<T>
    {
        /// <summary>
        /// The configuration of log4net.
        /// </summary>
        private static ICollection config;

        /// <summary>
        /// The base logger.
        /// </summary>
        private static Type baseLogger = typeof(Logger<Type>);

        /// <summary>
        /// The logger dictionary
        /// </summary>
        private static Dictionary<T, ILog> logger = new Dictionary<T, ILog>();

        /// <summary>
        /// Gets the logger and add a new logger if not exist.
        /// </summary>
        /// <param name="name">The name of the new logger.</param>
        /// <returns>The ILog logger.</returns>
        public static ILog GetLogger(T name)
        {
            if (config == null)
            {
                CreateBaseLogger();
            }

            AddLogger(name);

            return logger[name];
        }

        /// <summary>
        /// Creates the base logger and config.
        /// </summary>
        public static void CreateBaseLogger()
        {
            config = XmlConfigurator.Configure();
            Logger<Type>.AddLogger(baseLogger);

            Logger<Type>.logger[baseLogger].Debug("Base logger successfully created");
        }

        /// <summary>
        /// Adds the logger.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        private static void AddLogger(T name)
        {
            if (Logger<T>.logger.ContainsKey(name) == false)
            {
                Logger<T>.logger.Add(name, LogManager.GetLogger(typeof(T)));
                Logger<Type>.logger[baseLogger].DebugFormat("Added new logger [{0}]", name);
            }
            else
            {
                Logger<Type>.logger[baseLogger].DebugFormat("Logger already exists [{0}]", name);
            }
        }
    }
}