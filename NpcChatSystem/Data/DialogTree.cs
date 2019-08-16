using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NpcChatSystem.Data.DialogTreeItems;

namespace NpcChatSystem.Data
{
    public class DialogTree
    {
        private static Random s_random = new Random();

        public int Id { get; }
        private List<TreePart> m_dialog = new List<TreePart>();

        internal DialogTree(int id)
        {
            Id = id;
        }

        public TreePart CreateNewBranch()
        {
            TreePart part = new TreePart(Id);
            m_dialog.Add(part);
            return part;
        }

        public TreePart GetStart()
        {
            return m_dialog.FirstOrDefault();
        }
    }
}
