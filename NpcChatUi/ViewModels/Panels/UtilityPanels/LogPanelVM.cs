using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NLog;
using NpcChatSystem.Utilities;
using Prism.Commands;
using Xceed.Wpf.AvalonDock.Layout;

namespace NpcChat.ViewModels.Panels.UtilityPanels
{
    public class LogPanelVM : LayoutAnchorable
    {
        public ICommand TestLogCommand { get; }
        public bool AutoScroll
        {
            get => m_autoScroll;
            set
            {
                m_autoScroll = value;
                RaisePropertyChanged(nameof(AutoScroll));
            }
        }

        private bool m_autoScroll;

        public LogPanelVM()
        {
            Title = "Log";
            ToolTip = "Log";
            CanClose = true;

            TestLogCommand = new DelegateCommand(() =>
            {
                Logging.Logger.Info("This is a test log Message");
            });
        }
    }
}
