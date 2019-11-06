using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NpcChat.ViewModels.Panels.Script;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
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

        [Test]
        public void ClearBranchListAfterParent()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            Assert.AreEqual(1, tree.Branches.Count);

            bool branchesChanged = false;
            ScriptPanelVM script = new ScriptPanelVM(project, tree);
            script.OnVisibleBranchChange += (t) => branchesChanged = true;

            DialogTreeBranchIdentifier firstChild = script.AddNewBranch(start, true);
            DialogTreeBranchIdentifier secondChild = script.AddNewBranch(firstChild, true);

            // ensure all 3 branches are currently visible
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == start), $"Should be able to see '{nameof(start)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == firstChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == secondChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsTrue(branchesChanged, $"Branches changed since creation");

            branchesChanged = false;
            script.ClearBranchListAfterParent(secondChild);

            // ensure all 3 branches are still visible
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == start), $"Should be able to see '{nameof(start)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == firstChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == secondChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsFalse(branchesChanged, $"No branches should have changed");

            branchesChanged = false;
            script.ClearBranchListAfterParent(firstChild);

            // start and first child should be visible
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == start), $"Should be able to see '{nameof(start)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == firstChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsFalse(script.Branches.Any(b => b.DialogBranch == secondChild), $"Shouldn't be able to see '{nameof(secondChild)}'");
            Assert.IsTrue(branchesChanged, $"{nameof(firstChild)} removed from visible branches");

            branchesChanged = false;
            script.ClearBranchListAfterParent(start);

            // start should be visible
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == start), $"Should be able to see '{nameof(start)}'");
            Assert.IsFalse(script.Branches.Any(b => b.DialogBranch == firstChild), $"Shouldn't be able to see '{nameof(firstChild)}'");
            Assert.IsFalse(script.Branches.Any(b => b.DialogBranch == secondChild), $"Shouldn't be able to see '{nameof(secondChild)}'");
            Assert.IsTrue(branchesChanged, $"{nameof(start)} removed from visible branches");
        }

        [Test]
        public void ClearBranchListAfterParentRemoteBranch()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            Assert.AreEqual(1, tree.Branches.Count);

            DialogTreeBranch unusedBranch = tree.CreateNewBranch();
            bool branchesChanged = false;

            ScriptPanelVM script = new ScriptPanelVM(project, tree);
            script.OnVisibleBranchChange += (t) => branchesChanged = true;
            DialogTreeBranchIdentifier firstChild = script.AddNewBranch(start, true);
            DialogTreeBranchIdentifier secondChild = script.AddNewBranch(firstChild, true);

            // ensure all 3 branches are currently visible
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == start), $"Should be able to see '{nameof(start)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == firstChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == secondChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsFalse(script.Branches.Any(b => b.DialogBranch == unusedBranch), $"Shouldn't be able to see '{nameof(unusedBranch)}'");
            Assert.IsTrue(branchesChanged, $"Branches changed since creation");

            branchesChanged = false;
            script.ClearBranchListAfterParent(unusedBranch);

            // ensure same branches are still currently visible
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == start), $"Should be able to see '{nameof(start)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == firstChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsTrue(script.Branches.Any(b => b.DialogBranch == secondChild), $"Should be able to see '{nameof(firstChild)}'");
            Assert.IsFalse(script.Branches.Any(b => b.DialogBranch == unusedBranch), $"Shouldn't be able to see '{nameof(unusedBranch)}'");
            Assert.IsFalse(branchesChanged, $"No branches changed");
        }

        [Test]
        public void AddNewBranchCommand()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            bool branchesChanged = false;
            ScriptPanelVM script = new ScriptPanelVM(project, tree);
            script.OnVisibleBranchChange += (t) => branchesChanged = true;
            Assert.AreEqual(1, tree.Branches.Count);

            script.NewBranchCommand.Execute(null);
            Assert.AreEqual(2, tree.Branches.Count);
            Assert.IsTrue(branchesChanged, $"Command should have triggered change event");
        }
    }
}
