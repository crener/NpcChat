using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data;
using NpcChatSystem.Data.Character;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTest.Managers
{
    public class DialogTreeManagerTests
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
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            DialogTreeBranch branch2 = tree.CreateNewBranch();

            Assert.AreEqual(branch1.Id.DialogTreeId, branch2.Id.DialogTreeId);
            Assert.AreNotEqual(branch1.Id.DialogTreeBranchId, branch2.Id.DialogTreeBranchId);
        }

        [Test]
        public void StartPartCreation()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            Assert.IsNotNull(tree.GetStart());
            Assert.IsTrue(tree.GetStart().IsTreeRoot);
        }

        [Test]
        public void DialogModificationCallbackTests()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();

            bool created = false, destroyed = false;

            tree.OnBranchCreated += newBranch => created = true;
            tree.OnBranchRemoved += newBranch => destroyed = true;

            DialogTreeBranch branch = tree.CreateNewBranch();
            Assert.NotNull(branch);
            Assert.IsTrue(created, $"Failed to call '{nameof(tree.OnBranchCreated)}' callback");

            bool removeReported = tree.RemoveBranch(branch);
            Assert.IsTrue(removeReported);
            Assert.IsTrue(destroyed, $"Failed to call '{nameof(tree.OnBranchRemoved)}' callback");
        }

        [Test]
        public void DialogHasId()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            Assert.IsTrue(m_project.ProjectDialogs.HasDialog(tree.Id));

            DialogTreeBranch branch = tree.CreateNewBranch();
            Assert.NotNull(branch);
            Assert.IsTrue(m_project.ProjectDialogs.HasDialog(branch.Id));

            DialogSegment dialogSegment = branch.CreateNewDialog(CharacterId.DefaultId);
            Assert.NotNull(dialogSegment);
            Assert.IsTrue(m_project.ProjectDialogs.HasDialog(dialogSegment.Id));
        }

        /// <summary>
        /// Same as <see cref="DialogHasId"/> Except that it tests for negative results instead of positive ones
        /// </summary>
        [Test]
        public void DialogHasIdFalse()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            Assert.IsFalse(m_project.ProjectDialogs.HasDialog(new DialogTreeIdentifier(tree.Id.DialogTreeId + 1)));
            Assert.IsFalse(m_project.ProjectDialogs.HasDialog(new DialogTreeIdentifier(tree.Id.DialogTreeId + 10)));

            DialogTreeBranch branch = tree.CreateNewBranch();
            Assert.NotNull(branch);
            Assert.IsFalse(m_project.ProjectDialogs.HasDialog(new DialogTreeBranchIdentifier(branch.Id, branch.Id.DialogTreeBranchId + 1)));
            Assert.IsFalse(m_project.ProjectDialogs.HasDialog(new DialogTreeBranchIdentifier(branch.Id, branch.Id.DialogTreeBranchId + 10)));

            DialogSegment dialogSegment = branch.CreateNewDialog(CharacterId.DefaultId);
            Assert.NotNull(dialogSegment);
            Assert.IsFalse(m_project.ProjectDialogs.HasDialog(new DialogSegmentIdentifier(dialogSegment.Id, dialogSegment.Id.DialogSegmentId + 1)));
            Assert.IsFalse(m_project.ProjectDialogs.HasDialog(new DialogSegmentIdentifier(dialogSegment.Id, dialogSegment.Id.DialogSegmentId + 10)));
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(555)]
        public void DialogGetter(int offset)
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            Assert.IsNotNull(tree);
            Assert.IsNotNull(m_project.ProjectDialogs[tree]);
            DialogTreeIdentifier fakeTreeId = new DialogTreeIdentifier(tree.Id.DialogTreeId + offset);
            Assert.IsNull(m_project.ProjectDialogs[fakeTreeId]);

            DialogTreeBranch branch = tree.CreateNewBranch();
            Assert.IsNotNull(branch);
            Assert.IsNotNull(m_project.ProjectDialogs[branch]);
            DialogTreeBranchIdentifier fakeBranchId = new DialogTreeBranchIdentifier(branch.Id, branch.Id.DialogTreeBranchId + offset);
            Assert.IsNull(m_project.ProjectDialogs[fakeBranchId]);

            DialogSegment dialogSegment = branch.CreateNewDialog(CharacterId.DefaultId);
            Assert.IsNotNull(dialogSegment);
            Assert.IsNotNull(m_project.ProjectDialogs[dialogSegment.Id]);
            DialogSegmentIdentifier fakeSegmentId = new DialogSegmentIdentifier(dialogSegment.Id, dialogSegment.Id.DialogSegmentId + offset);
            Assert.IsNull(m_project.ProjectDialogs[fakeSegmentId]);
        }
    }
}
