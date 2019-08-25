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
    public class DialogSegmentTests
    {
        [Test]
        public void CharacterIdentification()
        {
            NpcChatProject project = new NpcChatProject();

            int id;
            if(project.ProjectCharacters.RegisterNewCharacter(out id, new Character("bill")))
            {
                DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
                TreePart branch = tree.CreateNewBranch();
                DialogSegment segment = branch.CreateNewDialog(id);

                Assert.NotNull(segment);
                Assert.AreEqual(id, segment.CharacterId);
            }
        }
    }
}
