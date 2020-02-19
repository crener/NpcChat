using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.Character;
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
        public void DuplicateNameTest()
        {
            NpcChatProject project = new NpcChatProject();
            bool result = project.ProjectCharacters.RegisterNewCharacter(out int gen, "Person");
            Assert.IsTrue(result);
            bool result2 = project.ProjectCharacters.RegisterNewCharacter(out int gen2, "Person");
            Assert.IsTrue(result);

            Assert.AreNotEqual(Character.PreRegisteredId, gen);
            Assert.AreNotEqual(Character.PreRegisteredId, gen2);
            Assert.AreEqual(project.ProjectCharacters.GetCharacter(gen).Id, gen);
            Assert.AreEqual(project.ProjectCharacters.GetCharacter(gen2).Id, gen2);
            Assert.AreNotEqual(gen2, gen);
        }

        [Test]
        public void NameChange()
        {
            const string person = "Person";
            NpcChatProject project = new NpcChatProject();
            bool result = project.ProjectCharacters.RegisterNewCharacter(out int gen, person);
            Assert.IsTrue(result);

            Character character = project.ProjectCharacters.GetCharacter(gen);
            Assert.AreEqual(character.Id, gen);
            Assert.AreEqual(character.Name, person);

            const string person2 = person + "_NEW!";
            character.Name = person2;

            Character character2 = project.ProjectCharacters.GetCharacter(gen);
            Assert.AreSame(character, character2);
            Assert.AreEqual(character2.Id, gen);
            Assert.AreEqual(person2, character.Name);
            Assert.AreEqual(person2, character2.Name);
        }

        [Test]
        public void NameChangeCallback()
        {
            const string person = "Person";
            NpcChatProject project = new NpcChatProject();
            bool result = project.ProjectCharacters.RegisterNewCharacter(out int gen, person);
            Assert.IsTrue(result);

            //register for callbacks
            bool callbackTriggered = false;
            project.ProjectCharacters.CharacterChanged += (id, changed) =>
            {
                Assert.AreEqual(gen, id);
                callbackTriggered = true;
            };

            Character character = project.ProjectCharacters.GetCharacter(gen);

            const string person2 = person + "_NEW!";
            character.Name = person2;

            Assert.AreEqual(person2, character.Name);
            Assert.IsTrue(callbackTriggered, "Change callback not fired, though the character name was changed!");
        }

        [Test]
        public void CharacterList()
        {
            const string name1 = "Person";
            const string name2 = "Person2";
            const string name3 = "Person3";
            NpcChatProject project = new NpcChatProject();
            Assert.AreEqual(0, project.ProjectCharacters.AvailableCharacters().Count);

            bool result = project.ProjectCharacters.RegisterNewCharacter(out int gen1, name1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, project.ProjectCharacters.AvailableCharacters().Count);
            Assert.Contains(name1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());

            result = project.ProjectCharacters.RegisterNewCharacter(out int gen2, name2);
            Assert.IsTrue(result);
            Assert.AreEqual(2, project.ProjectCharacters.AvailableCharacters().Count);
            Assert.Contains(name1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());
            Assert.Contains(name2, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen2, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());

            result = project.ProjectCharacters.RegisterNewCharacter(out int gen3, name3);
            Assert.IsTrue(result);
            Assert.AreEqual(3, project.ProjectCharacters.AvailableCharacters().Count);
            Assert.Contains(name1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());
            Assert.Contains(name2, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen2, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());
            Assert.Contains(name3, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen3, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());

            const string name4 = name2 + "00";
            Character char2 = project.ProjectCharacters.GetCharacter(gen2);
            char2.Name = name4;
            Assert.AreEqual(3, project.ProjectCharacters.AvailableCharacters().Count);
            Assert.Contains(name1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen1, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());
            Assert.Contains(name4, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen2, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());
            Assert.Contains(name3, project.ProjectCharacters.AvailableCharacters().Select(i => i.Name).ToList());
            Assert.Contains(gen3, project.ProjectCharacters.AvailableCharacters().Select(i => i.Id).ToList());
            Assert.IsFalse(project.ProjectCharacters.AvailableCharacters().Any(i => i.Name == name2),
                "Available Characters list still contains old name which should have been renamed (and thereby removed)");
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
            Assert.IsTrue(m_project.ProjectCharacters.RegisterNewCharacter(out int id, "Daisy"));
            Assert.IsTrue(m_project.ProjectCharacters.HasCharacter(id));
        }
    }
}
