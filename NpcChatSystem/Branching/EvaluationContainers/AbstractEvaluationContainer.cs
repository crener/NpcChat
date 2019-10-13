using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Annotations;
using NpcChatSystem.Branching.Conditions;

namespace NpcChatSystem.Branching.EvaluationContainers
{
    public abstract class AbstractEvaluationContainer : IEvaluationContainer
    {
        public virtual string EvaluatorName => GetType().FullName;
        public virtual int Priority { get; set; } = 100;
        public virtual int Depth { get; set; }
        public EvaluationType ComparisonType { get; set; }

        public abstract bool Evaluate(int level);

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
