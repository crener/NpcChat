using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public partial class CharacterLabel : UserControl
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

        public string Text
        {
            get => (string)GetValue(TextProperty);
        }

        public CharacterLabel()
        {
            InitializeComponent();
            Label.DataContext = this;
        }

        public static readonly DependencyProperty SelectedCharacterProperty =
            DependencyProperty.Register(nameof(SelectedCharacter), typeof(int), typeof(CharacterLabel), new PropertyMetadata(0, CharacterChanged));
        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register(nameof(Project), typeof(NpcChatProject), typeof(CharacterLabel), new PropertyMetadata(CurrentProject.Project));
        public static readonly DependencyPropertyKey TextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Text), typeof(string), typeof(CharacterLabel), new PropertyMetadata(""));
        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        private static void CharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterLabel label = d as CharacterLabel;
            if (label == null) return;

            NpcChatProject project = label.Project;
            if(project == null)
                label.SetValue(TextPropertyKey, "No Project Set!");

            IList<CharacterId> nameIds = project?.ProjectCharacters.AvailableCharacters();
            if (nameIds == null) return;

            int id = (int)e.NewValue;
            CharacterId character = nameIds.FirstOrDefault(n => n.Id == id);

            label.SetValue(TextPropertyKey, character.Id == id ? character.Name : $"No Char({id})");
        }
    }
}
