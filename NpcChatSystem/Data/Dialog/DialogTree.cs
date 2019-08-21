using System;
using System.Collections.Generic;
using System.Linq;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// A branching tree following all options within a conversation
    /// </summary>
    public class DialogTree
    {
        private static Random s_random = new Random();

        public DialogTreeIdentifier Id { get; }
        private List<TreePart> m_dialog = new List<TreePart>();

        internal DialogTree(int id)
        {
            Id = new DialogTreeIdentifier(id);
        }

        public TreePart CreateNewBranch()
        {
            int id;
            do
            {
                id = s_random.Next(1, int.MaxValue);
            } while(m_dialog.Any(d => d.Id.DialogTreeId == id));

            TreePart part = new TreePart(Id, id);
            m_dialog.Add(part);
            return part;
        }

        public TreePart GetStart()
        {
            return m_dialog.FirstOrDefault();
        }

        public TreePart GetTree(DialogTreePartIdentifier id)
        {
            if(!Id.Compatible(id)) return null;

            return m_dialog.FirstOrDefault(d => d.Id == id);
        }

        public TreePart this[DialogTreePartIdentifier id] => GetTree(id);
        public DialogSegment this[DialogSegmentIdentifier id] => GetTree(id)?[id];
    }
}
