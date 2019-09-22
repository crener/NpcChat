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
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogTreeBranch part2 = tree.CreateNewBranch();
            
            Assert.AreNotEqual(branch.Id.DialogTreeId, part2.Id.DialogTreeId);
        }

        [Test]
        public void StartPartCreation()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            Assert.IsNotNull(tree.GetStart());
            Assert.IsTrue(tree.GetStart().isTreeRoot);
        }
    }
}
