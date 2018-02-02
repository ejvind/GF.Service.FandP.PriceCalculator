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
    /// Logs the errors.
    /// Is specified by the web.configs log4net configuration section.
    /// </summary>
    public static class ErrorsLogger
    {
        #region Private variables
        /// <summary>
        /// Specified if the logger is configured or not.
        /// </summary>
        private static bool isConfigured = false;
        /// <summary>
        /// ILog static instance.
        /// </summary>
        private static ILog serviceLogger = null;
        #endregion Private variables

        #region Public methods
        /// <summary>
        /// Logs the provided error details inside ErrorInfo object to the error log with Error severity. 
        /// </summary>
        /// <param name="errorMessage">ErrorInfo object encapsulating error details.</param>
        public static void Error(ErrorInfo errorMessage)
        {
            try
            {
                if (!isConfigured)
                {
                    serviceLogger = LogManager.GetLogger("ErrorsLogger");
                    XmlConfigurator.Configure();
                    isConfigured = true;
                }

                serviceLogger.Error(errorMessage.ToString());
            }
            catch
            {
                // Do nothing. Ensure against unhandled exceptions
                isConfigured = false;
            }
        }
        #endregion Public methods
    }
}