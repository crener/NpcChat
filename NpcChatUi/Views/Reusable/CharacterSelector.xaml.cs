using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using NpcChat.Backend;
using NpcChat.Util;
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
    public partial class CharacterSelector : UserControl
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

        private CharacterSelectorVM m_viewModel => SelectorGrid.DataContext as CharacterSelectorVM;

        public CharacterSelector()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                if (args.OldValue is INotifyPropertyChanged oldContext)
                    oldContext.PropertyChanged -= OnDataContextChanged;
                if (args.NewValue is INotifyPropertyChanged newContext)
                    newContext.PropertyChanged += OnDataContextChanged;
            };
            SelectorGrid.DataContext = new CharacterSelectorVM(Project);
        }

        private void OnDataContextChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CharacterSelectorVM.SelectedCharacter) &&
               SelectedCharacter != m_viewModel.SelectedCharacter)
            {
                SelectedCharacter = m_viewModel.SelectedCharacter;
            }
        }

        private static void SelectedCharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterSelector selector = d as CharacterSelector;
            if (selector == null) return;

            selector.m_viewModel.SelectedCharacter = (int)e.NewValue;
        }


        private static void ProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterSelector selector = d as CharacterSelector;
            if (selector == null) return;

            selector.m_viewModel.Project = e.NewValue as NpcChatProject;
        }


        public static readonly DependencyProperty SelectedCharacterProperty = DependencyProperty.Register(nameof(SelectedCharacter), typeof(int), typeof(CharacterSelector),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedCharacterChanged));
        public static readonly DependencyProperty ProjectProperty = DependencyProperty.Register(nameof(Project), typeof(NpcChatProject), typeof(CharacterSelector),
            new PropertyMetadata(CurrentProject.Project, ProjectChanged));
    }


    internal class CharacterSelectorVM : NotificationObject
    {
        public int SelectedCharacter
        {
            get => m_selectedCharacter;
            set
            {
                if (m_selectedCharacter == value) return;

                m_selectedCharacter = value;
                RaisePropertyChanged();
            }
        }

        public NpcChatProject Project
        {
            get => m_project;
            set
            {
                if (m_project == value) return;

                ProjectChanged(m_project, value);
                m_project = value;
                UpdateCharacters();

                RaisePropertyChanged();
            }
        }

        public DeferrableObservableCollection<CharacterId> Names { get; } = new DeferrableObservableCollection<CharacterId>();

        private int m_selectedCharacter;
        private NpcChatProject m_project;

        public CharacterSelectorVM([NotNull]NpcChatProject project)
        {
            Project = project;
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
                RaisePropertyChanged(nameof(SelectedCharacter));
            }
        }

        /// <summary>
        /// Refreshes the name information in response to a likely rename ever from the <see cref="CharacterStore"/>
        /// </summary>
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

        /// <summary>
        /// Makes sure that the correct events are registered too for character updates
        /// </summary>
        /// <param name="oldProject">existing project, to unregister</param>
        /// <param name="newProject">existing project, to register</param>
        private void ProjectChanged(NpcChatProject oldProject, NpcChatProject newProject)
        {
            if (oldProject != null)
            {
                oldProject.ProjectCharacters.CharacterAdded -= UpdateCharacters;
                oldProject.ProjectCharacters.CharacterRemoved -= UpdateCharacters;
                oldProject.ProjectCharacters.CharacterChanged -= UpdateCharacters;
            }

            if (newProject != null)
            {
                newProject.ProjectCharacters.CharacterAdded += UpdateCharacters;
                newProject.ProjectCharacters.CharacterRemoved += UpdateCharacters;
                newProject.ProjectCharacters.CharacterChanged += UpdateCharacters;
            }
        }
    }
}
