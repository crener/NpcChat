using System;
using System.Diagnostics;

namespace NpcChatSystem.Data.CharacterData
{
    [DebuggerDisplay("{Name}, ID: {Id}")]
    public struct CharacterId
    {
        public static int DefaultId = 0;

        public string Name { get; }
        public int Id { get; }

        public CharacterId(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }

    public struct CharacterTrait
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public Util.ValueType Type { get; set; }
    }
}
