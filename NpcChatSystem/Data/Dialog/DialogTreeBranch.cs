﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NpcChatSystem.Branching.EvaluationContainers;
using NpcChatSystem.Data.Character;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// A section of dialog between between multiple <see cref="Character"/>s within part of the <see cref="DialogTree"/>
    /// </summary>
    [DebuggerDisplay("{Name}")]
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
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Determines the order that branches are evaluated in
        /// </summary>
        public int BranchEvaluationPriority => BranchCondition?.Priority ?? 0;

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
                RaisePropertyChanged();
            }
        }

        public IReadOnlyCollection<DialogSegment> Dialog => m_dialog;

        public event Action<DialogSegmentIdentifier> OnDialogCreated;
        public event Action<DialogSegmentIdentifier> OnDialogDestroyed;
        public event Action<DialogTreeBranchIdentifier> OnBranchParentAdded;
        public event Action<DialogTreeBranchIdentifier> OnBranchChildAdded;
        public event Action<DialogTreeBranchIdentifier> OnBranchParentRemoved;
        public event Action<DialogTreeBranchIdentifier> OnBranchChildRemoved;

        private string m_name = "New Tree Branch";
        private IEvaluationContainer m_branchCondition = null;
        private HashSet<DialogTreeBranchIdentifier> m_children = new HashSet<DialogTreeBranchIdentifier>();
        private HashSet<DialogTreeBranchIdentifier> m_parents = new HashSet<DialogTreeBranchIdentifier>();
        private readonly List<DialogSegment> m_dialog = new List<DialogSegment>();

        internal DialogTreeBranch(NpcChatProject project, DialogTreeIdentifier dialogId, int treePartId)
            : base(project)
        {
            Id = new DialogTreeBranchIdentifier(dialogId, treePartId, branch:this);
        }

        public DialogSegment CreateNewDialog(int characterId = -1)
        {
            DialogSegment dialog = new DialogSegment(m_project, Id, GenerateId(), characterId);
            m_dialog.Add(dialog);

            RaisePropertyChanged(nameof(Dialog));
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

        public bool AddChild(DialogTreeBranchIdentifier childId)
        {
            if (m_children.Contains(childId)) return false;
            if (Id == childId) return false;
            if (!m_project.ProjectDialogs.HasDialog(childId)) return false;

            m_children.Add(childId);
            m_project[childId].AddParent(Id);

            OnBranchChildAdded?.Invoke(childId);
            return true;
        }

        public bool RemoveChild(DialogTreeBranchIdentifier childId)
        {
            if (!m_children.Contains(childId)) return false;

            m_children.Remove(childId);
            m_project[childId].RemoveParent(Id);

            OnBranchChildRemoved?.Invoke(childId);
            return true;
        }

        public bool AddParent(DialogTreeBranchIdentifier parentId)
        {
            if (m_parents.Contains(parentId)) return false;
            if (this.Id == parentId) return false;
            if (!m_project.ProjectDialogs.HasDialog(parentId)) return false;

            m_parents.Add(parentId);
            m_project[parentId].AddChild(Id);

            OnBranchParentAdded?.Invoke(parentId);
            return true;
        }

        public bool RemoveParent(DialogTreeBranchIdentifier id)
        {
            if (!m_parents.Contains(id)) return false;

            m_parents.Remove(id);
            m_project[id].RemoveChild(Id);

            OnBranchParentRemoved?.Invoke(id);
            return true;
        }

        public bool RemoveDialog(DialogSegment id)
        {
            if (id == null) return false;
            return RemoveDialog(id.Id);
        }

        public bool RemoveDialog(DialogSegmentIdentifier id)
        {
            if(id == null) return false;

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
