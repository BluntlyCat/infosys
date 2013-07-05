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
    /// <typeparam name="T">The type of the key for the LogContainerDictionary.</typeparam>
    public static class Logger<T>
    {
        /// <summary>
        /// The configuration of log4net.
        /// </summary>
        private static ICollection config;

        /// <summary>
        /// The base logger.
        /// </summary>
        private static readonly Type BaseLogger = typeof(Logger<Type>);

        /// <summary>
        /// The LogContainerDictionary dictionary
        /// </summary>
        private static readonly Dictionary<T, ILog> LogContainerDictionary = new Dictionary<T, ILog>();

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

            return LogContainerDictionary[name];
        }

        /// <summary>
        /// Creates the base logger and config.
        /// </summary>
        private static void CreateBaseLogger()
        {
            config = XmlConfigurator.Configure();
            Logger<Type>.AddLogger(BaseLogger);

            Logger<Type>.LogContainerDictionary[BaseLogger].Debug(Properties.Resources.LOGGING_BASELOGGER_CREATED);
        }

        /// <summary>
        /// Adds the logger.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        private static void AddLogger(T name)
        {
            if (Logger<T>.LogContainerDictionary.ContainsKey(name) == false)
            {
                Logger<T>.LogContainerDictionary.Add(name, LogManager.GetLogger(typeof(T)));
                Logger<Type>.LogContainerDictionary[BaseLogger].DebugFormat(Properties.Resources.LOGGING_NEW_LOGGER_ADDED, name);
            }
            else
            {
                Logger<Type>.LogContainerDictionary[BaseLogger].DebugFormat(Properties.Resources.LOGGING_LOGGER_EXISTS, name);
            }
        }
    }
}