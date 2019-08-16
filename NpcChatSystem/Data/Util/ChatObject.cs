using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Data.Util
{
    public class ChatObject
    {
        protected NpcChatProject Project { get; }

        protected ChatObject(NpcChatProject project)
        {
            Project = project;
        }
    }
}
