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
        private NpcChatProject project;

        [SetUp]
        public void Setup()
        {
            project = new NpcChatProject();
        }

        public void DuplicateIdTest()
        {
            DialogTree tree = NpcChatProject.Dialogs.CreateNewDialogTree();
            TreePart part = tree.CreateNewBranch();
            TreePart part2 = tree.CreateNewBranch();
            
            Assert.AreNotEqual(part.Id.DialogTreeId, part2.Id.DialogTreeId);
        }

    }
}
