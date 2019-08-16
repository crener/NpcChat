using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data.Util;
using ValueType = NpcChatSystem.Data.Util.ValueType;

namespace NpcChatSystem.Data
{
    [DebuggerDisplay("{Name}")]
    public struct Character
    {
        public int Id { get; internal set; }
        public string Name { get; set; }

        private List<CharacterTrait> traits;

        public Character(string name) : this()
        {
            Id = 0;
            Name = name;
            traits = new List<CharacterTrait>();
        }

        public string GetTrait(string key, string defaultValue = "")
        {
            foreach(CharacterTrait trait in traits)
            {
                if(trait.Name == key) return trait.Value;
            }
            return defaultValue;
        }
    }

    public struct CharacterTrait
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ValueType Type { get; set; }
    }
}
