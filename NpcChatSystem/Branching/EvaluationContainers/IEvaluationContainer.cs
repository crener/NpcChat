using System.ComponentModel;
using System.Xml;
using NpcChatSystem.Branching.Conditions;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;

namespace NpcChatSystem.Branching.EvaluationContainers
{
    /// <summary>
    /// Describes the required functionality of a container which is used to determine which branch to use when
    /// multiple branches are linked in a <see cref="DialogTree"/>
    /// </summary>
    public interface IEvaluationContainer : INotifyPropertyChanged
    {
        string EvaluatorName { get; }

        /// <summary>
        /// Priority which this container should be evaluated in 
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Amount of evaluation levels the container currently contains
        /// </summary>
        int Depth { get; set; }

        /// <summary>
        /// When comparing multiple conditions how should this be handled (and, or, etc) 
        /// </summary>
        EvaluationType ComparisonType { get; set; }

        /// <summary>
        /// Determines if the current state of the world should allow this set of conditions to be true
        /// </summary>
        /// <returns>true if world state matches the conditions</returns>
        bool Evaluate(int level);
    }

    /// <summary>
    /// Core object which is able to evaluate an <see cref="IEvaluationContainer"/> of the corresponding type.
    /// An evaluator is required is required to correctly handle more complected evaluation strategies such as <see cref="LeveledEvaluationContainer"/>
    /// </summary>
    public interface IConditionEvaluator
    {
        /// <summary>
        /// Returns the id of the next branch to execute to
        /// </summary>
        /// <param name="currentBranch">Branch to find the next branch for</param>
        /// <returns>next tree branch if one could be found</returns>
        DialogTreeBranchIdentifier NextBranch(DialogTreeBranchIdentifier currentBranch);

        /// <summary>
        /// Create a new container that is compatible with the  
        /// </summary>
        /// <returns>new evaluation container</returns>
        IEvaluationContainer CreateContainer();

        IEvaluationContainer LoadContainer(XmlElement element);
        XmlElement SaveContainer();
    }

    public enum EvaluationType {
        Default,
        And,
        Or
    }
}
