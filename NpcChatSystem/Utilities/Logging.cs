using NLog;

namespace NpcChatSystem.Utilities
{
    public static class Logging
    {
        public static Logger Logger { get; }

        static Logging()
        {
            Logger = LogManager.GetCurrentClassLogger();
        }
    }
}
