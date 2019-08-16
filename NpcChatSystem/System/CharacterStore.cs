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
        List<Character> m_characters = new List<Character>();
        private static readonly Random s_random = new Random();

        /// <summary>
        /// Find a character by id
        /// </summary>
        /// <param name="id">id of the character</param>
        /// <returns>character if found, null if not found</returns>
        public Character GetCharacter(int id)
        {
            return m_characters.FirstOrDefault(c => c.Id == id);
        }

        public bool HasCharacter(int id)
        {
            if (id <= 0) return false;
            return m_characters.Any(c => c.Id == id);
        }

        public bool RegisterNewCharacter(out int generatedId, Character character)
        {
            generatedId = -1;
            Character possibleDuplicate = GetCharacter(character.Id);
            if (character.Equals(possibleDuplicate)) return false;

            foreach (Character test in m_characters.Where(c => c.Name == character.Name))
            {
                return false;
            }

            character.Id = GenerateUniqueId();
            m_characters.Add(character);

            generatedId = character.Id;
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
    }
}
