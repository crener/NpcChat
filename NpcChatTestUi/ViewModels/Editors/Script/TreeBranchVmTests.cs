using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NpcChat.Backend.Interfaces;
using NpcChat.ViewModels.Editors.Script;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.Editors.Script
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

            Assert.IsFalse(aBranch.AreBranchLinksPossible, "Branch links are possible as there are no other branches");
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";

            Assert.IsTrue(aBranch.AreBranchLinksPossible);
            Assert.IsTrue(aBranch.PotentialBranchLinks.Contains(b));
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
            branchTest.Branches.Add(aBranch);

            //no relation between the branches so all links are possible
            Assert.IsTrue(aBranch.AreBranchLinksPossible);
            Assert.IsTrue(aBranch.PotentialBranchLinks.Any(s => s == b));
            Assert.GreaterOrEqual(0, aBranch.SelectedBranchLinkIndex);
            Assert.IsTrue(bBranch.AreBranchLinksPossible, "B should be able to add A as a child");
            Assert.IsTrue(bBranch.PotentialBranchLinks.Any(s => s == a));
            Assert.GreaterOrEqual(0, bBranch.SelectedBranchLinkIndex);

            // link a to b so that the options for both branches are limited
            RelationshipCreate(a, b);

            //A is parent of B so there should be no possible actions
            Assert.IsFalse(aBranch.AreBranchLinksPossible);
            Assert.IsFalse(bBranch.AreBranchLinksPossible);
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);
            Assert.AreEqual(0, bBranch.PotentialBranchLinks.Count);
            Assert.Less(aBranch.SelectedBranchLinkIndex, 0);
            Assert.Less(bBranch.SelectedBranchLinkIndex, 0);
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

            Assert.IsFalse(aBranch.AreBranchLinksPossible, "Branch links are possible as there are no other branches");
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);

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

            Assert.IsFalse(cBranch.AreBranchLinksPossible, "A and B are parents or indirect parents of C");
            Assert.AreEqual(0, cBranch.PotentialBranchLinks.Count);
            Assert.IsTrue(aBranch.AreBranchLinksPossible, "A and B are parents or indirect parents of C");
            Assert.AreEqual(1, aBranch.PotentialBranchLinks.Count);
            Assert.IsTrue(aBranch.PotentialBranchLinks.Any(l => l == c));
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
            branchTest.Branches.Add(aBranch);

            //no relation between the branches so all links are possible
            Assert.IsTrue(aBranch.AreBranchLinksPossible);
            Assert.IsTrue(aBranch.PotentialBranchLinks.Any(s => s == b));
            Assert.IsTrue(bBranch.AreBranchLinksPossible, "B should be able to add A as a child");
            Assert.IsTrue(bBranch.PotentialBranchLinks.Any(s => s == a));

            // link a to b so that the options for both branches are limited
            RelationshipCreate(a, b);

            //A is parent of B so there should be no possible actions
            Assert.IsFalse(aBranch.AreBranchLinksPossible);
            Assert.IsFalse(bBranch.AreBranchLinksPossible);
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);
            Assert.AreEqual(0, bBranch.PotentialBranchLinks.Count);

            RelationshipDestroy(a, b);

            //no relation between the branches so all links should be possible
            Assert.IsTrue(aBranch.AreBranchLinksPossible);
            Assert.AreEqual(1, aBranch.PotentialBranchLinks.Count);
            Assert.IsTrue(aBranch.PotentialBranchLinks.Any(s => s == b));
            Assert.IsTrue(bBranch.AreBranchLinksPossible, "B should be able to add A as a child");
            Assert.IsTrue(bBranch.PotentialBranchLinks.Any(s => s == a));
            Assert.AreEqual(1, bBranch.PotentialBranchLinks.Count);

            RelationshipCreate(a, b);

            //A is parent of B so there should be no possible actions
            Assert.IsFalse(aBranch.AreBranchLinksPossible);
            Assert.IsFalse(bBranch.AreBranchLinksPossible);
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);
            Assert.AreEqual(0, bBranch.PotentialBranchLinks.Count);
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

            Assert.IsFalse(aBranch.AreBranchLinksPossible, "Branch links are possible as there are no other branches");
            Assert.AreEqual(0, aBranch.PotentialBranchLinks.Count);

            DialogTreeBranch b = tree.CreateNewBranch();
            b.Name = "B";
            TreeBranchVM bBranch = new TreeBranchVM(project, branchTest, b);
            branchTest.Branches.Add(aBranch);

            //no relation between the branches so all links are possible
            Assert.IsTrue(aBranch.AreBranchLinksPossible);
            Assert.IsTrue(aBranch.PotentialBranchLinks.Any(s => s == b));
            Assert.GreaterOrEqual(0, aBranch.SelectedBranchLinkIndex);
            Assert.IsTrue(bBranch.AreBranchLinksPossible);
            Assert.IsTrue(bBranch.PotentialBranchLinks.Any(s => s == a));
            Assert.GreaterOrEqual(0, bBranch.SelectedBranchLinkIndex);
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

            public event Action<IReadOnlyList<TreeBranchVM>> OnVisibleBranchChange;
            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
