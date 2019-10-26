using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FirstFloor.ModernUI.Presentation;
using NpcChat.Util;
using NpcChatSystem.Utilities;
using Prism.Commands;

namespace NpcChat.Views.Dialogs
{
    public class UnhandledExceptionDialogVM : NotificationObject
    {
        public string ExMessage => m_exception.Message;
        public string ExStack => m_exception.ToString();
        public ICommand ContinueCommand { get; }
        public ICommand QuitCommand { get; }
        public ICommand ReportCommand { get; }

        private readonly UnhandledExceptionEventArgs m_unhandledArgs;
        private readonly Window m_dialogWindow;
        private Exception m_exception = null;

        public UnhandledExceptionDialogVM(UnhandledExceptionEventArgs args, Window dialogWindow)
        {
            m_unhandledArgs = args;
            m_dialogWindow = dialogWindow;

            if (m_unhandledArgs.ExceptionObject != null)
            {
                m_exception = m_unhandledArgs.ExceptionObject as Exception;
            }

            ContinueCommand = new DelegateCommand(Continue);
            QuitCommand = new DelegateCommand(Quit);
            ReportCommand = new DelegateCommand(Report);
        }

        private void Continue()
        {
            MessageBoxResult result = MessageBox.Show("This may result in unexpected behaviour or an immediate crash.\nAre you sure you wish to continue.");
            switch (result)
            {
                case MessageBoxResult.Yes:
                case MessageBoxResult.OK:
                    m_dialogWindow.Close();
                    break;
                case MessageBoxResult.None:
                case MessageBoxResult.No:
                case MessageBoxResult.Cancel:
                    return;
                default:
                    Logging.Logger.Warn($"Unexpected '{nameof(UnhandledExceptionDialogVM)}' messageBox result: {result}");
                    return;
            }
        }

        private void Quit()
        {
            Environment.Exit(999);
        }

        private void Report()
        {
            string title = $"Unhandled Exception: '{(m_exception.ToString().Split('\n')[0])}'";
            string body = "Please add any any steps needed to get the error you experienced here!\n\n\n\n" +
                          "---------------------------\n" +
                          "Please leave the following text as is since it will give vital information needed to fix the issue.\n\n" +
                          $"{m_exception}\n" +
                          $"Terminal Exception: {(m_unhandledArgs.IsTerminating ? "Yes" : "No")}";
            string url = $"https://github.com/crener/NpcChat/issues/new?title={title}&body={HttpUtility.UrlEncode(body)}";

            System.Diagnostics.Process.Start(url);
        }
    }
}
