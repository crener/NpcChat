using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NUnit.Framework;

namespace NpcChatTest.DataTypes
{
    class DialogTreePartTests
    {
        private NpcChatProject project;

        [SetUp]
        public void Setup()
        {
            project = new NpcChatProject();
        }

        [Test]
        public void DuplicateIdTest()
        {
            DialogTree tree = NpcChatProject.Dialogs.CreateNewDialogTree();
            TreePart part = tree.CreateNewBranch();
            DialogSegment one = part.CreateNewDialog(12);
            DialogSegment two = part.CreateNewDialog(13);

            Assert.AreNotEqual(one.Id, two.Id);
        }

        [Test]
        public void Contains()
        {
            DialogTree tree = NpcChatProject.Dialogs.CreateNewDialogTree();
            TreePart part = tree.CreateNewBranch();
            DialogSegment one = part.CreateNewDialog(12);
            DialogSegment two = part.CreateNewDialog(13);

            Assert.IsTrue(part.ContainsDialogSegment(one));
            Assert.IsTrue(part.ContainsDialogSegment(one.Id));

            Assert.IsTrue(part.ContainsDialogSegment(two));
            Assert.IsTrue(part.ContainsDialogSegment(two.Id));
        }

        [Test]
        public void Contains2()
        {
            DialogTree tree = NpcChatProject.Dialogs.CreateNewDialogTree();
            TreePart part = tree.CreateNewBranch();
            DialogSegment one = part.CreateNewDialog(12);
            
            Assert.IsTrue(part.ContainsDialogSegment(one));
            Assert.IsTrue(part.ContainsDialogSegment(one.Id));

            part.RemoveDialog(one);

            Assert.IsFalse(part.ContainsDialogSegment(one));
            Assert.IsFalse(part.ContainsDialogSegment(one.Id));
        }

        [Test]
        public void Contains3()
        {
            DialogTree tree = NpcChatProject.Dialogs.CreateNewDialogTree();
            TreePart part = tree.CreateNewBranch();
            DialogSegment one = part.CreateNewDialog(12);
            
            Assert.IsTrue(part.ContainsDialogSegment(one));
            Assert.IsTrue(part.ContainsDialogSegment(one.Id));

            //make an id that has a different initial identifier but an id of an existing segment
            DialogTree tree2 = NpcChatProject.Dialogs.CreateNewDialogTree();
            TreePart branch2 = tree2.CreateNewBranch();
            DialogSegmentIdentifier fakeId = new DialogSegmentIdentifier(branch2.Id, one.Id.DialogSegmentId);

            Assert.IsFalse(part.ContainsDialogSegment(fakeId));
        }
    }
}
