using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using NpcChatSystem.Annotations;
using NpcChatSystem.Branching.Conditions;
using NpcChatSystem.Data.Util;

namespace NpcChatSystem.Branching.EvaluationContainers
{
    /// <summary>
    /// Simple evaluator which takes multiple <see cref="ICondition"/> and checks each one according to the <see cref="AbstractEvaluationContainer.ComparisonType"/>
    /// </summary>
    [Export(typeof(IEvaluationContainer))]
    public class SimpleEvaluationContainer : AbstractEvaluationContainer
    {
        public override string EvaluatorName => "Simple";
        public IReadOnlyList<ICondition> Conditions => m_conditions;

        private readonly List<ICondition> m_conditions = new List<ICondition>();

        public SimpleEvaluationContainer()
        {

        }

        public override bool Evaluate(int level)
        {
            switch(ComparisonType)
            {
                case EvaluationType.And:
                    return EvaluateAnd();
                case EvaluationType.Or:
                    return EvaluateOr();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool EvaluateAnd()
        {
            foreach(ICondition condition in Conditions)
            {
                if(!condition.Evaluate()) return false;
            }

            return true;
        }

        private bool EvaluateOr()
        {
            foreach (ICondition condition in Conditions)
            {
                if (condition.Evaluate()) return true;
            }

            return false;
        }

        public void AddCondition(ICondition condition)
        {
            m_conditions.Add(condition);
            OnPropertyChanged(nameof(Conditions));
        }

        public bool RemoveCondition(ICondition condition)
        {
            if (!m_conditions.Contains(condition))
            {
                return false;
            }

            m_conditions.Remove(condition);
            OnPropertyChanged(nameof(Conditions));

            return true;
        }
    }
}
