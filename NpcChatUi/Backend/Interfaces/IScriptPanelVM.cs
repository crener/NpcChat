using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NpcChat.ViewModels.Editors.Script;
using NpcChatSystem.Identifiers;

namespace NpcChat.Backend.Interfaces
{
    public interface IScriptPanelVM : INotifyPropertyChanged
    {
        ObservableCollection<TreeBranchVM> Branches { get; }
        ICommand NewBranchCommand { get; }

        /// <summary>
        /// sets the dialog tree to display
        /// </summary>
        /// <param name="dialogTreeId">Dialog Tree ID</param>
        void SetDialogTree(DialogTreeIdentifier dialogTreeId);

        /// <summary>
        /// Creates a new branch and links it to the <see cref="parentId"/>
        /// </summary>
        /// <param name="parentId">parent of the new branch</param>
        /// <param name="updateView"></param>
        /// <returns>id of the new tree branch</returns>
        DialogTreeBranchIdentifier AddNewBranch(DialogTreeBranchIdentifier parentId, bool updateView);
        
        /// <summary>
        /// Changes the visible branches so that the <see cref="parent"/> is visible with the <see cref="child"/>.
        /// </summary>
        /// <param name="parent">parent branch</param>
        /// <param name="child">child of parent branch</param>
        void RebaseBranchList(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child);

        /// <summary>
        /// Changes the visible branches so that the <see cref="parent"/> is last visible 
        /// </summary>
        /// <param name="parent">parent branch</param>
        void ClearBranchListAfterParent(DialogTreeBranchIdentifier parent);

        /// <summary>
        /// Triggered when the visible branches in <see cref="Branches"/> change
        /// </summary>
        event Action<IReadOnlyList<TreeBranchVM>> OnVisibleBranchChange;
    }
}
