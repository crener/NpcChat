using System.Windows.Input;
using FirstFloor.ModernUI.Presentation;
using NpcChat.Util;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace NpcChat.ViewModels.Base
{
    public abstract class DockPanelVM : NotificationObject
    {
        public ICommand CloseCommand
        {
            get
            {
                if (m_closeCommand == null)
                    m_closeCommand = new RelayCommand(call => Close());
                return m_closeCommand;
            }
        }

        public bool IsClosed
        {
            get => m_isClosed;
            set
            {
                if (m_isClosed != value)
                {
                    m_isClosed = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool CanClose
        {
            get => m_canClose;
            set
            {
                if (m_canClose != value)
                {
                    m_canClose = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Title
        {
            get => m_title;
            set
            {
                if (m_title != value)
                {
                    m_title = value;
                    RaisePropertyChanged();
                }
            }
        }


        private string m_title;
        private bool m_canClose = true;
        private bool m_isClosed = false;
        private ICommand m_closeCommand;

        public DockPanelVM()
        {
            m_title = GetType().FullName;
        }

        public void Close()
        {
            IsClosed = true;
        }
    }
}
