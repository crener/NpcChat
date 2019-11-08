using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using NpcChat.Views;
using NpcChatSystem;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.System;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.ViewModels.Panels.Project
{
    public class DialogOverview : INotifyPropertyChanged
    {
        public string Name => m_tree.TreeName;
        public ICommand ShowEditorCommand { get; }

        public int DialogCount
        {
            get => m_dialogCount;
            private set
            {
                m_dialogCount = value;
                OnPropertyChanged();
            }
        }
        public int BranchCount
        {
            get => m_branchCount;
            private set
            {
                m_branchCount = value;
                OnPropertyChanged();
            }
        }

        private NpcChatProject m_project;
        private DialogTree m_tree;
        private int m_dialogCount, m_branchCount;

        private DialogOverview(NpcChatProject project, DialogTreeIdentifier tree)
        {
            m_project = project;
            m_tree = project[tree];

            ShowEditorCommand = new DelegateCommand(ShowScriptEditor);

            UpdateUsageStatistics();
        }

        private void UpdateUsageStatistics()
        {
            if(m_tree == null) return;
            m_dialogCount = m_branchCount = 0;

            foreach (DialogTreeBranchIdentifier branchId in m_tree.Branches)
            {
                DialogTreeBranch branch = m_project[branchId];
                if(branch == null) continue;

                DialogCount += branch.Dialog.Count;
                BranchCount++;
            }
        }

        private void ShowScriptEditor()
        {
            WindowViewModel.Instance.ShowScriptEditorPanel(m_tree);
        }

        public static IEnumerable<DialogOverview> AnalyseProject(NpcChatProject project)
        {
            Logging.Logger.Info("Gathering Dialog statistics from current project");
            List<DialogOverview> lookup = new List<DialogOverview>();

            foreach (DialogTreeIdentifier id in project.ProjectDialogs.DialogTreeIds)
            {
                DialogTree possibleTree = project[id];
                if (possibleTree == null)
                {
                    Logging.Logger.Error($"Unable to find dialog tree '{id}' but this retrieved from {nameof(DialogManager)}.{nameof(DialogManager.DialogTreeIds)} so should be availible");
                    continue;
                }

                DialogOverview overview = new DialogOverview(project, possibleTree);
                lookup.Add(overview);
            }

            Logging.Logger.Info("Dialog project statistics done!");
            return lookup;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
