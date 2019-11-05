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

        public MainWindow()
        {
            DataContext = m_viewModel = new WindowViewModel();
            InitializeComponent();
            m_viewModel.SetDockingManager(DockingManager);
        }
    }
}