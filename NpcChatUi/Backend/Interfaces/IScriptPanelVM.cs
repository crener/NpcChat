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

        void RebaseBranchList(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child);

        /// <summary>
        /// Triggered when the visible branches in <see cref="Branches"/> change
        /// </summary>
        event Action<IReadOnlyList<TreeBranchVM>> OnVisibleBranchChange;
    }
}
