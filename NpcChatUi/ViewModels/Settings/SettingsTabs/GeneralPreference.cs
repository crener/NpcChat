using System.ComponentModel;
using System.ComponentModel.Composition;

namespace NpcChat.ViewModels.Settings.SettingsTabs
{
    [DisplayName("{Header}"), InheritedExport(nameof(GeneralPreference), typeof(IPreferenceTab))]
    public class GeneralPreference : PreferenceTab
    {
        [Browsable(false), Import(nameof(GeneralPreference), typeof(IPreferenceTab))]
        public static GeneralPreference Instance;

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
    }
}