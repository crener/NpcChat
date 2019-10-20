using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Editors.Script.Util
{
    public class TreeBranchLinkInfoVM : INotifyPropertyChanged
    {
        public DialogTreeBranchIdentifier Parent { get; }
        public DialogTreeBranchIdentifier Child { get; }
        public string ChildName => m_project[Child].Name;

        public ICommand RebaseScriptView { get; }

        private NpcChatProject m_project { get; }

        public TreeBranchLinkInfoVM(NpcChatProject project, ScriptPanelVM script,
            DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
        {
            m_project = project;
            Parent = parent;
            Child = child;

            if (!project[parent].Children.Contains(child))
            {
                Logging.Logger.Warn($"ScriptPanel - Tree Branch link created with a parent '{parent}' that doesn't have the child '{child}' as a Child relationship");
                //todo should this throw?
            }

            project[child].PropertyChanged += OnChanged;
            RebaseScriptView = new DelegateCommand(() =>
            {
                if(Parent == Child)
                {
                    Logging.Logger.Error($"ScriptPanel - Tree Branch link invalid, parent and child identicle '{Parent}'");
                    return;
                }

                script?.RebaseBranchList(Parent, Child);
            });
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
