using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using NpcChat.Backend;
using NpcChat.Util;
using NpcChat.ViewModels.Base;
using NpcChat.ViewModels.Editors.Script;
using NpcChatSystem;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NpcChatSystem.Identifiers;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace NpcChat.ViewModels
{
    public class WindowViewModel : NotificationObject
    {
        public ObservableCollection<DockPanelVM> Windows { get; }

        private NpcChatProject m_project;
        private DialogTree m_tree;


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
                DialogTreeBranch branch = m_tree.GetStart();
                branch.Name = "Start";

                DialogSegment segment = branch.CreateNewDialog(diane);
                DialogSegment segment2 = branch.CreateNewDialog(jerry);

                branch = m_tree.CreateNewBranch();
                branch.Name = "First Branch";
                //Branches.Add(new TreePartVM(m_project, branch));
                DialogSegment segment3 = branch.CreateNewDialog(gran);
                DialogSegment segment4 = branch.CreateNewDialog(banana);
            }

            Windows = new ObservableCollection<DockPanelVM>();
            Windows.Add(new ScriptPanelVM(m_project, m_tree));
        }
    }
}
