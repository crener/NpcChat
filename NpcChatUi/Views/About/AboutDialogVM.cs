using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NpcChat.Views.Utility;
using Prism.Commands;

namespace NpcChat.Views.Dialogs
{
    public class AboutDialogVM
    {
        public ICommand OpenBrowserCommand { get; }


        public AboutDialogVM()
        {
            OpenBrowserCommand = new DelegateCommand<string>(SimpleHelpers.OpenLink);
        }
    }
}
