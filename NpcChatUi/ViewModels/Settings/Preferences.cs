using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Web.WebSockets;
using System.Xml.Serialization;
using NpcChat.ViewModels.Settings.SettingsTabs;

namespace NpcChat.ViewModels.Settings
{
    [Serializable]
    public class Preferences
    {
        public static Preferences Instance { get; private set; }

        [XmlArray, ImportMany(typeof(IPreferenceTab), AllowRecomposition = true)]
        public ObservableCollection<IPreferenceTab> Tabs { get; } = new ObservableCollection<IPreferenceTab>();
          
        /// <summary>
        /// Path to the settings file
        /// </summary>
        public static string SettingsFileLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NpcChat", "Settings.xml");
        
        static Preferences()
        {
            // Load previous data
            if(File.Exists(SettingsFileLocation))
            {
                using(StringReader reader = new StringReader(SettingsFileLocation))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Preferences));

                    if(serializer.Deserialize(reader) is Preferences data)
                    {
                        Instance = data;
                    }
                }
            }
            else
            {
                Instance = new Preferences();
            }

            App.AppContainer.SatisfyImportsOnce(Instance);
        }
    }
}