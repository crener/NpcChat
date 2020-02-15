using System;
using System.Collections.Generic;
using System.Linq;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;

namespace NpcChatSystem.System
{
    public class DialogManager : ProjectObject
    {
        public IReadOnlyCollection<DialogTreeIdentifier> DialogTreeIds => m_dialogs.Select(d => d.Id).ToArray();

        public event Action<DialogTreeIdentifier> OnDialogTreeAdded;
        public event Action<DialogTreeIdentifier> OnDialogTreeRemoved;

        private List<DialogTree> m_dialogs = new List<DialogTree>();

        internal DialogManager(NpcChatProject project)
            : base(project)
        {
        }

        /// <summary>
        /// Find specific dialog by id
        /// </summary>
        /// <param name="id">id of the character</param>
        /// <returns>character if found, null if not found</returns>
        public DialogTree GetDialog(int id)
        {
            return m_dialogs.FirstOrDefault(c => c.Id.DialogTreeId == id);
        }

        /// <summary>  
        /// Find specific dialog by id
        /// </summary>
        /// <param name="id">id of the character</param>
        /// <returns>character if found, null if not found</returns>
        public DialogTree GetDialog(DialogTreeIdentifier id)
        {
            if (id == null) return null;
            return GetDialog(id.DialogTreeId);
        }

        public DialogTree CreateNewDialogTree()
        {
            int id = GenerateUniqueId();

            DialogTree tree = new DialogTree(m_project, id);
            {
                string testName = "Dialog Tree";
                int i = 0;
                while (m_dialogs.Any(t => t.TreeName == testName) && i++ <= 9999)
                    testName = $"Dialog Tree ({i})";
                tree.TreeName = testName;
            }

            m_dialogs.Add(tree);
            OnDialogTreeAdded?.Invoke(tree);
            return tree;
        }

        public bool RemoveDialog(DialogTree dialog)
        {
            if (dialog.Id.DialogTreeId == CharacterId.DefaultId) return false;

            if (m_dialogs.Contains(dialog))
            {
                m_dialogs.Remove(dialog);
                OnDialogTreeRemoved?.Invoke(dialog);
            }

            return true;
        }

        public bool RemoveDialog(int id)
        {
            if (id == 0) return false;

            DialogTree dialog = GetDialog(id);
            return RemoveDialog(dialog);
        }

        private int GenerateUniqueId()
        {
            Random random = new Random();
            do
            {
                int i = random.Next(1, int.MaxValue);
                if (GetDialog(i) == null) //dialog Id should always be 0 before being reassigned
                    return i;
            } while (true);
        }

        public DialogTree this[DialogTreeIdentifier id] => GetDialog(id);
        public DialogTreeBranch this[DialogTreeBranchIdentifier id] => GetDialog(id)?[id];
        public DialogSegment this[DialogSegmentIdentifier id] => GetDialog(id)?[id];

        public bool HasDialog(DialogTreeIdentifier id)
        {
            DialogTree tree = this[id];
            return tree != null;
        }

        public bool HasDialog(DialogTreeBranchIdentifier id)
        {
            DialogTreeBranch branch = this[id];
            return branch != null;
        }

        public bool HasDialog(DialogSegmentIdentifier id)
        {
            DialogSegment dialogSegment = this[id];
            return dialogSegment != null;
        }
    }
}
