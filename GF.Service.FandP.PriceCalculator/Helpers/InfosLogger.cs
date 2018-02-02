using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Custom namespaces
using GF.Components.ErrorHandling;
using log4net;
using log4net.Config;

namespace GF.Service.FandP.PriceCalc.Helpers
{
    /// <summary>
    /// Used to Log the informations on input and output.
    /// Is specified by the web.configs log4net configuration section.
    /// </summary>
    public static class InfosLogger
    {
        #region Private static variables
        /// <summary>
        /// Specified if the logger is configured or not.
        /// </summary>
        private static bool isConfigured = false;
        /// <summary>
        /// ILog static instance.
        /// </summary>
        private static ILog serviceLogger = null;
        #endregion Private static variables

        #region Public static methods
        /// <summary>
        /// Logs the object to log with info log level.
        /// The object is XML serialized and then logged as an ordinary log entry with Info level.
        /// </summary>
        /// <typeparam name="T">The type of the object to log</typeparam>
        /// <param name="objectToLog">The object to log</param>
        /// <param name="typeName">The type name of the object to log</param>
        public static void Info<T>(T objectToLog, string typeName)
        {
            if (objectToLog != null)
            {
                ErrorInfo error;
                string xml;
                if (!SerializationHelper.Serialize(objectToLog, out xml, out error))
                {
                    Info(typeName + " failed to serialize");
                }
                else
                {
                    Info(xml);
                }
            }
            else
            {
                Info(typeName + " is null");
            }
        }
        #endregion Public static methods

        #region Private static methods
        /// <summary>
        /// Logs the XML serialized object string with Info log level.
        /// </summary>
        /// <param name="infoString">The string to be logged, null or an XML serialized object.</param>
        private static void Info(string infoString)
        {
            try
            {
                if (!isConfigured)
                {
                    serviceLogger = LogManager.GetLogger("InfosLogger");
                    XmlConfigurator.Configure();
                    isConfigured = true;
                }

                serviceLogger.Info(infoString + Environment.NewLine);
            }
            catch
            {
                // Do nothing. Ensure against unhandled exceptions
                isConfigured = false;
            }

        }
        #endregion Private static methods
    }
}