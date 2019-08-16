using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data.DialogParts;
using NpcChatSystem.Data.Util;

namespace NpcChatSystem.Data
{
    /// <summary>
    /// Piece of dialog from within a larger conversation
    /// </summary>
    [DebuggerDisplay("{CharacterId}: {Text}")]
    public class DialogSegment
    {
        public int DialogId { get; }
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

        internal DialogSegment(int dialogId, int charId)
        {
            CharacterId = charId;
            DialogId = dialogId;

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
