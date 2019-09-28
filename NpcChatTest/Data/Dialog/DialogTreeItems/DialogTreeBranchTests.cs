using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTest.Data.Dialog.DialogTreeItems
{
    class DialogTreeBranchTests
    {
        private NpcChatProject m_project;

        [SetUp]
        public void Setup()
        {
            m_project = new NpcChatProject();
        }

        [Test]
        public void DuplicateIdTest()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);
            DialogSegment two = branch.CreateNewDialog(13);

            Assert.AreNotEqual(one.Id, two.Id);
        }

        [Test]
        public void Contains()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);
            DialogSegment two = branch.CreateNewDialog(13);

            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            Assert.IsTrue(branch.ContainsDialogSegment(two));
            Assert.IsTrue(branch.ContainsDialogSegment(two.Id));
        }

        [Test]
        public void Contains2()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);
            
            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            branch.RemoveDialog(one);

            Assert.IsFalse(branch.ContainsDialogSegment(one));
            Assert.IsFalse(branch.ContainsDialogSegment(one.Id));
        }

        [Test]
        public void Contains3()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);
            
            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            //make an id that has a different initial identifier but an id of an existing segment
            DialogTree tree2 = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch2 = tree2.CreateNewBranch();
            DialogSegmentIdentifier fakeId = new DialogSegmentIdentifier(branch2.Id, one.Id.DialogSegmentId);

            Assert.IsFalse(branch.ContainsDialogSegment(fakeId));
        }
    }
}
