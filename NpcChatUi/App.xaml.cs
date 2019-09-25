using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Windows;

namespace NpcChat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            List<AssemblyTitleAttribute> title = Assembly.GetAssembly(typeof(App)).GetCustomAttributes<AssemblyTitleAttribute>().ToList();
            string path = Path.Combine(Path.GetTempPath(), title[0].Title);

            ProfileOptimization.SetProfileRoot(path);
            ProfileOptimization.StartProfile("jit-profile.cache");
        }
    }
}