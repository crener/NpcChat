using System.Diagnostics;
using NpcChatSystem.Data.Util;

namespace NpcChatSystem.Data.DialogParts
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
                Character character = NpcChatProject.Characters.GetCharacter(CharacterId);
                if(CharacterTrait == "Name") return character.Name;
                return character.GetTrait(CharacterTrait, null);
            }
        }

        /// <summary>
        /// Id of the character
        /// </summary>
        public int CharacterId { get; }

        /// <summary>
        /// Character trait to get information about
        /// </summary>
        public string CharacterTrait { get; } = "Name";


        public DialogCharacterTrait(int characterId)
        {
            CharacterId = characterId;
        }
    }
}
