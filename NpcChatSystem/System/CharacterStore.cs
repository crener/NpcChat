using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data;
using NpcChatSystem.Data.Character;

namespace NpcChatSystem.System
{
    public class CharacterStore
    {
        private static readonly Random s_random = new Random();

        /// <summary>
        /// Called when a character is modified
        /// </summary>
        public event CharacterChangeEvent CharacterChanged;

        /// <summary>
        /// Called when a new character is added
        /// </summary>
        public event CharacterEvent CharacterAdded;

        /// <summary>
        /// Called when an existing character is removed
        /// </summary>
        public event CharacterEvent CharacterRemoved;

        /// <summary>
        /// Find a character by id
        /// </summary>
        /// <param name="id">id of the character</param>
        /// <returns>character if found, null if not found</returns>
        public Character GetCharacter(int id)
        {
            if (HasCharacter(id))
            {
                return m_characters[id];
            }

            return null;
        }

        public Character GetCharacter(CharacterId id)
        {
            return GetCharacter(id.Id);
        }

        public bool HasCharacter(int id)
        {
            if (id <= Character.PreRegisteredId) return false;
            return m_characters.ContainsKey(id);
        }

        private Dictionary<int, Character> m_characters = new Dictionary<int, Character>();

        public bool RegisterNewCharacter(out int generatedId, string name)
        {
            Character character = new Character(this, name);

            generatedId = character.Id = GenerateUniqueId();
            m_characters.Add(generatedId, character);

            character.PropertyChanged += CharacterPropChanged;
            CharacterAdded?.Invoke(generatedId);

            return true;
        }

        public bool RemoveCharacter(Character character)
        {
            return RemoveCharacter(character.Id);
        }

        public bool RemoveCharacter(int characterId)
        {
            if (!HasCharacter(characterId)) return false;

            m_characters.Remove(characterId);
            CharacterRemoved?.Invoke(characterId);

            return true;
        }

        public IList<CharacterId> AvailableCharacters()
        {
            List<CharacterId> characters = new List<CharacterId>(m_characters.Count);
            foreach (KeyValuePair<int, Character> character in m_characters)
            {
                characters.Add(new CharacterId(character.Value.Name, character.Value.Id));
            }

            return characters;
        }

        private void CharacterPropChanged(object e, PropertyChangedEventArgs args)
        {
            int id = (e as Character)?.Id ?? Character.PreRegisteredId;
            if (args.PropertyName == nameof(Character.Name))
            {
                CharacterChanged?.Invoke(id, UpdatedField.Name);
            }
            else if (args.PropertyName == nameof(Character.TraitNames))
            {
                CharacterChanged?.Invoke(id, UpdatedField.Trait);
            }
            else
            {
                CharacterChanged?.Invoke(id, UpdatedField.Unspecified);
            }
        }

        private int GenerateUniqueId()
        {
            while (true)
            {
                int i = s_random.Next(Character.PreRegisteredId + 1, int.MaxValue);
                if (!HasCharacter(i)) //char Id should always be 0 before being reassigned
                    return i;
            }
        }

        public Character this[int key] => GetCharacter(key);
        public Character this[CharacterId key] => GetCharacter(key);

        /// <summary>
        /// Event used to notify of a change in a characters information
        /// </summary>
        /// <param name="charId">Id of the character</param>
        /// <param name="changed">Data that was changed</param>
        public delegate void CharacterChangeEvent(int charId, UpdatedField changed);
        public delegate void CharacterEvent(int charId);

        public enum UpdatedField
        {
            Unspecified = 0,
            Name = 100,
            Trait = 200,
        }
    }
}
