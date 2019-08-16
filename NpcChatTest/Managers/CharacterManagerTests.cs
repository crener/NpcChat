using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data;
using NUnit.Framework;

namespace NpcChatTest.Managers
{
    public class CharacterManagerTests
    {
        [Test]
        public void AddCharacter()
        {
            NpcChatProject project = new NpcChatProject();

            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);

            int id;
            bool success = NpcChatProject.Characters.RegisterNewCharacter(out id, character);
            Assert.IsTrue(success);
            Assert.AreNotEqual(Character.PreRegisteredId, id);
        }

        [Test]
        public void AddDuplicateCharacter()
        {
            NpcChatProject project = new NpcChatProject();

            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);

            int id;
            bool success = NpcChatProject.Characters.RegisterNewCharacter(out id, character);
            Assert.IsTrue(success);
            Assert.AreNotEqual(Character.PreRegisteredId, id);

            success = NpcChatProject.Characters.RegisterNewCharacter(out id, character);
            Assert.IsFalse(success);
        }

        [Test]
        public void AddDuplicateCharacter2()
        {
            NpcChatProject project = new NpcChatProject();

            Character character = new Character("Daisy");
            Assert.AreEqual(Character.PreRegisteredId, character.Id);

            int id;
            bool success = NpcChatProject.Characters.RegisterNewCharacter(out id, character);
            Assert.IsTrue(success);
            Assert.AreNotEqual(Character.PreRegisteredId, id);

            character = NpcChatProject.Characters.GetCharacter(id);
            success = NpcChatProject.Characters.RegisterNewCharacter(out id, character);
            Assert.IsFalse(success);
        }

        [TestCase(23)]
        [TestCase(0)]
        [TestCase(-1)]
        public void HasCharacterFalse(int id)
        {
            NpcChatProject project = new NpcChatProject();
            Assert.IsFalse(NpcChatProject.Characters.HasCharacter(id));
        }

        [Test]
        public void HasCharacterTrue2()
        {
            NpcChatProject project = new NpcChatProject();
            Character character = new Character("Daisy");

            int id;
            Assert.IsTrue(NpcChatProject.Characters.RegisterNewCharacter(out id, character));
            Assert.IsTrue(NpcChatProject.Characters.HasCharacter(id));
        }
    }
}
