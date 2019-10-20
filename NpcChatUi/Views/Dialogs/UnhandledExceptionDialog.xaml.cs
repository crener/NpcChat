using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using NpcChat.Views.Dialogs;

namespace NpcChat.Views.Dialog
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
