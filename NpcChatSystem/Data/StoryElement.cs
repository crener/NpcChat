using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data.Util;
using ValueType = NpcChatSystem.Data.Util.ValueType;

namespace NpcChatSystem.Data
{
    public class StoryElement
    {
        public string Name = "";
        public string Value = null;
        public ValueType Type = ValueType.Unknown;
    }
}
