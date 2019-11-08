using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using NpcChatSystem;
using Prism.Commands;
using Xceed.Wpf.AvalonDock.Layout;

namespace NpcChat.ViewModels.Panels.Project
{
    public class ProjectOverviewVM : LayoutDocument
    {
        //characters
        public int CharacterCount => m_project.ProjectCharacters.AvailableCharacters().Count;
        public ObservableCollection<CharacterOverview> CharacterStats { get; } = new ObservableCollection<CharacterOverview>();
        public ICommand RefreshCharacterDataCommand { get; }
        
        public int DialogTreeCount => m_project.ProjectDialogs.DialogTreeIds.Count;
        public ObservableCollection<DialogOverview> DialogStats { get; } = new ObservableCollection<DialogOverview>();
        public ICommand RefreshDialogDataCommand { get; }


        private NpcChatProject m_project { get; }

        public ProjectOverviewVM(NpcChatProject project)
        {
            m_project = project;
            Title = "Project Overview";

            project.ProjectCharacters.CharacterAdded += id => RaisePropertyChanged(nameof(CharacterCount));
            project.ProjectCharacters.CharacterRemoved += id => RaisePropertyChanged(nameof(CharacterCount));
            RefreshCharacterDataCommand = new DelegateCommand(RefreshCharacterData);
            RefreshCharacterDataCommand.Execute(null); // force update

            project.ProjectDialogs.OnDialogTreeAdded += id => RaisePropertyChanged(nameof(DialogTreeCount));
            project.ProjectDialogs.OnDialogTreeRemoved += id => RaisePropertyChanged(nameof(DialogTreeCount));
            RefreshDialogDataCommand = new DelegateCommand(RefreshDialogData);
            RefreshDialogDataCommand.Execute(null); // force update
        }

        private void RefreshCharacterData()
        {
            CharacterStats.Clear();
            CharacterStats.AddRange(CharacterOverview.AnalyseProject(m_project));
        }

        private void RefreshDialogData()
        {
            DialogStats.Clear();
            DialogStats.AddRange(DialogOverview.AnalyseProject(m_project));
        }
    }
}
