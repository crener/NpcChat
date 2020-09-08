using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using NpcChat.ViewModels.Settings;

namespace NpcChat.Views.Dialogs
{
    public partial class PreferencePane : ModernWindow
    {
        public PreferencePane()
        {
            DataContext = Preferences.Instance;
            InitializeComponent();
        }
    }
}