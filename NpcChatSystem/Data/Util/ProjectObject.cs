using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Data.Util
{
    public class ProjectObject
    {
        protected NpcChatProject Project { get; private set; }

        public ProjectObject(NpcChatProject project)
        {
            Project = project;
        }
    }
}
