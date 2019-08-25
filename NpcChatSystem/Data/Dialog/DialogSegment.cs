using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

namespace NpcChatSystem.Data.Dialog
{
    /// <summary>
    /// Piece of dialog from within a larger conversation
    /// </summary>
    [DebuggerDisplay("{CharacterId}: {Text}")]
    public class DialogSegment
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

                foreach(IDialogElement element in dialogParts)
                {
                    text.Append(element.Text);
                }
                
                return text.ToString();
            }
        }

        private List<IDialogElement> dialogParts = new List<IDialogElement>();

        internal DialogSegment(DialogTreePartIdentifier treeId, int dialogId, int charId)
        {
            Id = new DialogSegmentIdentifier(treeId, dialogId);
            CharacterId = charId;

            dialogParts.Add(new DialogText{Text = "Before "});
            dialogParts.Add(new DialogCharacterTrait(charId));
            dialogParts.Add(new DialogText{Text = " after"});
        }

        public void ChangeCharacter(int newChar)
        {
            if(CharacterId == newChar) return;

            if(NpcChatProject.Characters.HasCharacter(newChar))
            {
                CharacterId = newChar;
            }
        }
    }
}
