using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms.VisualStyles;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NpcChat.Exceptions;
using NpcChat.Util;
using NpcChatSystem.Utilities;
using Prism.Commands;
using Formatting = Newtonsoft.Json.Formatting;

namespace NpcChat.ViewModels.Settings
{
    public class Preferences : NotificationObject
    {
        [JsonProperty(PropertyName = "Version")]
        public const int SettingsVersion = 1;
        public static Preferences Instance { get; }

        /// <summary>
        /// Path to the settings file
        /// </summary>
        public static string SettingsFileLocation => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "NpcChat", "Settings.json");

        [ImportMany(typeof(IPreferenceTab), AllowRecomposition = true), JsonProperty(PropertyName = "Categories")]
        public ObservableCollection<IPreferenceTab> Tabs { get; } = new ObservableCollection<IPreferenceTab>();

        [JsonIgnore] public DelegateCommand SavePreferencesCmd { get; }
        [JsonIgnore] public DelegateCommand CancelChangesCmd { get; }

        private bool m_changed;

        static Preferences()
        {
            Instance = new Preferences();

            // load all data that we can!
            App.AppContainer.SatisfyImportsOnce(Instance);
            Instance.ValidateDuplicatedTabs();

            if(File.Exists(SettingsFileLocation))
            {
                Instance.LoadData();
            }
            else
            {
                // this is done so that cancel works correctly by loading defaults
                Instance.SaveChanges(true);
            }

            foreach (IPreferenceTab preferenceTab in Instance.Tabs)
            {
                preferenceTab.PropertyChanged += Instance.PreferenceChanged;
            }
        }

        internal Preferences()
        {
            SavePreferencesCmd = new DelegateCommand(() => SaveChanges(), () => m_changed);
            CancelChangesCmd = new DelegateCommand(LoadData);
        }

        public void LoadData()
        {
            if(!File.Exists(SettingsFileLocation)) return;

            Logging.Logger.Info("Loading preferences");
            
            JObject jsonFile = JObject.Parse(File.ReadAllText(SettingsFileLocation));
            if(jsonFile.ContainsKey("Categories"))
            {
                foreach (JToken tabData in jsonFile["Categories"])
                {
                    string name = tabData[nameof(IPreferenceTab.SerializationName)]?.Value<string>();
                    if(name == null)
                    {
                        Logging.Logger.Error("Currupted preference data found, " +
                            $"a category is missing the {nameof(IPreferenceTab.SerializationName)} entry!");
                        continue;
                    }

                    IPreferenceTab tab = Tabs.FirstOrDefault(t => t.SerializationName == name);

                    if(tab != null) tab.Load(tabData);
                    else Logging.Logger.Error($"Unknown preference data found for {name}");
                }
            }
            else Logging.Logger.Error("Failed to locate preference data in json document!");

            Logging.Logger.Info("Loading complete");
        }

        /// <summary>
        /// Check the tabs to make sure that all settings have unique names, if there are name collisions the wrong settings could be passed
        /// to a preference tab.
        /// </summary>
        /// <exception cref="DuplicateLoadingException">thrown when a setting tab name collision has been found</exception>
        private void ValidateDuplicatedTabs()
        {
            HashSet<string> visited = new HashSet<string>();
            foreach (IPreferenceTab tabData in Tabs)
            {
                string name = tabData.SerializationName;
                if(visited.Contains(name))
                {
                    throw new DuplicateLoadingException(
                        $"{name} loaded twice! This means there is may be invalid/corrupted preference data present");
                }

                visited.Add(name);
            }
        }

        /// <summary>
        /// Save any pending changes to user preferences
        /// </summary>
        /// <param name="forceSave">save to disk regardless of changed data</param>
        public void SaveChanges(bool forceSave = false)
        {
            if(m_changed || forceSave)
            {
                // make sure the saving directory exists
                string dirPath = Path.GetDirectoryName(SettingsFileLocation);
                if(!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                string jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);

                using(Stream stream = new FileStream(SettingsFileLocation, FileMode.Create))
                using(TextWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                }
            }

            // everything saved, reset the UI
            m_changed = false;
            SavePreferencesCmd.RaiseCanExecuteChanged();
        }

        private void PreferenceChanged(object sender, PropertyChangedEventArgs e)
        {
            m_changed = true;
            SavePreferencesCmd.RaiseCanExecuteChanged();
        }
    }
}