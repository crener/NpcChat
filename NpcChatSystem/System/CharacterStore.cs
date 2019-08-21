using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data;

namespace NpcChatSystem.System
{
    public class CharacterStore
    {
        private static readonly Random s_random = new Random();

        Dictionary<int, Character> m_characters = new Dictionary<int, Character>();

        /// <summary>
        /// Event used to notify of a change in a characters information
        /// </summary>
        /// <param name="charId">Id of the character</param>
        /// <param name="changed">Data that was changed</param>
        public delegate void CharacterChangeEvent(int charId, UpdatedField changed);

        /// <summary>
        /// Find a character by id
        /// </summary>
        /// <param name="id">id of the character</param>
        /// <returns>character if found, null if not found</returns>
        public Character? GetCharacter(int id)
        {
            if(HasCharacter(id))
            {
                return m_characters[id];
            }

            return null;
        }

        public bool HasCharacter(int id)
        {
            if (id <= 0) return false;
            return m_characters.ContainsKey(id);
        }

        public bool RegisterNewCharacter(out int generatedId, Character character)
        {
            generatedId = -1;
            if(HasCharacter(character.Id)) return false;

            if(m_characters.Values.Any(c => c.Name == character.Name))
            {
                return false;
            }

            generatedId = character.Id = GenerateUniqueId();
            m_characters.Add(generatedId, character);

            return true;
        }

        private int GenerateUniqueId()
        {
            while (true)
            {
                int i = s_random.Next(1, int.MaxValue);
                if (!HasCharacter(i)) //char Id should always be 0 before being reassigned
                    return i;
            }
        }

        public enum UpdatedField
        {
            Unspecified = 0,
            Name = 100,
            Trait = 200,
        }

    }
}
