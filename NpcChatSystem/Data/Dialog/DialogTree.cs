using System;
using System.Collections.Generic;
using System.Linq;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NpcChatSystem.Data.Util;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// A branching tree following all options within a conversation
    /// </summary>
    public class DialogTree : ProjectObject
    {
        private static Random s_random = new Random();

        public DialogTreeIdentifier Id { get; }
        private List<DialogTreeBranch> m_dialog = new List<DialogTreeBranch>();

        public event Action<DialogTreeBranch> BranchCreated;

        internal DialogTree(NpcChatProject project, int id)
            : base(project)
        {
            Id = new DialogTreeIdentifier(id);
            DialogTreeBranch branch = CreateNewBranch();
            branch.isTreeRoot = true;
        }

        public DialogTreeBranch CreateNewBranch()
        {
            int id;
            do
            {
                id = s_random.Next(1, int.MaxValue);
            } while (m_dialog.Any(d => d.Id.DialogTreeId == id));

            DialogTreeBranch branch = new DialogTreeBranch(m_project, Id, id);
            m_dialog.Add(branch);

            BranchCreated?.Invoke(branch);

            return branch;
        }

        public DialogTreeBranch GetStart()
        {
            return m_dialog.First();
        }

        public DialogTreeBranch GetTree(DialogTreePartIdentifier id)
        {
            if (!Id.Compatible(id)) return null;

            return m_dialog.FirstOrDefault(d => d.Id == id);
        }

        public DialogTreeBranch this[DialogTreePartIdentifier id] => GetTree(id);
        public DialogSegment this[DialogSegmentIdentifier id] => GetTree(id)?[id];

        public static implicit operator DialogTreeIdentifier(DialogTree d) => d.Id;

    }
}
