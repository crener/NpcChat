using System;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace NpcChatSystem.Utilities
{
    public static class Logging
    {
        public static Logger Logger { get; }

        static Logging()
        {
            Logger = LogManager.GetCurrentClassLogger();

            try
            {
                LogManager.Configuration = new XmlLoggingConfiguration("NLog.config");
            }
            catch (Exception ex)
            {
                LogManager.Configuration = new LoggingConfiguration();
                Logger.Error(ex, "Failed to read NLog config");
            }
        }
    }
}
