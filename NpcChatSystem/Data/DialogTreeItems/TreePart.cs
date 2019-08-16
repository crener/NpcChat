using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcChatSystem.Data.DialogTreeItems
{
    public class TreePart
    {
        public int DialogTreeId { get; }

        public object Condition { get; set; }
        public IReadOnlyCollection<DialogSegment> Dialog => m_dialog;

        private readonly List<DialogSegment> m_dialog = new List<DialogSegment>();

        internal TreePart(int dialogId)
        {
            DialogTreeId = dialogId;
        }

        public DialogSegment CreateNewDialog(int characterId = -1)
        {
            DialogSegment dialog = new DialogSegment(m_dialog.Count + 1, characterId);
            m_dialog.Add(dialog);
            return dialog;
        }
    }
}
