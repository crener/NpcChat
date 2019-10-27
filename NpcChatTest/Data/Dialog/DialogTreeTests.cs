using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTest.Data.Dialog
{
    public class DialogTreeTests
    {
        [Test]
        public void Start()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            Assert.NotNull(start, "There should be a default start node when creating a dialog tree");
        }

        [Test]
        public void StartRemove()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            Assert.NotNull(start);

            Assert.IsTrue(tree.RemoveBranch(start));
            Assert.IsFalse(tree.HasBranch(start));

            start = tree.GetStart();
            Assert.Null(start);
        }

        [Test]
        public void HasTree()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            Assert.NotNull(start);
            Assert.IsTrue(tree.HasBranch(start));

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.IsTrue(tree.HasBranch(aBranch));
        }

        [Test]
        public void HasTreeFromSeparateTree()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();
            DialogTree tree2 = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start2 = tree2.GetStart();

            Assert.IsTrue(tree.HasBranch(start));
            Assert.IsTrue(tree2.HasBranch(start2));

            Assert.IsFalse(tree2.HasBranch(start));
            Assert.IsFalse(tree.HasBranch(start2));
        }

        [Test]
        public void RemoveBranch()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.NotNull(aBranch);

            Assert.IsTrue(tree.RemoveBranch(aBranch));
            Assert.IsFalse(tree.RemoveBranch(aBranch), "Removed branch which doesn't exist'");
        }

        [Test]
        public void GetBranch()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.AreSame(aBranch, tree.GetBranch(aBranch));
        }

        [Test]
        public void GetBranch2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch[] branches =
            {
                tree.CreateNewBranch(),
                tree.CreateNewBranch(),
                tree.CreateNewBranch(),
                tree.CreateNewBranch(),
            };

            Assert.AreSame(branches[0], tree.GetBranch(branches[0]));
            Assert.AreSame(branches[1], tree.GetBranch(branches[1]));
            Assert.AreSame(branches[2], tree.GetBranch(branches[2]));
            Assert.AreSame(branches[3], tree.GetBranch(branches[3]));
        }

        [Test]
        public void GetBranchAfterRemoval()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.AreSame(aBranch, tree.GetBranch(aBranch));

            tree.RemoveBranch(aBranch);
            Assert.Null(tree.GetBranch(aBranch));
        }

        [Test]
        public void GetBranchFromAnotherTree()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTree tree2 = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.Null(tree2.GetBranch(aBranch));
        }

        [Test]
        public void AddBranch()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            Assert.NotNull(start);

            DialogTreeBranch newBranch = tree.CreateNewBranch();
            Assert.NotNull(newBranch);
            Assert.AreNotSame(newBranch, start);
            Assert.AreNotEqual(newBranch, start);
            Assert.AreNotEqual(newBranch.Id, start.Id);
        }

        [Test]
        public void AddBranch2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            for (int i = 0; i < 10; i++)
            {
                DialogTreeBranch newBranch = tree.CreateNewBranch();
                Assert.NotNull(newBranch);
            }

            IReadOnlyList<DialogTreeBranchIdentifier> branches = tree.Branches;
            for (int inner = 0; inner < branches.Count; inner++)
                for (int outer = 0; inner < branches.Count; inner++)
                {
                    if (inner == outer) continue;
                    Assert.AreNotEqual(branches[inner], branches[outer]);
                    Assert.AreNotEqual(tree[branches[inner]], tree[branches[outer]]);
                }
        }

        [Test]
        public void NewBranchNameDuplication()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            List<string> newNames = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                DialogTreeBranch newBranch = tree.CreateNewBranch();
                Assert.NotNull(newBranch);

                newNames.Add(newBranch.Name);
            }

            for (int inner = 0; inner < newNames.Count; inner++)
                for (int outer = 0; inner < newNames.Count; inner++)
                {
                    if (inner == outer) continue;
                    Assert.AreNotEqual(newNames[inner], newNames[outer]);
                }
        }
    }
}
