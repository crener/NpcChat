using System.Windows;
using NpcChat.Util;
using NpcChat.Views.Editors.Reusable;
using NpcChatSystem;

namespace NpcChat.ViewModels.Editors.Reusable
{
    public class CharacterLabelViewModel : DependencyNotificationObject
    {
        public NpcChatProject Project
        {
            get => project;
            set
            {
                project = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CharacterName));
            }
        }

        public int CharacterId
        {
            get => characterId;
            set
            {
                characterId = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CharacterName));
            }
        }

        public string CharacterName
        {
            get
            {
                if (Project == null) return "No Project";
                if (Project.ProjectCharacters == null) return "No Character manager!";

                return Project.ProjectCharacters.GetCharacter(CharacterId)?.Name ?? "Unknown";
            }
        }

        private NpcChatProject project = null;
        private int characterId = -1;
    }
}
