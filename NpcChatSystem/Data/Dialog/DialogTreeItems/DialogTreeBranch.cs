using System;
using System.Collections.Generic;
using System.Linq;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;

namespace NpcChatSystem.Data.Dialog.DialogTreeItems
{
    /// <summary>
    /// A section of dialog between between multiple <see cref="Character"/>s within part of the <see cref="DialogTree"/>
    /// </summary>
    public class DialogTreeBranch : ProjectNotificationObject
    {
        public static readonly Random Rand = new Random();

        public DialogTreeBranchIdentifier Id { get; }

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

        public object Condition { get; set; } = null;
        public IReadOnlyCollection<DialogSegment> Dialog => m_dialog;

        /// <summary>
        /// Is this the first branch in the <see cref="DialogTree"/>
        /// </summary>
        public bool isTreeRoot { get; internal set; } = false;

        public event Action<DialogSegmentIdentifier> OnDialogCreated;
        public event Action<DialogSegmentIdentifier> OnDialogDestroyed;

        private readonly List<DialogSegment> m_dialog = new List<DialogSegment>();
        private string m_name = "New Tree Branch";

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
            int id = m_dialog.Count+1;
            while(m_dialog.Any(d => d.CharacterId == id))
            {
                id = Rand.Next();
            }

            return id;
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

            return m_dialog.FirstOrDefault(d => d.Id.Compatible(id));
        }

        public DialogSegment this[DialogSegmentIdentifier id] => GetDialogSegment(id);

        public static implicit operator DialogTreeIdentifier(DialogTreeBranch d) => d.Id;
        public static implicit operator DialogTreeBranchIdentifier(DialogTreeBranch d) => d.Id;
    }
}
