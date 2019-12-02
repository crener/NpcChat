using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChatSystem;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;
using Xceed.Wpf.AvalonDock.Layout;
using LogLevel = NLog.LogLevel;

namespace NpcChat.ViewModels.Panels.ScriptEditor
{
    public class ScriptPanelVM : LayoutDocument, IScriptPanelVM
    {
        public ObservableCollection<TreeBranchVM> Branches { get; } = new ObservableCollection<TreeBranchVM>();

        public DialogTree Tree
        {
            get => m_tree;
            set => SetDialogTree(value);
        }

        public ICommand NewBranchCommand { get; }
        public ICommand ShowScriptDiagram { get; }

        public event Action<IReadOnlyList<TreeBranchVM>> OnVisibleBranchChange;

        private NpcChatProject m_project { get; }
        private DialogTree m_tree;

        public ScriptPanelVM(NpcChatProject project, DialogTreeIdentifier dialog = null)
        {
            Title = "Script Editor";
            ToolTip = "Script Editor";
            CanClose = true;
            m_project = project;

            if (dialog != null) SetDialogTree(dialog);

            NewBranchCommand = new DelegateCommand(() => AddNewBranch(Branches.LastOrDefault()?.DialogBranch?.Id, true));
            ShowScriptDiagram = new DelegateCommand(() => WindowViewModel.Instance.ShowScriptDiagramPanel(Tree));
        }

        /// <summary>
        /// sets the dialog tree to display
        /// </summary>
        /// <param name="dialogTreeId">Dialog Tree ID</param>
        public void SetDialogTree(DialogTreeIdentifier dialogTreeId)
        {
            m_tree = m_project.ProjectDialogs.GetDialog(dialogTreeId);
            m_tree.OnBranchCreated += OnBranchCreated;
            m_tree.OnBranchRemoved += OnBranchRemoved;
            Title = string.IsNullOrWhiteSpace(m_tree.TreeName) ? "Script Editor" : m_tree.TreeName;
            Branches.Clear();

            DialogTreeBranch start = m_tree.GetStart();
            if (start != null) Branches.Add(new TreeBranchVM(m_project, this, start));

            TriggerOnVisibleBranchChange();
            RaisePropertyChanged(nameof(Tree));
        }

        private void OnBranchCreated(DialogTreeBranch obj)
        {

        }

        private void OnBranchRemoved(DialogTreeBranch removed)
        {
            bool remove = false;
            for (int i = 0; i < Branches.Count; i++)
            {
                if (remove)
                {
                    Branches.RemoveAt(i);
                    i--;
                    continue;
                }

                TreeBranchVM branch = Branches[i];
                if (branch.DialogBranch.Id == removed)
                {
                    remove = true;
                    Branches.RemoveAt(i);
                    i--;
                }
            }
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
            if (!m_tree.HasBranch(parentId))
            {
                Logging.Logger.Log(LogLevel.Error, $"Unable to add parent ({parentId}) to new branch ({identifier}) as it's not contained inside the tree ({m_tree.Id})");
            }

            //auto add the new branch to the ui
            if (updateView)
            {
                if (Branches.Count == 0 ||
                    parentId != null && Branches.LastOrDefault()?.DialogBranch?.Id == parentId)
                {
                    Branches.Add(new TreeBranchVM(m_project, this, identifier));
                    TriggerOnVisibleBranchChange();
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

        /// <summary>
        /// Changes the visible branches so that the <see cref="parent"/> is visible with the <see cref="child"/>.
        /// </summary>
        /// <param name="parent">parent branch</param>
        /// <param name="child">child of parent branch</param>
        public void RebaseBranchList(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
        {
            if (!m_project.ProjectDialogs.HasDialog(parent))
            {
                Logging.Logger.Warn($"Attempted to rebase ScriptPanel branches but parent '{parent}' doesn't exist");
                return;
            }
            if (!m_project.ProjectDialogs.HasDialog(child))
            {
                Logging.Logger.Warn($"Attempted to rebase ScriptPanel branches but child '{child}' doesn't exist");
                return;
            }

            if (!m_project[parent].Children.Contains(child))
            {
                Logging.Logger.Warn($"Attempted to rebase ScriptPanel branches but parnet '{parent}' doesn't child '{child}'");
                return;
            }

            int found = -1;
            for (int i = 0; i < Branches.Count; i++)
            {
                if (Branches[i].DialogBranch.Id == parent)
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
            if (m_project[child].Children.Count == 1)
                RebaseBranchList(child, m_project[child].Children[0]);
            else TriggerOnVisibleBranchChange();
        }

        /// <summary>
        /// Changes the visible branches so that the <see cref="parent"/> is last visible 
        /// </summary>
        /// <param name="parent">parent branch</param>
        public void ClearBranchListAfterParent(DialogTreeBranchIdentifier parent)
        {
            bool removeBranches = false; // does next branch need to be removed?
            bool removedBranches = false; // has a branch been removed?
            for (int i = 0; i < Branches.Count; i++)
            {
                if (removeBranches)
                {
                    Branches.RemoveAt(i);
                    removedBranches = true;
                    i--;
                }

                if (Branches[i].DialogBranch == parent)
                {
                    removeBranches = true;
                }
            }

            if (!removeBranches)
                Logging.Logger.Error($"{nameof(ClearBranchListAfterParent)} - Unable to clear, parent '{parent}' is not a visible branch");
            if (removedBranches) TriggerOnVisibleBranchChange();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TriggerOnVisibleBranchChange()
        {
            OnVisibleBranchChange?.Invoke(Branches);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
