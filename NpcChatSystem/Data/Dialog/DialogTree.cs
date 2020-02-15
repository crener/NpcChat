using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// A branching tree following all options within a conversation
    /// </summary>
    public class DialogTree : ProjectNotificationObject
    {
        private static Random s_random = new Random();

        public DialogTreeIdentifier Id { get; }
        public IReadOnlyList<DialogTreeBranchIdentifier> Branches => m_branches.Select(b => (DialogTreeBranchIdentifier)b).ToList();
        public string TreeName
        {
            get => m_treeName;
            set
            {
                m_treeName = value;
                RaisePropertyChanged();
            }
        }

        public event Action<DialogTreeBranch> OnBranchCreated;
        public event Action<DialogTreeBranch> OnBranchRemoved;

        private List<DialogTreeBranch> m_branches = new List<DialogTreeBranch>();
        private string m_treeName;

        internal DialogTree(NpcChatProject project, int id)
            : base(project)
        {
            Id = new DialogTreeIdentifier(id);
            DialogTreeBranch branch = CreateNewBranch();
            branch.IsTreeRoot = true;
        }

        public DialogTreeBranch CreateNewBranch()
        {
            int id = GenerateId();

            DialogTreeBranch branch = new DialogTreeBranch(m_project, Id, id);
            if (m_branches.Any(b => b.Name == branch.Name))
            {
                int iteration = 0;
                string potentialName;
                do
                {
                    iteration++;
                    potentialName = $"{branch.Name} ({iteration})";
                } while (m_branches.Any(b => b.Name == potentialName));

                branch.Name = potentialName;
            }

            m_branches.Add(branch);
            OnBranchCreated?.Invoke(branch);

            return branch;
        }

        public DialogTreeBranch CreateNewBranch(DialogTreeBranchIdentifier parent)
        {
            DialogTreeBranch newBranch = CreateNewBranch();
            newBranch.AddParent(parent);
            return newBranch;
        }

        private int GenerateId()
        {
            int id;
            do
            {
                id = s_random.Next(1, int.MaxValue);
            } while (m_branches.Any(d => d.Id.DialogTreeId == id));

            return id;
        }

        public bool RemoveBranch(DialogTreeBranchIdentifier id)
        {
            if (!HasBranch(id)) return false;

            DialogTreeBranch branch = GetBranch(id);
            foreach (DialogTreeBranchIdentifier child in branch.Children)
            {
                Logging.Logger.Log(LogLevel.Warn, $"Orphaned child of '{branch.Name}' called '{this[child].Name}'");
                branch.RemoveChild(child);
            }
            foreach (DialogTreeBranchIdentifier parent in branch.Parents)
                branch.RemoveParent(parent);

            m_branches.Remove(branch);
            OnBranchRemoved?.Invoke(branch);

            return true;
        }

        public DialogTreeBranch GetStart()
        {
            return m_branches.FirstOrDefault(b => b.IsTreeRoot);
        }

        public DialogTreeBranchIdentifier GetNextBranch(DialogTreeBranchIdentifier branchId)
        {
            //todo implement branch evaluation
            throw new NotImplementedException();

            /*DialogTreeBranch branchTree = this[branchId];
            List<IGrouping<int, DialogTreeBranch>> depth = branchTree.Children
                .Select(b => m_project[b])
                .GroupBy(g => g.BranchCondition.Depth)
                .ToList();
            int maxDepth = depth.Last().Key;

            for (int i = maxDepth - 1; i >= 0; i--)
            {
                foreach (IGrouping<int, DialogTreeBranch> dialogTreeBranches in depth)
                {

                }
            }

            return branchTree.Children.First();*/
        }

        public DialogTreeBranch GetBranch(DialogTreeBranchIdentifier id)
        {
            if (!Id.Compatible(id)) return null;

            return m_branches.FirstOrDefault(d => d.Id == id);
        }

        public bool HasBranch(DialogTreeBranchIdentifier id)
        {
            if (!Id.Compatible(id)) return false;

            return m_branches.Any(d => d.Id == id);
        }

        public DialogTreeBranch this[DialogTreeBranchIdentifier id] => GetBranch(id);
        public DialogSegment this[DialogSegmentIdentifier id] => GetBranch(id)?[id];

        public static implicit operator DialogTreeIdentifier(DialogTree d) => d.Id;
    }
}
