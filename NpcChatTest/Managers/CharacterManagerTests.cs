using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.CharacterData;
using NUnit.Framework;

namespace NpcChatTest.Managers
{
    public class CharacterManagerTests
    {
        private NpcChatProject m_project;

        [SetUp]
        public void Setup()
        {
            m_project = new NpcChatProject();
        }

        [Test]
        public void AddCharacter()
        {
            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);

            int id;
            bool success = m_project.ProjectCharacters.RegisterNewCharacter(out id, character);
            Assert.IsTrue(success);
            Assert.AreNotEqual(Character.PreRegisteredId, id);
        }

        [Test]
        public void AddDuplicateCharacter()
        {
            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);

            int id;
            bool success = m_project.ProjectCharacters.RegisterNewCharacter(out id, character);
            Assert.IsTrue(success);
            Assert.AreNotEqual(Character.PreRegisteredId, id);

            success = m_project.ProjectCharacters.RegisterNewCharacter(out id, character);
            Assert.IsFalse(success);
        }

        [Test]
        public void AddDuplicateCharacter2()
        {
            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);

            int id;
            bool success = m_project.ProjectCharacters.RegisterNewCharacter(out id, character);
            Assert.IsTrue(success);
            Assert.AreNotEqual(Character.PreRegisteredId, id);

            Character? newCharacter = m_project.ProjectCharacters.GetCharacter(id);
            Assert.IsTrue(newCharacter.HasValue);
            character = newCharacter.Value;

            success = m_project.ProjectCharacters.RegisterNewCharacter(out id, character);
            Assert.IsFalse(success);
        }

        [TestCase(23)]
        [TestCase(0)]
        [TestCase(-1)]
        public void HasCharacterFalse(int id)
        {
            Assert.IsFalse(m_project.ProjectCharacters.HasCharacter(id));
        }

        [Test]
        public void HasCharacterTrue2()
        {
            Character character = new Character("Daisy");

            int id;
            Assert.IsTrue(m_project.ProjectCharacters.RegisterNewCharacter(out id, character));
            Assert.IsTrue(m_project.ProjectCharacters.HasCharacter(id));
        }
    }
}
