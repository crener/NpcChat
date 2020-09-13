using System.ComponentModel;
using System.ComponentModel.Composition;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NpcChatSystem.Utilities;

namespace NpcChat.ViewModels.Settings.SettingsTabs
{
    public class GeneralPreference : PreferenceTab
    {
        [Browsable(false), JsonIgnore] 
        public static GeneralPreference Instance { get; set; }

        /// <summary>
        /// Name of the preference type
        /// </summary>
        [Browsable(false)]
        public override string Header => "General";
        
        /// <summary>
        /// Allow spell check
        /// </summary>
        [DisplayName("Enable Spell Check"), Description("Enables spell check inside branch editing")]
        public bool EnableSpellCheck
        {
            get => m_enableSpellCheck;
            set
            {
                if(m_enableSpellCheck != value)
                {
                    m_enableSpellCheck = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool m_enableSpellCheck = false;
        
        [Browsable(false)] 
        public GeneralPreference()
        {
            if(Instance == null) Instance = this;
            else Logging.Logger.Log(LogLevel.Error, $"Multiple {nameof(GeneralPreference)} preference tabs created during runtime!");
        }
    }
}