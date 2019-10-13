using System;
using System.Collections.Generic;
using System.Linq;
using NpcChatSystem.Branching.EvaluationContainers;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// A section of dialog between between multiple <see cref="Character"/>s within part of the <see cref="DialogTree"/>
    /// </summary>
    public class DialogTreeBranch : ProjectNotificationObject
    {
        public static readonly Random Rand = new Random();

        public DialogTreeBranchIdentifier Id { get; }

        /// <summary>
        /// Is this the first branch in the <see cref="DialogTree"/>
        /// </summary>
        public bool IsTreeRoot { get; internal set; } = false;

        /// <summary>
        /// Name of Branch, useful mainly for illustrative purposes and debugging
        /// </summary>
        public string Name
        {
            get => m_name;
            set
            {
                m_name = value;
                RaiseChanged();
            }
        }

        /// <summary>
        /// Determines the order that branches are evaluated in
        /// </summary>
        public int BranchEvaluationPriority => BranchCondition.Priority;

        public IReadOnlyList<DialogTreeBranchIdentifier> Children => m_children.ToList();
        public IReadOnlyList<DialogTreeBranchIdentifier> Parents => m_parents.ToList();

        /// <summary>
        /// Set of conditions to match when checking if this branch should be used next
        /// </summary>
        public IEvaluationContainer BranchCondition
        {
            get => m_branchCondition;
            set
            {
                m_branchCondition = value;
                RaiseChanged();
            }
        }

        public IReadOnlyCollection<DialogSegment> Dialog => m_dialog;

        public event Action<DialogSegmentIdentifier> OnDialogCreated;
        public event Action<DialogSegmentIdentifier> OnDialogDestroyed;

        private string m_name = "New Tree Branch";
        private IEvaluationContainer m_branchCondition = null;
        private HashSet<DialogTreeBranchIdentifier> m_children = new HashSet<DialogTreeBranchIdentifier>();
        private HashSet<DialogTreeBranchIdentifier> m_parents = new HashSet<DialogTreeBranchIdentifier>();
        private readonly List<DialogSegment> m_dialog = new List<DialogSegment>();

        internal DialogTreeBranch(NpcChatProject project, DialogTreeIdentifier dialogId, int treePartId)
            : base(project)
        {
            Id = new DialogTreeBranchIdentifier(dialogId, treePartId);
        }

        public DialogSegment CreateNewDialog(int characterId = -1)
        {
            DialogSegment dialog = new DialogSegment(m_project, Id, GenerateId(), characterId);
            m_dialog.Add(dialog);

            RaiseChanged(nameof(Dialog));
            OnDialogCreated?.Invoke(dialog);
            return dialog;
        }

        private int GenerateId()
        {
            int id = m_dialog.Count + 1;
            while (m_dialog.Any(d => d.CharacterId == id))
            {
                id = Rand.Next();
            }

            return id;
        }

        public bool AddChild(DialogTreeBranchIdentifier id)
        {
            if(m_children.Contains(id)) return false;
            if(this.Id == id) return false;

            m_children.Add(id);
            m_project[id].m_parents.Add(this.Id);

            return true;
        }

        public bool AddParent(DialogTreeBranchIdentifier id)
        {
            if(m_parents.Contains(id)) return false;
            if (this.Id == id) return false;

            m_project[id].m_children.Add(this.Id);
            m_parents.Add(id);

            return true;
        }

        public bool RemoveDialog(DialogSegment id)
        {
            return RemoveDialog(id.Id);
        }

        public bool RemoveDialog(DialogSegmentIdentifier id)
        {
            for (int i = m_dialog.Count - 1; i >= 0; i--)
            {
                if (m_dialog[i].Id == id)
                {
                    m_dialog.RemoveAt(i);
                    OnDialogDestroyed?.Invoke(id);
                    return true;
                }
            }

            return false;
        }

        public bool ContainsDialogSegment(DialogSegment segment)
        {
            return ContainsDialogSegment(segment.Id);
        }

        public bool ContainsDialogSegment(DialogSegmentIdentifier id)
        {
            if (!Id.Compatible(id))
            {
                //given Id is not from this tree part
                return false;
            }

            return m_dialog.Any(d => d.Id == id);
        }

        public DialogSegment GetDialogSegment(DialogSegmentIdentifier id)
        {
            if (!Id.Compatible(id)) return null;

            return m_dialog.FirstOrDefault(d => d.Id == id);
        }

        public DialogSegment this[DialogSegmentIdentifier id] => GetDialogSegment(id);

        public static implicit operator DialogTreeIdentifier(DialogTreeBranch d) => d.Id;
        public static implicit operator DialogTreeBranchIdentifier(DialogTreeBranch d) => d.Id;
    }
}
