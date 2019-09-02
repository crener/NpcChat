using System.Windows;
using System.Windows.Controls;

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

        
        private static void CharacterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public CharacterSelector()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty SelectedCharacterProperty =
            DependencyProperty.Register("SelectedCharacter", typeof(int), typeof(CharacterSelector), new PropertyMetadata(0, CharacterChanged));
    }
}
