using System;
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
        }
    }
}
