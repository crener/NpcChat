using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Branching.Conditions
{
    public interface ICondition
    {
        /// <summary>
        /// Checks if the condition matches in the current project
        /// </summary>
        /// <returns>true if condition matches</returns>
        bool Evaluate();
    }
}
