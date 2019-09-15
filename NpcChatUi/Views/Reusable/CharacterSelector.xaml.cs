using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using NpcChat.Backend;
using NpcChatSystem;
using NpcChatSystem.Data.CharacterData;

namespace NpcChat.Views.Reusable
{
    /// <summary>
    /// Interaction logic for CharacterSelector.xaml
    /// </summary>
    public partial class CharacterSelector : UserControl
    {
        public int SelectedCharacter
        {
            get => (int)GetValue(SelectedCharacterProperty);
            set => SetValue(SelectedCharacterProperty, value);
        }

        public NpcChatProject Project
        {
            get { return (NpcChatProject)GetValue(ProjectProperty); }
            set { SetValue(ProjectProperty, value); }
        }

        public ObservableCollection<CharacterId> Names { get; } = new ObservableCollection<CharacterId>();


        public CharacterSelector()
        {
            InitializeComponent();
            SelectorGrid.DataContext = this;

            UpdateCharacters(0);
        }

        private void UpdateCharacters(int charId)
        {
            IList<CharacterId> nameIds = Project?.ProjectCharacters.AvailableCharacters();
            if (nameIds == null) return;

            Names.Clear();
            Names.AddRange(nameIds);
        }

        public static readonly DependencyProperty SelectedCharacterProperty =
            DependencyProperty.Register("SelectedCharacter", typeof(int), typeof(CharacterSelector), new PropertyMetadata(0, CharacterChanged));
        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(NpcChatProject), typeof(CharacterSelector), new PropertyMetadata(CurrentProject.Project, ProjectChanged));

        private static void ProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterSelector selector = d as CharacterSelector;
            if(selector == null) return;

            if(e.OldValue != null)
            {
                NpcChatProject project = e.OldValue as NpcChatProject;
                project.ProjectCharacters.CharacterAdded -= selector.UpdateCharacters;
                project.ProjectCharacters.CharacterRemoved -= selector.UpdateCharacters;
            }

            if (e.NewValue != null)
            {
                NpcChatProject project = e.NewValue as NpcChatProject;
                project.ProjectCharacters.CharacterAdded += selector.UpdateCharacters;
                project.ProjectCharacters.CharacterRemoved += selector.UpdateCharacters;

                selector.UpdateCharacters(0);
            }
        }

        private static void CharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
    }
}
