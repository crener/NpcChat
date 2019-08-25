using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using NpcChat.Properties;
using NpcChat.ViewModels.Editors.Reusable;
using NpcChatSystem;

namespace NpcChat.Views.Editors.Reusable
{
    /// <summary>
    /// Interaction logic for CharacterLabel.xaml
    /// </summary>
    public partial class CharacterLabel : UserControl, INotifyPropertyChanged
    {
        public NpcChatProject Project
        {
            get => (NpcChatProject)GetValue(ProjectProperty);
            set
            {
                SetValue(ProjectProperty, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(CharacterName));
            }
        }

        public int CharacterId
        {
            get => (int)GetValue(CharacterIdProperty);
            set
            {
                SetValue(CharacterIdProperty, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(CharacterName));
            }
        }

        public string CharacterName
        {
            get
            {
                if (Project == null) return "No Project";
                if (Project.ProjectCharacters == null) return "No Character manager!";

                return Project.ProjectCharacters.GetCharacter(CharacterId)?.Name ?? "Unknown";
            }
        }

        //private CharacterLabelViewModel m_model;
        /*private NpcChatProject m_project;
        private int m_characterId;*/

        public CharacterLabel()
        {
            //DataContext = m_model = new CharacterLabelViewModel();
            InitializeComponent();
            //DataContext = this;
        }

        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register(nameof(Project), typeof(NpcChatProject), typeof(CharacterLabel), new PropertyMetadata(null, ProjectChanged));
        public static readonly DependencyProperty CharacterIdProperty =
            DependencyProperty.Register(nameof(CharacterId), typeof(int), typeof(CharacterLabel), new PropertyMetadata(-1, CharacterChanged));

        private static void CharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterLabel label = ((CharacterLabel)d);
            label.OnPropertyChanged(nameof(CharacterId));
            label.OnPropertyChanged(nameof(CharacterName));
        }
        private static void ProjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CharacterLabel label = ((CharacterLabel)d);
            label.OnPropertyChanged(nameof(Project));
            label.OnPropertyChanged(nameof(CharacterName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
