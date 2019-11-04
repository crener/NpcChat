using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Utilities;

namespace NpcChat.Views.Utility
{
    public static class SimpleHelpers
    {
        public static void OpenLink(string link)
        {
            if (string.IsNullOrEmpty(link)) return;
            Logging.Logger.Info($"Opening link '{link}'");

            if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
            {
                System.Diagnostics.Process.Start(link);
            }
            else
            {
                Logging.Logger.Warn($"link not valid Uri, not opening! ('{link}')");
            }
        }
    }
}
