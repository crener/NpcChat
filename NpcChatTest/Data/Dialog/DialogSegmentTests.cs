using System;
using System.Linq;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogParts;
using NpcChatSystem.System.TypeStore;
using NpcChatSystem.System.TypeStore.Stores;
using NUnit.Framework;

namespace NpcChatTest.Data.Dialog
{
    public class DialogSegmentTests
    {
        [Test]
        public void CharacterIdentification()
        {
            NpcChatProject project = new NpcChatProject();

            int id;
            if(project.ProjectCharacters.RegisterNewCharacter(out id, new NpcChatSystem.Data.CharacterData.Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();
                DialogSegment segment = branch.CreateNewDialog(id);

                Assert.NotNull(segment);
                Assert.AreEqual(id, segment.CharacterId);
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }

        [TestCase(typeof(DialogText))]
        [TestCase(typeof(DialogCharacterTrait))]
        public void AddElement(Type dialogType)
        {
            NpcChatProject project = new NpcChatProject();

            if (project.ProjectCharacters.RegisterNewCharacter(out int id, new NpcChatSystem.Data.CharacterData.Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();

                DialogSegment segment = branch.CreateNewDialog(id);
                Assert.NotNull(segment);

                bool callback = false;
                segment.PropertyChanged += (sender, args) => callback = true;

                int before = segment.SegmentParts.Count;
                IDialogElement element = DialogTypeStore.Instance.CreateEntity(dialogType);
                segment.AddDialogElement(element);

                Assert.AreEqual(before + 1, segment.SegmentParts.Count);
                Assert.IsTrue(callback, "Failed to send callback for added type");
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }

        [Test]
        public void ClearElements()
        {
            NpcChatProject project = new NpcChatProject();

            if (project.ProjectCharacters.RegisterNewCharacter(out int id, new NpcChatSystem.Data.CharacterData.Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();

                DialogSegment segment = branch.CreateNewDialog(id);
                Assert.NotNull(segment);

                DialogText one = DialogTypeStore.Instance.CreateEntity<DialogText>();
                one.Text = "one";
                segment.AddDialogElement(one);

                Assert.IsTrue(segment.SegmentParts.Count > 0);

                bool callback = false;
                segment.PropertyChanged += (sender, args) => callback = true;

                segment.ClearElements();

                Assert.AreEqual(0, segment.SegmentParts.Count);
                Assert.IsTrue(callback, "Failed to send callback for added type");
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }

        [Test]
        public void RemoveElement()
        {
            NpcChatProject project = new NpcChatProject();

            if (project.ProjectCharacters.RegisterNewCharacter(out int id, new NpcChatSystem.Data.CharacterData.Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();

                DialogSegment segment = branch.CreateNewDialog(id);
                Assert.NotNull(segment);
                segment.ClearElements();

                //ensure there are elements in the segment
                DialogText one = DialogTypeStore.Instance.CreateEntity<DialogText>();
                one.Text = "one";
                segment.AddDialogElement(one);
                DialogText two = DialogTypeStore.Instance.CreateEntity<DialogText>();
                one.Text = "two";
                segment.AddDialogElement(two);
                Assert.IsTrue(segment.SegmentParts.Count > 0);

                bool callback = false;
                segment.PropertyChanged += (sender, args) => callback = true;

                int before = segment.SegmentParts.Count;
                segment.RemoveDialogElement(one);

                Assert.AreEqual(before - 1, segment.SegmentParts.Count);
                Assert.IsTrue(callback, "Failed to send callback for added type");

                //ensure correct element is actually removed
                Assert.IsFalse(segment.SegmentParts.Contains(one));
                Assert.IsTrue(segment.SegmentParts.Contains(two));
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }

        [Test]
        public void ChangeCharacter()
        {
            NpcChatProject project = new NpcChatProject();

            if (project.ProjectCharacters.RegisterNewCharacter(out int bill, new NpcChatSystem.Data.CharacterData.Character("bill")) && 
                project.ProjectCharacters.RegisterNewCharacter(out int tommy, new NpcChatSystem.Data.CharacterData.Character("tommy")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();

                DialogSegment segment = branch.CreateNewDialog(bill);
                Assert.NotNull(segment);
                Assert.AreEqual(segment.CharacterId, bill, "Unexpected Character");

                bool callback = false;
                segment.PropertyChanged += (sender, args) => callback = true;

                segment.ChangeCharacter(tommy);

                Assert.AreEqual(segment.CharacterId, tommy, "Failed change character");
                Assert.IsTrue(callback, "Failed to send callback for character change");

                callback = false;
                segment.ChangeCharacter(bill);

                Assert.AreEqual(segment.CharacterId, bill, "Failed change character");
                Assert.IsTrue(callback, "Failed to send callback for character change");
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }

        [Test]
        public void ChangeCharacterSame()
        {
            NpcChatProject project = new NpcChatProject();

            if (project.ProjectCharacters.RegisterNewCharacter(out int bill, new NpcChatSystem.Data.CharacterData.Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();

                DialogSegment segment = branch.CreateNewDialog(bill);
                Assert.NotNull(segment);
                Assert.AreEqual(segment.CharacterId, bill, "Unexpected Character");

                bool callback = false;
                segment.PropertyChanged += (sender, args) => callback = true;

                segment.ChangeCharacter(bill);

                Assert.AreEqual(segment.CharacterId, bill, "character changed for no reason");
                Assert.IsFalse(callback, "Failed to send callback for character change");
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }

        [TestCase("This is a test")]
        [TestCase("This text should be the segment text")]
        public void Text(string text)
        {
            NpcChatProject project = new NpcChatProject();

            if (project.ProjectCharacters.RegisterNewCharacter(out int bill, new NpcChatSystem.Data.CharacterData.Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                DialogTreeBranch branch = tree.CreateNewBranch();

                DialogSegment segment = branch.CreateNewDialog(bill);
                Assert.NotNull(segment);
                Assert.AreEqual(segment.CharacterId, bill, "Unexpected Character");

                segment.ClearElements();
                Assert.AreEqual(0, segment.SegmentParts.Count);

                segment.AddDialogElement(new DialogText(){Text = text });
                Assert.AreEqual(text, segment.Text);
            }
            else
            {
                Assert.Fail("Failed to create character");
            }
        }
    }
}
