using MahApps.Metro.Controls;
using NpcChat.ViewModels;

namespace NpcChat.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            DataContext = new WindowViewModel();
            InitializeComponent();
        }
    }
}