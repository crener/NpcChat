using System.Collections.Generic;
using System.Diagnostics;
using NpcChatSystem.Data.Util;
using NpcChatSystem.System;

namespace NpcChatSystem.Data.Character
{
    [DebuggerDisplay("{Name}")]
    public class Character : NotificationObject
    {
        public const int PreRegisteredId = 0;

        public int Id { get; internal set; }

        public string Name
        {
            get => m_name;
            set
            {
                if (m_name != value)
                {
                    m_name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public IReadOnlyList<string> TraitNames => m_traitNames;

        private readonly CharacterStore m_store;
        private readonly List<CharacterTrait> m_traits;
        private readonly List<string> m_traitNames;
        private string m_name;

        internal Character(CharacterStore store, string name)
        {
            m_store = store;
            Id = PreRegisteredId;
            Name = name;

            m_traits = new List<CharacterTrait>();

            m_traitNames = new List<string>();
            m_traitNames.Add(nameof(Name));
        }

        public string GetTrait(string key, string defaultValue = null)
        {
            foreach (CharacterTrait trait in m_traits)
            {
                if (trait.Name == key) return trait.Value;
            }
            return defaultValue;
        }
    }
}
