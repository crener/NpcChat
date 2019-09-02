using System.Collections.Generic;
using System.Collections.ObjectModel;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data.CharacterData;

namespace NpcChat.ViewModels.Reusable
{
    public class CharacterSelectorModel : NotificationObject
    {
        public ObservableCollection<CharacterId> Names { get; }

        public int SelectedCharacter
        {
            get => m_selectedCharacter;
            set
            {
                m_selectedCharacter = value;
                RaisePropertyChanged();
            }
        }

        private readonly NpcChatProject m_project;
        private int m_selectedCharacter;

        public CharacterSelectorModel(NpcChatProject project)
        {
            m_project = project;
            Names = new ObservableCollection<CharacterId>();
            UpdateCharacters(0);

            project.ProjectCharacters.CharacterAdded += UpdateCharacters;
            project.ProjectCharacters.CharacterRemoved += UpdateCharacters;
        }

        private void UpdateCharacters(int charId)
        {
            IList<CharacterId> nameIds = m_project?.ProjectCharacters.AvailableCharacters();
            if(nameIds == null) return;

            Names.Clear();
            Names.AddRange(nameIds);
        }
    }
}
