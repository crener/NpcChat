using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChat.ViewModels.Editors.Script;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.Editors.Script
{
    public class ScriptPanelVmTests
    {
        [Test]
        public void NewBranchWhenEmpty()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            //ensure there are no branches
            if (tree.Branches.Count > 0)
                for (int i = tree.Branches.Count - 1; i >= 0; i--)
                {
                    tree.RemoveBranch(tree.Branches[i]);
                }
            Assert.AreEqual(0, tree.Branches.Count);

            ScriptPanelVM panel = new ScriptPanelVM(project, tree);
            Assert.AreEqual(0, panel.Branches.Count, "The tree has no branches so the panel shouldn't have any branches");
            
            panel.NewBranchCommand.Execute(null);
            Assert.AreEqual(1, panel.Branches.Count);
        }
    }
}
