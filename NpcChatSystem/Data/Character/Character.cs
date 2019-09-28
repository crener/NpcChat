using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NpcChatSystem.Data.CharacterData
{
    [DebuggerDisplay("{Name}")]
    public struct Character
    {
        public const int PreRegisteredId = 0;

        public int Id { get; internal set; }
        public string Name { get; set; }

        public IReadOnlyList<string> TraitNames => m_traitNames;

        private readonly List<CharacterTrait> m_traits;
        private readonly List<string> m_traitNames;

        public Character(string name) : this()
        {
            Id = PreRegisteredId;
            Name = name;

            m_traits = new List<CharacterTrait>();

            m_traitNames = new List<string>();
            m_traitNames.Add(nameof(Name));
        }

        public string GetTrait(string key, string defaultValue = null)
        {
            foreach(CharacterTrait trait in m_traits)
            {
                if(trait.Name == key) return trait.Value;
            }
            return defaultValue;
        }
    }
}
