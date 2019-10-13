using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Annotations;
using NpcChatSystem.Branching.Conditions;

namespace NpcChatSystem.Branching.EvaluationContainers
{
    /// <summary>
    /// Evaluates condition by grouping the <see cref="ICondition"/> by priority and checks them in groups.
    /// If a single group matches based on the <see cref="LeveledEvaluationContainer.ComparisonType"/> it returns true.
    /// </summary>
    [Export(typeof(IEvaluationContainer))]
    public class LeveledEvaluationContainer : AbstractEvaluationContainer
    {
        public override string EvaluatorName => "Leveled";

        public override bool Evaluate(int level)
        {
            throw new NotImplementedException();
        }
    }
}
