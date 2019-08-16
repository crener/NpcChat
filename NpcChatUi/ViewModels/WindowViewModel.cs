using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChat.Util;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.DialogTreeItems;

namespace NpcChat.ViewModels
{
    class WindowViewModel : NotificationObject
    {
        public int TreeId
        {
            get => treeId;
            set
            {
                SetDialogTree(value);
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<DialogSegment> Speech
        {
            get => m_speech;
            /*set
            {
                m_speech = value;
                RaisePropertyChanged();
            }*/
        }

        private ObservableCollection<DialogSegment> m_speech = new ObservableCollection<DialogSegment>();
        private DialogTree m_tree;

        public void SetDialogTree(int dialogTreeId)
        {
            m_tree = NpcChatProject.Dialogs.GetDialog(dialogTreeId);
            TreePart part = m_tree.GetStart();

            Speech.Clear();
            Speech.AddRange(part.Dialog);
        }


        private int treeId;

        public WindowViewModel()
        {
            NpcChatProject project = new NpcChatProject();
            if (project.ProjectCharacters.RegisterNewCharacter(out int diane, new Character("diane")) &&
                project.ProjectCharacters.RegisterNewCharacter(out int jerry, new Character("jerry")))
            {
                DialogTree dialog = project.ProjectDialogs.CreateNewDialogTree();
                TreePart branch = dialog.CreateNewBranch();
                DialogSegment segment = branch.CreateNewDialog(diane);
                DialogSegment segment2 = branch.CreateNewDialog(jerry);

                TreeId = branch.DialogTreeId;
            }
        }

    }
}
