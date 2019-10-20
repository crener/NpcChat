using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Windows;
using NpcChat.Views.Dialog;
using NpcChat.Views.Dialogs;
using NpcChatSystem.System;
using NpcChatSystem.System.TypeStore;

namespace NpcChat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                UnhandledExceptionDialog dialog = new UnhandledExceptionDialog(args);
                dialog.ShowDialog();
            };

            List<AssemblyTitleAttribute> title = Assembly.GetAssembly(typeof(App)).GetCustomAttributes<AssemblyTitleAttribute>().ToList();
            string path = Path.Combine(Path.GetTempPath(), title[0].Title);

            ProfileOptimization.SetProfileRoot(path);
            ProfileOptimization.StartProfile("jit-profile.cache");
        }
    }
}