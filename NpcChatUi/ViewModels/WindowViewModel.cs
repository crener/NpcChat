using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using NpcChat.Backend;
using NpcChat.Util;
using NpcChat.ViewModels.Panels.Project;
using NpcChat.ViewModels.Panels.ScriptDiagram;
using NpcChat.ViewModels.Panels.ScriptEditor;
using NpcChat.ViewModels.Panels.UtilityPanels;
using NpcChat.Views.About;
using NpcChat.Views.Dialogs;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace NpcChat.ViewModels
{
    public class WindowViewModel : NotificationObject
    {
        public static WindowViewModel Instance { get; private set; }

        /// <summary>
        /// Collection of possible windows. Note this isn't a set of active windows as some may be hidden (ie closed)
        /// </summary>
        public ObservableCollection<LayoutContent> Windows { get; }
        public IReadOnlyCollection<KeyValuePair<string, string>> RecentProjects { get; private set; }

        public ICommand OpenProjectCommand { get; }
        public ICommand NewProjectCommand { get; }
        public ICommand SaveProjectCommand { get; }
        public ICommand ShowAboutCommand { get; }
        public ICommand ForceSaveLayoutCommand { get; }
        public ICommand ForceLoadLayoutCommand { get; }
        public ICommand ShowWindowCommand { get; }

        private NpcChatProject m_project;
        private string WorkspaceLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NpcChat", "workspace.xml");
        private DockingManager m_dockingManager;


        public WindowViewModel()
        {
            Instance = this;
            CurrentProject.Project = m_project = new NpcChatProject();
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();

            if (m_project.ProjectCharacters.RegisterNewCharacter(out int diane, "diane") &&
               m_project.ProjectCharacters.RegisterNewCharacter(out int jerry, "jerry") &&
               m_project.ProjectCharacters.RegisterNewCharacter(out int gran, "Granny") &&
               m_project.ProjectCharacters.RegisterNewCharacter(out int banana, "Banana"))
            {
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

            FindRecentProjects();
            Windows = new ObservableCollection<LayoutContent>();
            Windows.Add(new ScriptPanelVM(m_project, tree));

            {
                //File
                OpenProjectCommand = new DelegateCommand<string>(OpenProject);
                NewProjectCommand = new DelegateCommand(NewProject);
                SaveProjectCommand = new DelegateCommand<string>(SaveAs);

                //View
                ShowWindowCommand = new DelegateCommand<Type>(ShowWindow);

                //About
                ShowAboutCommand = new DelegateCommand(ShowAbout);

                //Debug
                ForceLoadLayoutCommand = new DelegateCommand(LoadLayout);
                ForceSaveLayoutCommand = new DelegateCommand(SaveLayout);
            }
        }

        private void ShowWindow(Type type)
        {
            LayoutContent panel = Windows.FirstOrDefault(p => p.GetType() == type);
            if (panel != null)
            {
                panel.IsSelected = true;
                panel.IsActive = true;
                if (m_dockingManager != null) m_dockingManager.ActiveContent = panel;
                return;
            }

            if (type == typeof(ProjectOverviewVM)) panel = new ProjectOverviewVM(m_project);
            else if (type == typeof(LogPanelVM)) panel = new LogPanelVM();
            else
            {
                Logging.Logger.Warn($"Tried to show unknown panel: {type.FullName}");
                return;
            }

            Logging.Logger.Info($"Adding new panel '{panel.Title}' based on requested type: {type.FullName}");
            Windows.Add(panel);
        }

        /// <summary>
        /// Shows the script editor panel for the given <see cref="tree"/> identifier
        /// </summary>
        /// <param name="tree">Dialog Tree to show</param>
        public void ShowScriptEditorPanel(DialogTreeIdentifier tree)
        {
            DialogTree dialogTree = m_project[tree];
            if (dialogTree == null) return;

            ScriptPanelVM window = Windows.Select(w => w as ScriptPanelVM)
                .Where(s => s != null)
                .FirstOrDefault(s => s.Tree == tree);
            if (window == null)
            {
                window = new ScriptPanelVM(m_project, dialogTree);
                Windows.Add(window);
            }

            window.IsSelected = true;
            if (m_dockingManager != null) m_dockingManager.ActiveContent = window;
        }

        /// <summary>
        /// Shows the script diagram panel for the given <see cref="tree"/> identifier
        /// </summary>
        /// <param name="tree">Dialog Tree to show</param>
        public void ShowScriptDiagramPanel(DialogTreeIdentifier tree)
        {
            DialogTree dialogTree = m_project[tree];
            if (dialogTree == null) return;

            ScriptDiagramVM window = Windows.Select(w => w as ScriptDiagramVM)
                .Where(s => s != null)
                .FirstOrDefault(s => s.Tree == tree);
            if (window == null)
            {
                window = new ScriptDiagramVM(m_project, dialogTree);
                Windows.Add(window);
            }

            window.IsSelected = true;
            if (m_dockingManager != null) m_dockingManager.ActiveContent = window;
        }

        private void FindRecentProjects()
        {
            const int recentQuantity = 5;
            List<KeyValuePair<string, string>> recent = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < recentQuantity; i++)
            {
                recent.Add(new KeyValuePair<string, string>("RecentFile " + i, "c://somefile"));
            }
            RecentProjects = recent;
        }

        private void NewProject()
        {
            throw new NotImplementedException();
        }

        private void OpenProject(string path)
        {
            string filePath = path;
            if (path == null)
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Multiselect = false;
                openFile.Filter = $"NPC Project |*.{NpcChatProject.ProjectExtension}|All files|*.*";
                openFile.FilterIndex = 0;

                if (openFile.ShowDialog() == true)
                {
                    filePath = openFile.FileName;
                }
                else return;
            }

            if (filePath == null) return;

            throw new NotImplementedException();
        }

        private void SaveAs(string path)
        {
            string filePath = path;
            if (path == null)
            {
                SaveFileDialog openFile = new SaveFileDialog();
                openFile.Filter = $"NPC Project |*.{NpcChatProject.ProjectExtension}|All files|*.*";
                openFile.FilterIndex = 0;

                if (openFile.ShowDialog() == true)
                {
                    filePath = openFile.FileName;
                }
                else return;
            }

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
            Logging.Logger.Info($"Closed window, '{e.Document.Title}', id: '{e.Document.ContentId}'");

            LayoutDocument document = e.Document?.Content as LayoutDocument;
            if (Windows.Contains(document))
            {
                Windows.Remove(document);
            }
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
