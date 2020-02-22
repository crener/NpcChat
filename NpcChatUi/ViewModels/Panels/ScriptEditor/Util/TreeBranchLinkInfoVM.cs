using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChat.Properties;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.ScriptEditor.Util
{
    /// <summary>
    /// Link between a parent and child branch
    /// </summary>
    public class TreeBranchLinkInfoVM : INotifyPropertyChanged
    {
        public DialogTreeBranchIdentifier Parent { get; }
        public DialogTreeBranchIdentifier Child { get; }
        public string ChildName => m_project[Child].Name;

        public ICommand RebaseScriptView { get; }
        public ICommand RemoveLinkCommand { get; }

        private readonly NpcChatProject m_project;
        private readonly IScriptPanelVM m_script;

        public TreeBranchLinkInfoVM([NotNull] NpcChatProject project, [NotNull] IScriptPanelVM script,
            DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
        {
            m_project = project;
            m_script = script;
            Parent = parent;
            Child = child;

            if (!project[parent].Children.Contains(child))
            {
                Logging.Logger.Warn($"Tree Branch link created with a parent '{parent}' that doesn't have the child '{child}' as a Child relationship");
                //todo should this throw?
            }

            m_project[child].PropertyChanged += OnChanged;
            RebaseScriptView = new DelegateCommand(() =>
            {
                if (Parent == Child)
                {
                    Logging.Logger.Warn($"Tree Branch link invalid, parent and child identicle '{Parent}'");
                    return;
                }

                m_script.RebaseBranchList(Parent, Child);
            });
            RemoveLinkCommand = new DelegateCommand(RemoveLink);
        }

        private void RemoveLink()
        {
            DialogTreeBranch parentBranch = m_project[(DialogTreeBranchIdentifier)Parent];
            if (parentBranch == null)
            {
                Logging.Logger.Warn($"Unable to remove branch link as parent '{Parent}' couldn't be found");
                return;
            }

            if(!parentBranch.RemoveChild(Child))
                Logging.Logger.Warn($"Failed to remove link between '{Parent}' and '{Child}'");
            else m_script.ClearBranchListAfterParent(Parent);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(DialogTreeBranch.Name))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChildName)));
            }
        }
    }
}
