using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using NpcChatSystem.Data.Util;
using NpcChatSystem.System;
using NpcChatSystem.System.TypeStore;
using NotImplementedException = System.NotImplementedException;

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
        private const string c_traitName = nameof(Character.Character.Name);
        private const string c_traitFallback = "<???>";

        public bool AllowsInspection => true;
        public string ElementName => c_elementName;

        /// <summary>
        /// Text representation of the dialog element
        /// </summary>
        public string Text
        {
            get
            {
                Character.Character character = m_project?.ProjectCharacters.GetCharacter(CharacterId);
                if (character == null) return c_traitFallback;

                if (CharacterTrait == c_traitName) return character.Name;
                return character.GetTrait(CharacterTrait, c_traitFallback);
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

                Character.Character character = m_project?.ProjectCharacters[CharacterId];
                if (character != null)
                {
                    CharacterProperties = character.TraitNames;
                }

                RaisePropertyChanged();
            }
        }

        public IEnumerable<string> CharacterProperties
        {
            get => m_characterProperties;
            set
            {
                m_characterProperties = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged();
            }
        }

        private int m_characterId;
        private string m_characterTrait = "Name";
        private IEnumerable<string> m_characterProperties;

        public DialogCharacterTrait(NpcChatProject project, int characterId = -1)
            : base(project)
        {
            CharacterId = characterId;

            if(project != null)
            {
                project.ProjectCharacters.CharacterChanged += (id, changed) =>
                {
                    if(CharacterId == id) RaisePropertyChanged(nameof(Text));
                };
            }
        }

        public bool Edit(string source, string edit)
        {
            if (source == edit) return true;

            Character.Character character = m_project?.ProjectCharacters.GetCharacter(CharacterId);
            if (character == null) return false;

            if (CharacterTrait == c_traitName)
            {
                if (source != character.Name)
                {
                    // source is outdated! the suggestion may be wrong
                    return false;
                }

                //todo add "are you sure?" message box

                character.Name = edit;
                return true;
            }

            // any none name trait value will depend on where the value came from. I.e. if it's dynamically generated or not.
            throw new NotImplementedException("This needs to be implemented once the character trait system is in place");
        }

        public bool SuggestedEdit(string source, string edit)
        {
            return Edit(source, edit);
        }
    }
}
