using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChat.ViewModels.Panels.ScriptEditor;
using NpcChat.ViewModels.Panels.ScriptEditor.Util;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.ScriptEditor
{
    public abstract class TreeBranchVmTests
    {
        [Test]
        public void PossibleBranchLinkAdded()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch a = tree.GetStart();
            a.Name = "A";
            BranchTestScriptView branchTest = new BranchTestScriptView();
            TreeBranchVM aBranch = new TreeBranchVM(project, branchTest, a);
            branchTest.Branches.Add(aBranch);

            TestBranchPossibility(aBranch);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";

            TestBranchPossibility(aBranch, b);
        }

        [Test]
        public void PossibleBranchLinkAdded2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch a = tree.GetStart();
            a.Name = "A";
            BranchTestScriptView branchTest = new BranchTestScriptView();
            TreeBranchVM aBranch = new TreeBranchVM(project, branchTest, a);
            branchTest.Branches.Add(aBranch);

            Assert.IsFalse(aBranch.AreBranchLinksPossible, "Branch links are possible as there are no other branches");
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";
            TreeBranchVM bBranch = new TreeBranchVM(project, branchTest, b);
            branchTest.Branches.Add(bBranch);

            //no relation between the branches so all links are possible
            TestBranchPossibility(aBranch, b);
            TestBranchPossibility(bBranch, a);

            // link a to b so that the options for both branches are limited
            RelationshipCreate(a, b);

            //A is parent of B so there should be no possible actions
            TestBranchPossibility(aBranch);
            TestBranchPossibility(bBranch);
        }

        [Test]
        public void PossibleBranchLinkAdded3()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch a = tree.GetStart();
            a.Name = "A";
            BranchTestScriptView branchTest = new BranchTestScriptView();
            TreeBranchVM aBranch = new TreeBranchVM(project, branchTest, a);
            branchTest.Branches.Add(aBranch);

            TestBranchPossibility(aBranch);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";
            TreeBranchVM bBranch = new TreeBranchVM(project, branchTest, b);
            branchTest.Branches.Add(bBranch);

            RelationshipCreate(a, b);

            DialogTreeBranch c = tree.CreateNewBranch();
            c.Name = "C";
            TreeBranchVM cBranch = new TreeBranchVM(project, branchTest, c);
            branchTest.Branches.Add(cBranch);

            RelationshipCreate(b, c);

            TestBranchPossibility(cBranch);
            TestBranchPossibility(aBranch, c);
        }

        [Test]
        public void PossibleBranchLinkRemoved()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch a = tree.GetStart();
            a.Name = "A";
            BranchTestScriptView branchTest = new BranchTestScriptView();
            TreeBranchVM aBranch = new TreeBranchVM(project, branchTest, a);
            branchTest.Branches.Add(aBranch);

            Assert.IsFalse(aBranch.AreBranchLinksPossible, "Branch links are possible as there are no other branches");
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";
            TreeBranchVM bBranch = new TreeBranchVM(project, branchTest, b);
            branchTest.Branches.Add(bBranch);

            //no relation between the branches so all links are possible
            TestBranchPossibility(aBranch, b);
            TestBranchPossibility(bBranch, a);

            // link a to b so that the options for both branches are limited
            RelationshipCreate(a, b);

            //A is parent of B so there should be no possible actions
            TestBranchPossibility(aBranch);
            TestBranchPossibility(bBranch);

            RelationshipDestroy(a, b);

            //no relation between the branches so all links should be possible
            TestBranchPossibility(aBranch, b);
            TestBranchPossibility(bBranch, a);

            RelationshipCreate(a, b);

            //A is parent of B so there should be no possible actions
            TestBranchPossibility(aBranch);
            TestBranchPossibility(bBranch);
        }

        [Test]
        public void PossibleBranchLinkPreExisting()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch a = tree.GetStart();
            a.Name = "A";
            BranchTestScriptView branchTest = new BranchTestScriptView();
            TreeBranchVM aBranch = new TreeBranchVM(project, branchTest, a);
            branchTest.Branches.Add(aBranch);

            //no other branches so there shouldn't be any possibilities
            TestBranchPossibility(aBranch);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";
            RelationshipCreate(a, b);

            TreeBranchVM bBranch = new TreeBranchVM(project, branchTest, b);
            branchTest.Branches.Add(bBranch);

            //branches already linked so there shouldn't be any possible links
            TestBranchPossibility(aBranch);
            TestBranchPossibility(bBranch);
        }

        [Test]
        public void PossibleBranchLinkDeleted()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            BranchTestScriptView branchTest = new BranchTestScriptView();

            DialogTreeBranch a = tree.GetStart();
            a.Name = "A";
            TreeBranchVM aBranch = new TreeBranchVM(project, branchTest, a);
            branchTest.Branches.Add(aBranch);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";
            TreeBranchVM bBranch = new TreeBranchVM(project, branchTest, b);
            branchTest.Branches.Add(bBranch);

            DialogTreeBranch c = tree.CreateNewBranch();
            c.Name = "C";
            TreeBranchVM cBranch = new TreeBranchVM(project, branchTest, c);
            branchTest.Branches.Add(cBranch);

            RelationshipCreate(a, b);
            RelationshipCreate(b, c);

            TestBranchPossibility(aBranch, c);
            TestBranchPossibility(bBranch);
            TestBranchPossibility(cBranch);

            tree.RemoveBranch(b);

            TestBranchPossibility(aBranch, c);
            TestBranchPossibility(cBranch, a);
        }

        /// <summary>
        /// Performs a series of tests to ensure a tree branches possible links are correct based on the expected links in the <see cref="children"/> parameter
        /// </summary>
        /// <param name="branch">tree branch VM to test</param>
        /// <param name="children">all expected links, if any</param>
        private void TestBranchPossibility(TreeBranchVM branch, params DialogTreeBranch[] children)
        {
            bool shouldHaveLinks = children.Any();
            Assert.AreEqual(shouldHaveLinks, branch.AreBranchLinksPossible);
            if (shouldHaveLinks)
                Assert.GreaterOrEqual(0, branch.SelectedBranchLinkIndex);

            Assert.AreEqual(children.Length, branch.PotentialBranchLinks.Count, "Unexpected amount of children found");
            foreach (DialogTreeBranch link in children)
            {
                Assert.IsTrue(branch.PotentialBranchLinks.Any(s => s == link),
                    $"branch '{branch.DialogBranch.Name}' is expected to have '{link.Name}' as a potential link... but it doesn't!");
            }

            foreach(TreeBranchLinkInfoVM link in branch.BranchLinks)
            {
                if(children.Any(c => c == link.Child))
                {
                    Assert.Fail($"Potential branch link contained inside '{branch.DialogBranch.Name}' that is an actual branch link!");
                }
            }
        }

        protected abstract void RelationshipCreate(DialogTreeBranch parent, DialogTreeBranch child);
        protected abstract void RelationshipDestroy(DialogTreeBranch parent, DialogTreeBranch child);

        public class TreeBranchVmTestsParents : TreeBranchVmTests
        {
            protected override void RelationshipCreate(DialogTreeBranch parent, DialogTreeBranch child)
            {
                child.AddParent(parent);
            }

            protected override void RelationshipDestroy(DialogTreeBranch parent, DialogTreeBranch child)
            {
                child.RemoveParent(parent);
            }
        }

        public class TreeBranchVmTestsChild : TreeBranchVmTests
        {
            protected override void RelationshipCreate(DialogTreeBranch parent, DialogTreeBranch child)
            {
                parent.AddChild(child);
            }

            protected override void RelationshipDestroy(DialogTreeBranch parent, DialogTreeBranch child)
            {
                parent.RemoveChild(child);
            }
        }


        public class BranchTestScriptView : IScriptPanelVM
        {
            public ObservableCollection<TreeBranchVM> Branches { get; } = new ObservableCollection<TreeBranchVM>();
            public ICommand NewBranchCommand { get; } = null;
            public EditMode EditMode { get; set; }

            public void SetDialogTree(DialogTreeIdentifier dialogTreeId)
            {
                throw new NotImplementedException();
            }

            public DialogTreeBranchIdentifier AddNewBranch(DialogTreeBranchIdentifier parentId, bool updateView)
            {
                throw new NotImplementedException();
            }

            public readonly List<DialogTreeBranchIdentifier> RebaseHistory = new List<DialogTreeBranchIdentifier>();

            public void RebaseBranchList(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
            {
                RebaseHistory.Add(parent);
            }

            public void ClearBranchListAfterParent(DialogTreeBranchIdentifier parent)
            {
                throw new NotImplementedException();
            }

            public event Action<IReadOnlyList<TreeBranchVM>> OnVisibleBranchChange;
            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
