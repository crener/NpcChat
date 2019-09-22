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
            Assert.IsTrue(tree.GetStart().isTreeRoot);
        }

        [Test]
        public void DialogModificationCallbackTests()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();

            bool created = false, destroyed = false;

            tree.BranchCreated += newBranch => created = true;
            tree.BranchRemoved += newBranch => destroyed = true;

            DialogTreeBranch branch = tree.CreateNewBranch();
            Assert.NotNull(branch);
            Assert.IsTrue(created, $"Failed to call '{nameof(tree.BranchCreated)}' callback");

            bool removeReported = tree.RemoveBranch(branch);
            Assert.IsTrue(removeReported);
            Assert.IsTrue(destroyed,$"Failed to call '{nameof(tree.BranchRemoved)}' callback");
        }
    }
}
