using System.Collections.Generic;
using System.Linq;
using NpcChatSystem.Data.Util;

namespace NpcChatSystem.Data.Dialog.DialogTreeItems
{
    /// <summary>
    /// A section of dialog between two branches
    /// </summary>
    public class TreePart : ProjectObject
    {
        public DialogTreePartIdentifier Id { get; }

        public object Condition { get; set; }
        public IReadOnlyCollection<DialogSegment> Dialog => m_dialog;

        private readonly List<DialogSegment> m_dialog = new List<DialogSegment>();

        internal TreePart(NpcChatProject project, DialogTreeIdentifier dialogId, int treePartId)
            : base(project)
        {
            Id = new DialogTreePartIdentifier(dialogId, treePartId);
        }

        public DialogSegment CreateNewDialog(int characterId = -1)
        {
            DialogSegment dialog = new DialogSegment(Project, Id, m_dialog.Count + 1, characterId);
            m_dialog.Add(dialog);
            return dialog;
        }

        public bool RemoveDialog(DialogSegment id)
        {
            //todo add implicit conversion from DialogSegment to DialogSegmentIdentifier
            return RemoveDialog(id.Id);
        }

        public bool RemoveDialog(DialogSegmentIdentifier id)
        {
            for (int i = m_dialog.Count - 1; i >= 0; i--)
            {
                if (m_dialog[i].Id == id)
                {
                    m_dialog.RemoveAt(i);
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
    }
}
