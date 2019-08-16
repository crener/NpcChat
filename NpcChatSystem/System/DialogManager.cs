﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data;

namespace NpcChatSystem.System
{
    public class DialogManager
    {
        public IReadOnlyCollection<int> DialogTreeIds => m_dialogs.Select(d => d.Id).ToArray();
        List<DialogTree> m_dialogs = new List<DialogTree>();
        
        /// <summary>
        /// Find specific dialog by id
        /// </summary>
        /// <param name="id">id of the character</param>
        /// <returns>character if found, null if not found</returns>
        public DialogTree GetDialog(int id)
        {
            return m_dialogs.FirstOrDefault(c => c.Id == id);
        }

        public DialogTree CreateNewDialogTree()
        {
            int id = GenerateUniqueId();

            DialogTree tree = new DialogTree(id);
            m_dialogs.Add(tree);
            return tree;
        }

        public bool RemoveDialog(DialogTree dialog)
        {
            if(dialog.Id == 0) return false;

            if(m_dialogs.Contains(dialog))
            {
                m_dialogs.Remove(dialog);
            }

            return true;
        }

        public bool RemoveDialog(int id)
        {
            if(id == 0) return false;

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
    }
}