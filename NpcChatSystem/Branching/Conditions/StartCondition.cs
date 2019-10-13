using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Branching.Conditions
{
    public class StartCondition : AbstractCondition
    {
        public bool DefaultNode { get; set; }

        public override bool Evaluate()
        {
            return true;
        }
    }
}
