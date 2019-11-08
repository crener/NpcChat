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
    }
}
