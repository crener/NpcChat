using System.Collections.Generic;
using System.Collections.ObjectModel;
using NpcChat.Backend;
using NpcChat.Util;
using NpcChat.ViewModels.Editors.Script;
using NpcChatSystem;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

namespace NpcChat.ViewModels
{
    public class WindowViewModel : NotificationObject
    {
        public ObservableCollection<TreePartVM> Branches => m_branches;

        private NpcChatProject m_project;
        private ObservableCollection<TreePartVM> m_branches = new ObservableCollection<TreePartVM>();
        private DialogTree m_tree;

        public WindowViewModel()
        {
            CurrentProject.Project = m_project = new NpcChatProject();
            if (m_project.ProjectCharacters.RegisterNewCharacter(out int diane, new Character("diane")) &&
                m_project.ProjectCharacters.RegisterNewCharacter(out int jerry, new Character("jerry")))
            {
                m_tree = m_project.ProjectDialogs.CreateNewDialogTree();
                SetDialogTree(m_tree.Id);
                DialogTreeBranch branch = m_tree.GetStart();

                DialogSegment segment = branch.CreateNewDialog(diane);
                //Speech.Add(new CharacterDialogVM(m_project, segment));
                DialogSegment segment2 = branch.CreateNewDialog(jerry);
                //Speech.Add(new CharacterDialogVM(m_project, segment2));
            }
        }

        public void SetDialogTree(DialogTreeIdentifier dialogTreeId)
        {
            m_tree = m_project.ProjectDialogs.GetDialog(dialogTreeId);
            m_tree.BranchCreated += OnBranchCreated;
            Branches.Clear();
            Branches.Add(new TreePartVM(m_project, m_tree.GetStart()));

            /*List<CharacterDialogVM> tempList = new List<CharacterDialogVM>();
            foreach (DialogSegment segment in part.Dialog)
            {
                tempList.Add(new CharacterDialogVM(m_project, segment));
            }*/

            //Speech.Clear();
            //Speech.AddRange(tempList);
        }

        private void OnBranchCreated(DialogTreeBranch obj)
        {
            
        }
    }
}
