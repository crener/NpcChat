

using System;
using System.Xml;

namespace NpcChat.ViewModels.Settings
{
    public interface IPreferenceTab
    {
        /// <summary>
        /// Name of the settings tab
        /// </summary>
        string Header { get; }
        
        /// <summary>
        /// Save the tabs settings into the <paramref name="tabRoot"/>
        /// </summary>
        /// <param name="tabRoot">xml Element to save settings into</param>
        void Save(XmlElement tabRoot);

        /// <summary>
        /// Loads settings from preexisting settings
        /// </summary>
        /// <param name="tabRoot">xmlElement containing data</param>
        /// <remarks>may be null if previous settings don't contain this data</remarks>
        void Load(XmlElement tabRoot);
    }
}