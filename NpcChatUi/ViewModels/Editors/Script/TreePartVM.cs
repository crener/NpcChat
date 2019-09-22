using System;
using System.Collections.ObjectModel;
using NpcChat.Properties;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

namespace NpcChat.ViewModels.Editors.Script
{
    public class TreePartVM : NotificationObject
    {
        public NpcChatProject Project { get; }
        public ObservableCollection<CharacterDialogVM> Speech { get; } = new ObservableCollection<CharacterDialogVM>();
        public DialogTreeBranch DialogTree { get; }

        public TreePartVM(NpcChatProject project, [NotNull] DialogTreeBranch dialogTree)
        {
            Project = project;
            DialogTree = dialogTree;

            if (dialogTree != null)
            {
                dialogTree.OnDialogCreated += added => Speech.Add(new CharacterDialogVM(Project, Project.ProjectDialogs[added]));
                dialogTree.OnDialogCreated += removed =>
                {
                    Speech.Clear();
                    foreach (DialogSegment segment in dialogTree.Dialog)
                        Speech.Add(new CharacterDialogVM(Project, segment));
                };
            }
        }
    }
}
