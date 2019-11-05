using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using NpcChat.Backend;
using NpcChat.Util;
using NpcChat.ViewModels.Base;
using NpcChat.ViewModels.Editors.Script;
using NpcChat.Views.Dialogs;
using NpcChatSystem;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace NpcChat.ViewModels
{
    public class WindowViewModel : NotificationObject
    {
        /// <summary>
        /// Collection of possible windows. Note this isn't a set of active windows as some may be hidden (ie closed)
        /// </summary>
        public ObservableCollection<DockPanelVM> Windows { get; }

        public ICommand OpenProjectCommand { get; }
        public ICommand NewProjectCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand ForceSaveLayoutCommand { get; }
        public ICommand ForceLoadLayoutCommand { get; }


        private NpcChatProject m_project;
        private DialogTree m_tree;
        private string WorkspaceLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NpcChat", "workspace.xml");
        private DockingManager m_dockingManager;


        public WindowViewModel()
        {
            CurrentProject.Project = m_project = new NpcChatProject();
            if (m_project.ProjectCharacters.RegisterNewCharacter(out int diane, new Character("diane")) &&
               m_project.ProjectCharacters.RegisterNewCharacter(out int jerry, new Character("jerry")) &&
               m_project.ProjectCharacters.RegisterNewCharacter(out int gran, new Character("Granny")) &&
               m_project.ProjectCharacters.RegisterNewCharacter(out int banana, new Character("Banana")))
            {
                m_tree = m_project.ProjectDialogs.CreateNewDialogTree();
                //SetDialogTree(m_tree.Id);
                /*DialogTreeBranch branch = m_tree.GetStart();
                branch.Name = "Start";

                DialogSegment segment = branch.CreateNewDialog(diane);
                DialogSegment segment2 = branch.CreateNewDialog(jerry);

                branch = m_tree.CreateNewBranch();
                branch.Name = "First Branch";
                //Branches.Add(new TreePartVM(m_project, branch));
                DialogSegment segment3 = branch.CreateNewDialog(gran);
                DialogSegment segment4 = branch.CreateNewDialog(banana);*/
            }

            Windows = new ObservableCollection<DockPanelVM>();
            Windows.Add(new ScriptPanelVM(m_project, m_tree));

            {
                //File
                OpenProjectCommand = new DelegateCommand<string>(OpenProject);
                NewProjectCommand = new DelegateCommand(NewProject);

                //About
                ShowAboutCommand = new DelegateCommand(ShowAbout);

                //Debug
                ForceLoadLayoutCommand = new DelegateCommand(LoadLayout);
                ForceSaveLayoutCommand = new DelegateCommand(SaveLayout);
            }
        }

        private void NewProject()
        {
            throw new NotImplementedException();
        }

        private void OpenProject(string path)
        {
            throw new NotImplementedException();
        }

        private void ShowAbout()
        {
            AboutDialog about = new AboutDialog();
            about.ShowDialog();
        }

        public void SetDockingManager(DockingManager dockingManager)
        {
            m_dockingManager = dockingManager;
            m_dockingManager.DocumentClosed += WindowClosed;
        }

        private void WindowClosed(object sender, DocumentClosedEventArgs e)
        {
            Logging.Logger.Info($"Closed window, '{e.Document.Title}'");
        }


        private void LoadLayout()
        {
            if (m_dockingManager == null)
            {
                Logging.Logger.Error("Docking manager is null! Cannot load into null object");
                return;
            }

            using (StreamReader reader = new StreamReader(WorkspaceLocation))
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(m_dockingManager);
                layoutSerializer.Deserialize(reader);
            }
        }

        private void SaveLayout()
        {
            if (m_dockingManager == null)
            {
                Logging.Logger.Error("Docking manager is null! Cannot save into null object");
                return;
            }

            using (StreamWriter writer = new StreamWriter(WorkspaceLocation))
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(m_dockingManager);
                layoutSerializer.Serialize(writer);
            }
        }
    }
}
