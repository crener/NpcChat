using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NpcChat.Properties;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using Prism.Commands;

namespace NpcChat.ViewModels.Editors.Script
{
    public class TreePartVM : NotificationObject
    {
        public NpcChatProject Project { get; }
        public ObservableCollection<CharacterDialogVM> Speech { get; } = new ObservableCollection<CharacterDialogVM>();
        public DialogTreeBranch DialogTree { get; }

        public int NewDialogCharacterId
        {
            get
            {
                if (!m_newDialogCharacterId.HasValue)
                {
                    if (Speech.Count == 0)
                    {
                        IList<CharacterId> characters = Project.ProjectCharacters.AvailableCharacters();
                        if (characters.Count == 0) return CharacterId.DefaultId;
                        return characters[0].Id;
                    }
                    return Speech.Last().CharacterId;
                }

                return m_newDialogCharacterId.Value;
            }
            set
            {
                m_newDialogCharacterId = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(AddNewDialogCommand));
            }
        }

        public ICommand AddNewDialogCommand => m_addNewDialogCommand;

        private int? m_newDialogCharacterId = null;
        private DelegateCommand m_addNewDialogCommand;

        public TreePartVM(NpcChatProject project, [NotNull] DialogTreeBranch dialogTree)
        {
            Project = project;
            DialogTree = dialogTree;

            if (dialogTree != null)
            {
                dialogTree.OnDialogCreated += added =>
                {
                    if(!Project.ProjectDialogs.HasDialog(added)) return;
                    Speech.Add(new CharacterDialogVM(Project, Project.ProjectDialogs[added]));
                };
                dialogTree.OnDialogDestroyed += removed =>
                {
                    Speech.Clear();
                    foreach (DialogSegment segment in dialogTree.Dialog)
                        Speech.Add(new CharacterDialogVM(Project, segment));
                };
            }

            m_addNewDialogCommand = new DelegateCommand(() =>
            {
                DialogTree.CreateNewDialog(NewDialogCharacterId);
            }, () => NewDialogCharacterId != CharacterId.DefaultId);
        }
    }
}
