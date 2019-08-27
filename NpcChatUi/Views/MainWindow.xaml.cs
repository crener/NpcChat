using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using NpcChat.ViewModels;

namespace NpcChat.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            DataContext = new WindowViewModel();
            InitializeComponent();
        }
    }
}