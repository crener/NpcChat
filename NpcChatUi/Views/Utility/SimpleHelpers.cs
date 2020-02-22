using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
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

        public static string Text(this FlowDocument doc)
        {
            if(doc == null) throw new NullReferenceException("FlowDocument shouldn't be null!");

            StringBuilder strings = new StringBuilder();

            foreach(Block block in doc.Blocks)
            {
                if(block is Paragraph content)
                {
                    foreach(Run inline in content.Inlines)
                    {
                        if(inline == null) continue;
                        strings.Append(inline.Text);
                    }
                }
            }

            return strings.ToString();
        }
    }
}
