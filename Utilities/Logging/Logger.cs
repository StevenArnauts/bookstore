using System;
using System.IO;
using NLog;
using NLog.Config;
using NLogLogger = NLog.Logger;

namespace Utilities.Logging
{
    public static class Logger
    {
		private static XmlLoggingConfiguration configuration;

		public static void Debug(object sender, string message)
        {
            SendMessage(LogLevel.Debug, sender, message);
        }

        public static void Info(object sender, string message)
        {
            SendMessage(LogLevel.Info, sender, message);
        }

        public static void Warn(object sender, string message, Exception error = null)
        {
            SendMessage(LogLevel.Warn, sender, message, error);
        }

        public static void Error(object sender, Exception error)
        {
            SendMessage(LogLevel.Error, sender, error.Message, error);
        }

        public static void Error(object sender, string message, Exception error = null)
        {
            SendMessage(LogLevel.Error, sender, message, error);
        }

        public static void Configure(string configFilePath)
        {
            string file = Path.IsPathRooted(configFilePath) ? configFilePath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFilePath);
			configuration = new XmlLoggingConfiguration(file, false);
			LogManager.Configuration = configuration;
        }

        private static void SendMessage(LogLevel level, object sender, string message, Exception exception = null)
        {
            NLogLogger logger = GetLoggerBySender(sender);
            LogEventInfo evt = new LogEventInfo(level, logger.Name, message);
            if (exception != null)
            {
                evt.Exception = exception;
            }
            logger.Log(evt);
        }

        private static NLogLogger GetLoggerBySender(object sender)
        {
            string loggerName = GetLoggerName(sender);
            return GetLoggerByName(loggerName);
        }

        public static NLogLogger GetLoggerByName(string name)
        {
            NLogLogger logger = LogManager.GetLogger(name);
            return logger;
        }

        private static string GetLoggerName(object sender)
        {
            if (sender == null)
            {
                return string.Empty;
            }
            if (sender is Type)
            {
                return ((Type) sender).FullName;
            }
            if (sender is string)
            {
                return (string) sender;
            }
            return sender.GetType().FullName;
        }
    }
}