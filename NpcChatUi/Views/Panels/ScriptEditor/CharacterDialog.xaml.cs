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

        public CharacterDialog()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property.Name == "IsSelectionActive")
            {
                DialogVm.InspectionActive = e.NewValue as bool? ?? false;
            }
        }
    }
}
