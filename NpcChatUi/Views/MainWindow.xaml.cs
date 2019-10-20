using System;
using System.IO;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using NpcChat.ViewModels;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace NpcChat.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private WindowViewModel m_viewModel { get; }

        private string WorkspaceLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NpcChat", "workspace.xml");

        public MainWindow()
        {
            DataContext = m_viewModel = new WindowViewModel();
            InitializeComponent();
        }

        private void SaveLayout(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter(WorkspaceLocation))
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(DockingManager);
                layoutSerializer.Serialize(writer);
            }
        }

        private void LoadLayout(object sender, RoutedEventArgs e)
        {
            using (StreamReader reader = new StreamReader(WorkspaceLocation))
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(DockingManager);
                layoutSerializer.Deserialize(reader);
            }
        }
    }
}