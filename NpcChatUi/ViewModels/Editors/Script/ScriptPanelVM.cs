using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NpcChat.Util;
using NpcChat.ViewModels.Base;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;

namespace NpcChat.ViewModels.Editors.Script
{
    public class ScriptPanelVM : DockPanelVM
    {
        public ObservableCollection<TreePartVM> Branches => m_branches;

        private NpcChatProject m_project { get; set; }
        private ObservableCollection<TreePartVM> m_branches = new ObservableCollection<TreePartVM>();
        private DialogTree m_tree;

        public ScriptPanelVM(NpcChatProject project, DialogTreeIdentifier dialog = null)
        {
            Title = "Script Editor";
            m_project = project;

            if (dialog != null) SetDialogTree(dialog);
        }

        /// <summary>
        /// sets the dialog tree to display
        /// </summary>
        /// <param name="dialogTreeId">Dialog Tree ID</param>
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
