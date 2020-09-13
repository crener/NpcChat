using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NpcChat.Util;
using NpcChat.ViewModels.Panels.Project;

namespace NpcChat.ViewModels.Settings.SettingsTabs
{
    [DisplayName("{Header}"), InheritedExport(typeof(IPreferenceTab))]
    public abstract class PreferenceTab : NotificationObject, IPreferenceTab
    {
        /// <summary>
        /// Name of the settings tab
        /// </summary>
        [Browsable(false), JsonIgnore]
        public abstract string Header { get; }

        /// <summary>
        /// Iteration of the preference tabs data, useful for handling possible data migration between versions
        /// </summary>
        [Browsable(false)]
        public virtual int DataVersion => 0;

        /// <summary>
        /// <inheritdoc cref="IPreferenceTab.SerializationName"/>
        /// </summary>
        [Browsable(false)]
        public string SerializationName => GetType().Name;

        
        public virtual void Load(JToken data)
        {
            string jsonData;
            int version = data[nameof(DataVersion)].Value<int>();
            if(version != DataVersion && LoadDataRequiresUpgrade(version))
                jsonData = UpgradeData(data);
            else jsonData = data.ToString();
            
            JsonConvert.PopulateObject(jsonData, this);
        }

        /// <summary>
        /// Given <paramref name="dataVersion"/> does this version of data require tweaks before being loaded into the current version?
        /// </summary>
        /// <remarks>This is only called if the current <see cref="DataVersion"/> does not match the version provided in the data </remarks>
        /// <returns>true if </returns>
        protected virtual bool LoadDataRequiresUpgrade(int dataVersion) => true;

        /// <summary>
        /// transform the load data to make it compatible with the current version of the preferenceData
        /// </summary>
        /// <param name="data">data that requires conversion</param>
        /// <returns>json representation of the compatible save data</returns>
        protected string UpgradeData(JToken data)
        {
            // overload this when data actually starts requiring upgrades 
            return data.ToString();
        }
    }
}