using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.System.TypeStore;
using NpcChatSystem.System.TypeStore.Stores;
using NUnit.Framework;

namespace NpcChatTest.Data.Dialog.DialogParts
{
    public class DialogCharacterTraitTests
    {
        [Test]
        public void Instantiate()
        {
            DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>();
            Assert.NotNull(element);
            Assert.NotNull(element.Text);
        }

        [Test]
        public void HasElementName()
        {
            DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>();
            Assert.NotNull(element.ElementName);
        }

        [Test]
        public void IdChangedCallback()
        {
            DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>();

            bool changed = false;
            element.PropertyChanged += (s, a) => { changed = true; };

            element.CharacterId = 12;
            Assert.IsTrue(changed, "Failed to trigger PropertyChanged callback");
        }

        [Test]
        public void TraitChangedCallback()
        {
            DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>();

            bool changed = false;
            element.PropertyChanged += (s, a) => { changed = true; };

            element.CharacterTrait = "Name";
            Assert.IsTrue(changed, "Failed to trigger PropertyChanged callback");
            Assert.AreEqual("Name", element.CharacterTrait);
        }

        [Test]
        public void NoProject()
        {
            DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>();
            Assert.AreEqual("<???>", element.Text, "missing project should return a fallback value");
        }

        [Test]
        public void NoCharacter()
        {
            NpcChatProject project = new NpcChatProject();

            DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>(project);
            Assert.IsFalse(project.ProjectCharacters.HasCharacter(element.CharacterId));
            Assert.AreEqual("<???>", element.Text, "missing character should return a fallback value");
        }

        [Test]
        public void NameTrait()
        {
            NpcChatProject project = new NpcChatProject();
            if (project.ProjectCharacters.RegisterNewCharacter(out int id,
                new NpcChatSystem.Data.CharacterData.Character("Tim")))
            {
                DialogCharacterTrait element = DialogTypeStore.Instance.CreateElement<DialogCharacterTrait>(project);
                element.CharacterTrait = "Name";

                Assert.IsFalse(project.ProjectCharacters.HasCharacter(element.CharacterId));
                Assert.AreNotEqual("Tim", element.Text, "Character name doesn't match");

                element.CharacterId = id;
                Assert.IsTrue(project.ProjectCharacters.HasCharacter(element.CharacterId));
                Assert.AreEqual("Tim", element.Text, "Character name doesn't match");
            }
            else
            {
                Assert.Fail("Failed to create new character");
            }
        }
    }
}
