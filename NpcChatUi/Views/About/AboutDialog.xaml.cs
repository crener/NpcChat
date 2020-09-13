using FirstFloor.ModernUI.Windows.Controls;

namespace NpcChat.Views.About
{
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : ModernWindow
    {
        public AboutDialog()
        {
            DataContext = new AboutDialogVM();
            InitializeComponent();
        }
    }
}
