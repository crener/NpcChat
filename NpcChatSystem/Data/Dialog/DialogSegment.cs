using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NpcChatSystem.Data.Util;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// Piece of dialog from within a larger conversation
    /// </summary>
    [DebuggerDisplay("{CharacterId}: {Text}")]
    public class DialogSegment : ProjectObject
    {
        /// <summary>
        /// Id of the dialog
        /// </summary>
        public DialogSegmentIdentifier Id { get; }

        /// <summary>
        /// Who is expressing this dialog?
        /// Id of the character
        /// </summary>
        public int CharacterId { get; private set; }

        public string Text
        {
            get
            {
                StringBuilder text = new StringBuilder();

                foreach(IDialogElement element in m_dialogParts)
                {
                    text.Append(element.Text);
                }
                
                return text.ToString();
            }
        }

        public IReadOnlyList<IDialogElement> SegmentParts => m_dialogParts;

        private List<IDialogElement> m_dialogParts = new List<IDialogElement>();

        internal DialogSegment(NpcChatProject project, DialogTreePartIdentifier treeId, int dialogId, int charId)
            : base(project)
        {
            Id = new DialogSegmentIdentifier(treeId, dialogId);
            CharacterId = charId;

            m_dialogParts.Add(new DialogText{Text = "Before "});
            m_dialogParts.Add(new DialogCharacterTrait(Project, charId));
            m_dialogParts.Add(new DialogText{Text = " after"});
        }

        public void ChangeCharacter(int newChar)
        {
            if(CharacterId == newChar) return;

            if(Project.ProjectCharacters.HasCharacter(newChar))
            {
                CharacterId = newChar;
            }
        }
    }
}
