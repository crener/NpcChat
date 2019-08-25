using System.Windows.Controls;
using NpcChat.ViewModels.Editors.Script;

namespace NpcChat.Views.Editors.Script
{
    /// <summary>
    /// Interaction logic for ScriptCharDialog.xaml
    /// </summary>
    public partial class CharacterDialog : UserControl
    {
        public CharacterDialogModel DialogModel => DataContext as CharacterDialogModel;

        public CharacterDialog()
        {
            InitializeComponent();
        }
    }
}
