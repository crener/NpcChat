using Microsoft.Xaml.Behaviors.Core;
using NpcChat.Util;
using NpcChat.ViewModels.Reusable;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

namespace NpcChat.ViewModels.Editors.Script
{
    public class CharacterDialogModel : NotificationObject
    {
        public NpcChatProject Project { get; }
        public DialogSegment DialogSegment
        {
            get => m_dialogSegment;
            set
            {
                m_dialogSegment = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DialogSegmentId));
                RaisePropertyChanged(nameof(CharacterName));
            }
        }

        public DialogSegmentIdentifier DialogSegmentId
        {
            get => m_dialogSegment?.Id ?? null;
            set => RetrieveDialog(value);
        }

        public string CharacterName
        {
            get
            {
                if (Project?.ProjectCharacters == null) return "No Project";
                return Project.ProjectCharacters.GetCharacter(DialogSegment?.CharacterId ?? -1)?.Name ?? "Unknown";
            }
        }

        public CharacterSelectorModel SelectorModel { get; }

        private DialogSegment m_dialogSegment = null;

        public CharacterDialogModel(NpcChatProject project, DialogSegment dialog)
        {
            Project = project;
            m_dialogSegment = dialog;
            SelectorModel = new CharacterSelectorModel(project);
        }

        private void RetrieveDialog(DialogSegmentIdentifier dialogId)
        {
            DialogSegment tree = Project?.ProjectDialogs[dialogId];
            if(tree == null) return;

            m_dialogSegment = tree;
        }
    }
}
