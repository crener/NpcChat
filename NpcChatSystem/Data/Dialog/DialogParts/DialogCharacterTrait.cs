using System.Diagnostics;
using NpcChatSystem.Data.CharacterData;

namespace NpcChatSystem.Data.Dialog.DialogParts
{
    /// <summary>
    /// Information about a particular character
    /// For example the name or a trait value
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public class DialogCharacterTrait : IDialogElement
    {
        /// <summary>
        /// Text representation of the dialog element
        /// </summary>
        public string Text
        {
            get
            {
                Character? character = NpcChatProject.Characters.GetCharacter(CharacterId);
                if(!character.HasValue) return "<???>";

                if(CharacterTrait == "Name") return character.Value.Name;
                return character.Value.GetTrait(CharacterTrait, null);
            }
        }

        /// <summary>
        /// Id of the character
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Character trait to get information about
        /// </summary>
        public string CharacterTrait { get; set; } = "Name";


        public DialogCharacterTrait(int characterId)
        {
            CharacterId = characterId;
        }
    }
}
