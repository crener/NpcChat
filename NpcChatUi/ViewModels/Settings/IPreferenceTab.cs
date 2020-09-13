using System.ComponentModel;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace NpcChat.ViewModels.Settings
{
    public interface IPreferenceTab : INotifyPropertyChanged
    {
        /// <summary>
        /// Name of the settings tab
        /// </summary>
        string Header { get; }

        /// <summary>
        /// Name as shown in the saved data of the preferences to identify what to pass the data into 
        /// </summary>
        string SerializationName { get; }

        /// <summary>
        /// Loads settings from preexisting settings
        /// </summary>
        /// <param name="tabData">XElement containing the previously data</param>
        /// <remarks>may be null if previous settings don't contain any data</remarks>
        void Load(JToken tabData);
    }
}