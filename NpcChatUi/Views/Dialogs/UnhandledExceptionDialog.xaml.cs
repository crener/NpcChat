using System;
using System.Windows;

namespace NpcChat.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for UnhandledExceptionDialog.xaml
    /// </summary>
    public partial class UnhandledExceptionDialog : Window
    {
        public UnhandledExceptionDialog(UnhandledExceptionEventArgs args)
        {
            DataContext = new UnhandledExceptionDialogVM(args, this);
            InitializeComponent();
        }
    }
}
