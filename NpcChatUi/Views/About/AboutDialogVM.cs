using System.Windows.Input;
using NpcChat.Views.Utility;
using Prism.Commands;

namespace NpcChat.Views.About
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
