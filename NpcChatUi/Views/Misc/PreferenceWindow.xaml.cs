using System;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using NpcChat.ViewModels.Settings;

namespace NpcChat.Views.Misc
{
    public partial class PreferenceWindow : ModernWindow
    {
        private bool m_handledClosure = false;
        
        public PreferenceWindow()
        {
            DataContext = Preferences.Instance;
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            if(!m_handledClosure && Preferences.Instance.CancelChangesCmd.CanExecute())
            {
                Preferences.Instance.CancelChangesCmd.Execute();
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            m_handledClosure = true;
            Close();
        }
    }
}