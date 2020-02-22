using System.Windows;
using System.Windows.Controls;
using NpcChat.ViewModels.Panels.ScriptEditor;

namespace NpcChat.Views.Panels.ScriptEditor
{
    /// <summary>
    /// Interaction logic for ScriptCharDialog.xaml
    /// </summary>
    public partial class CharacterDialog : UserControl
    {
        public CharacterDialogVM DialogVm => DataContext as CharacterDialogVM;

        /// <summary>
        /// boolean to represent wether the character dialog should show it's edit mode components or show the dialog text
        /// </summary>
        public bool InspectionActive
        {
            get => (bool)GetValue(InspectionActiveProperty);
            set => SetValue(InspectionActiveProperty, value);
        }


        public CharacterDialog()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty InspectionActiveProperty =
            DependencyProperty.Register(nameof(InspectionActive), typeof(bool), typeof(CharacterDialog), new PropertyMetadata(false, OnInspectionActiveChanged));

        private static void OnInspectionActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharacterDialog dialog && dialog.DialogVm != null)
                dialog.DialogVm.InspectionActive = (bool)e.NewValue;
        }
    }
}
