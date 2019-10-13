using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Util;
using NpcChatSystem.System.TypeStore;

namespace NpcChatSystem.Data.Dialog.DialogParts
{
    /// <summary>
    /// Information about a particular character
    /// For example the name or a trait value
    /// </summary>
    [DebuggerDisplay("{Text}"), Export(typeof(IDialogElement)), NiceTypeName(c_elementName)]
    public class DialogCharacterTrait : ProjectNotificationObject, IDialogElement
    {
        private const string c_elementName = "Character Trait";
        private const string c_traitFallback = "<???>";
        public string ElementName => c_elementName;

        /// <summary>
        /// Text representation of the dialog element
        /// </summary>
        public string Text
        {
            get
            {
                Character? character = m_project?.ProjectCharacters.GetCharacter(CharacterId);
                if (!character.HasValue) return c_traitFallback;

                if (CharacterTrait == "Name") return character.Value.Name;
                return character.Value.GetTrait(CharacterTrait, c_traitFallback);
            }
        }

        /// <summary>
        /// Id of the character
        /// </summary>
        public int CharacterId
        {
            get => m_characterId;
            set
            {
                m_characterId = value;

                Character? character = m_project?.ProjectCharacters[CharacterId];
                if(character.HasValue)
                {
                    CharacterProperties = character.Value.TraitNames;
                }

                RaiseChanged();
            }
        }

        public IEnumerable<string> CharacterProperties
        {
            get => m_characterProperties;
            set
            {
                m_characterProperties = value;
                RaiseChanged();
            }
        }

        /// <summary>
        /// Character trait to get information about
        /// </summary>
        public string CharacterTrait
        {
            get => m_characterTrait;
            set
            {
                m_characterTrait = value;
                RaiseChanged();
            }
        }

        private int m_characterId;
        private string m_characterTrait = "Name";
        private IEnumerable<string> m_characterProperties;

        public DialogCharacterTrait(NpcChatProject project, int characterId = -1)
            : base(project)
        {
            CharacterId = characterId;
        }
    }
}
