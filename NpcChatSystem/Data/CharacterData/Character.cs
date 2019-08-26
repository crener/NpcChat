using System.Collections.Generic;
using System.Diagnostics;

namespace NpcChatSystem.Data.CharacterData
{
    [DebuggerDisplay("{Name}")]
    public struct Character
    {
        public const int PreRegisteredId = 0;

        public int Id { get; internal set; }
        public string Name { get; set; }

        private List<CharacterTrait> m_traits;

        public Character(string name) : this()
        {
            Id = PreRegisteredId;
            Name = name;
            m_traits = new List<CharacterTrait>();
        }

        public string GetTrait(string key, string defaultValue = "")
        {
            foreach(CharacterTrait trait in m_traits)
            {
                if(trait.Name == key) return trait.Value;
            }
            return defaultValue;
        }
    }
}
