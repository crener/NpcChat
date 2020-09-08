using System.ComponentModel.Composition;
using System.Xml;
using System.Xml.Serialization;
using NpcChat.Util;

namespace NpcChat.ViewModels.Settings.SettingsTabs
{
    [InheritedExport(typeof(IPreferenceTab))]
    public abstract class PreferenceTab : NotificationObject, IPreferenceTab
    {
        /// <summary>
        /// Name of the settings tab
        /// </summary>
        public abstract string Header { get; }
        
        /// <summary>
        /// Save the settings contained in the tab
        /// </summary>
        public void Save(XmlElement tabRoot)
        {
            using(XmlWriter writer = tabRoot.CreateNavigator().AppendChild())
            {
                new XmlSerializer(GetType()).Serialize(writer, this);
            }
        }

        public void Load(XmlElement tabRoot)
        {
            using(XmlReader reader = new XmlNodeReader(tabRoot))
            {
                XmlSerializer serializer = new XmlSerializer(GetType());
                object deserialize = serializer.Deserialize(reader);
            }


            using(XmlWriter writer = tabRoot.CreateNavigator().AppendChild())
            {
                new XmlSerializer(GetType()).Serialize(writer, this);
            }
        }
    }
}