using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using NpcChat.Backend.Validation;
using NpcChat.Views.Dialogs;
using NpcChatSystem.System;
using NpcChatSystem.System.TypeStore;
using NpcChatSystem.Utilities;
using LogLevel = NLog.LogLevel;

namespace NpcChat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                UnhandledExceptionDialog dialog = new UnhandledExceptionDialog(args);
                dialog.ShowDialog();
            };
#endif

            SetupJitCache();
            StartBackgroundInitialization();
        }

        private void SetupJitCache()
        {
            List<AssemblyTitleAttribute> title = Assembly.GetAssembly(typeof(App)).GetCustomAttributes<AssemblyTitleAttribute>().ToList();
            string path = Path.Combine(Path.GetTempPath(), title[0].Title);

            Directory.CreateDirectory(path);
            ProfileOptimization.SetProfileRoot(path);
            ProfileOptimization.StartProfile("jit-profile.cache");
        }

        private void StartBackgroundInitialization()
        {
            Task spellingSetup = new Task(() =>
            {
                try
                {
                    // Load the spelling dictionaries
                    RuntimeHelpers.RunClassConstructor(typeof(SpellCheck).TypeHandle);
                }
                catch(Exception ex)
                {
                    Logging.Logger.Log(LogLevel.Error, ex, $"Failed to force initialize '{nameof(SpellCheck)}' spelling dictionaries");
                }
            });

            spellingSetup.Start();
        }
    }
}