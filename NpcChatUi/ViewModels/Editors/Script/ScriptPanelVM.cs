using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using NpcChat.Backend.Interfaces;
using NpcChat.Util;
using NpcChat.ViewModels.Base;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Editors.Script
{
    public class ScriptPanelVM : DockPanelVM, IScriptPanelVM
    {
        public ObservableCollection<TreeBranchVM> Branches { get; } = new ObservableCollection<TreeBranchVM>();

        public ICommand NewBranchCommand { get; }

        private NpcChatProject m_project { get; set; }
        private DialogTree m_tree;

        public ScriptPanelVM(NpcChatProject project, DialogTreeIdentifier dialog = null)
        {
            Title = "Script Editor";
            m_project = project;

            if (dialog != null) SetDialogTree(dialog);

            NewBranchCommand = new DelegateCommand(() => AddNewBranch(Branches.Last()?.DialogTree?.Id, true));
        }

        /// <summary>
        /// sets the dialog tree to display
        /// </summary>
        /// <param name="dialogTreeId">Dialog Tree ID</param>
        public void SetDialogTree(DialogTreeIdentifier dialogTreeId)
        {
            m_tree = m_project.ProjectDialogs.GetDialog(dialogTreeId);
            m_tree.OnBranchCreated += OnBranchCreated;
            Branches.Clear();
            Branches.Add(new TreeBranchVM(m_project, this, m_tree.GetStart()));
            OnVisibleBranchChange?.Invoke(Branches);

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

        /// <summary>
        /// Creates a new branch and links it to the <see cref="parentId"/>
        /// </summary>
        /// <param name="parentId">parent of the new branch</param>
        /// <param name="updateView"></param>
        /// <returns>id of the new tree branch</returns>
        public DialogTreeBranchIdentifier AddNewBranch(DialogTreeBranchIdentifier parentId, bool updateView)
        {
            DialogTreeBranchIdentifier identifier = CreateNewBranch(parentId);

            //auto add the new branch to the ui
            if (updateView)
            {
                if(Branches.Last()?.DialogTree?.Id == parentId)
                {
                    Branches.Add(new TreeBranchVM(m_project, this, identifier));
                    OnVisibleBranchChange?.Invoke(Branches);
                }
                else RebaseBranchList(parentId, identifier);
            }

            return identifier;
        }

        private DialogTreeBranchIdentifier CreateNewBranch(DialogTreeBranchIdentifier parent)
        {
            DialogTreeBranch newBranch = m_tree.CreateNewBranch();
            if (parent != null) newBranch.AddParent(parent);
            return newBranch.Id;
        }

        public void RebaseBranchList(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
        {
            if (!m_project.ProjectDialogs.HasDialog(parent))
            {
                Logging.Logger.Error($"Attempted to rebase ScriptPanel branches but parent '{parent}' doesn't exist");
                return;
            }
            if (!m_project.ProjectDialogs.HasDialog(child))
            {
                Logging.Logger.Error($"Attempted to rebase ScriptPanel branches but child '{child}' doesn't exist");
                return;
            }

            if (!m_project[parent].Children.Contains(child))
            {
                Logging.Logger.Error($"Attempted to rebase ScriptPanel branches but parnet '{parent}' doesn't child '{child}'");
                return;
            }

            int found = -1;
            for (int i = 0; i < Branches.Count; i++)
            {
                if (Branches[i].DialogTree.Id == parent)
                {
                    found = i;
                    break;
                }
            }

            for (int i = Branches.Count - 1; i > found; i--)
            {
                Branches.RemoveAt(i);
            }

            Branches.Add(new TreeBranchVM(m_project, this, child));
            OnVisibleBranchChange?.Invoke(Branches);
        }


        public event Action<IReadOnlyList<TreeBranchVM>> OnVisibleBranchChange;
    }
}
