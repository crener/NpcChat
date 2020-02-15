using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using NpcChat.Backend;
using NpcChat.Views.Utility;
using NpcChatSystem;
using NpcChatSystem.Annotations;
using NpcChatSystem.Data.CharacterData;
using NpcChatSystem.System;

namespace NpcChat.Views.Reusable
{
    /// <summary>
    /// Interaction logic for CharacterSelector.xaml
    /// </summary>
    public partial class CharacterSelector : UserControl, INotifyPropertyChanged
    {
        public int SelectedCharacter
        {
            get => (int)GetValue(SelectedCharacterProperty);
            set => SetValue(SelectedCharacterProperty, value);
        }

        public NpcChatProject Project
        {
            get => (NpcChatProject)GetValue(ProjectProperty);
            set => SetValue(ProjectProperty, value);
        }

        public DeferrableObservableCollection<CharacterId> Names { get; } = new DeferrableObservableCollection<CharacterId>();


        public CharacterSelector()
        {
            InitializeComponent();
            SelectorGrid.DataContext = this;

            Project.ProjectCharacters.CharacterAdded += UpdateCharacters;
            Project.ProjectCharacters.CharacterRemoved += UpdateCharacters;
            Project.ProjectCharacters.CharacterChanged += UpdateCharacters;

            UpdateCharacters();
        }

        /// <summary>
        /// Refreshes the name information in response to a likely rename ever from the <see cref="CharacterStore"/>
        /// </summary>
        private void UpdateCharacters(int charId, CharacterStore.UpdatedField field)
        {
            if (field == CharacterStore.UpdatedField.Name ||
               field == CharacterStore.UpdatedField.Unspecified)
            {
                UpdateCharacters(charId);

                {
                    // really stupid hack to stop combo box from being blank
                    // todo there has to be a better way of doing this?
                    int original = SelectedCharacter;
                    SelectedCharacter = original + 1;
                    SelectedCharacter = original;
                }
            }
        }

        private void UpdateCharacters(int charId = 0)
        {
            IList<CharacterId> nameIds = Project?.ProjectCharacters.AvailableCharacters();
            if (nameIds == null) return;

            using (Names.CreateDeferringScope())
            {
                Names.Clear();
                Names.AddRange(nameIds);
            }
        }

        public static readonly DependencyProperty SelectedCharacterProperty = DependencyProperty.Register(nameof(SelectedCharacter), typeof(int), typeof(CharacterSelector),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register(nameof(Project), typeof(NpcChatProject), typeof(CharacterSelector), new PropertyMetadata(CurrentProject.Project, ProjectChanged));

        private static void ProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterSelector selector = d as CharacterSelector;
            if (selector == null) return;

            if (e.OldValue is NpcChatProject oldProject)
            {
                oldProject.ProjectCharacters.CharacterAdded -= selector.UpdateCharacters;
                oldProject.ProjectCharacters.CharacterRemoved -= selector.UpdateCharacters;
                oldProject.ProjectCharacters.CharacterChanged -= selector.UpdateCharacters;
            }

            if (e.OldValue is NpcChatProject newProject)
            {
                newProject.ProjectCharacters.CharacterAdded += selector.UpdateCharacters;
                newProject.ProjectCharacters.CharacterRemoved += selector.UpdateCharacters;
                newProject.ProjectCharacters.CharacterChanged += selector.UpdateCharacters;

                selector.UpdateCharacters();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
