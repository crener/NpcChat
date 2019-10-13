using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
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

            Assert.NotNull(start);
        }

        [Test]
        public void StartRemove()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            Assert.NotNull(start);

            Assert.IsTrue(tree.RemoveBranch(start));
            Assert.IsFalse(tree.HasTree(start));

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
            Assert.IsTrue(tree.HasTree(start));

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.IsTrue(tree.HasTree(aBranch));
        }

        [Test]
        public void HasTreeFromSeparateTree()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();
            DialogTree tree2 = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start2 = tree2.GetStart();

            Assert.IsTrue(tree.HasTree(start));
            Assert.IsTrue(tree2.HasTree(start2));

            Assert.IsFalse(tree2.HasTree(start));
            Assert.IsFalse(tree.HasTree(start2));
        }

        /// <summary>
        /// S -> A
        /// </summary>
        [Test]
        public void AddSelf()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.NotNull(aBranch);
            Assert.IsFalse(start.AddChild(start), "Should be able to add self");
            Assert.IsFalse(start.AddParent(start), "Should be able to add self");
        }

        /// <summary>
        /// S -> A
        /// </summary>
        [Test]
        public void AddChildren()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.NotNull(aBranch);

            Assert.IsTrue(start.AddChild(aBranch));

            Assert.IsTrue(aBranch.Parents.Contains(start));
            Assert.AreEqual(1, aBranch.Parents.Count);
            Assert.AreEqual(0, aBranch.Children.Count);

            Assert.IsTrue(start.Children.Contains(aBranch));
            Assert.AreEqual(0, start.Parents.Count);
            Assert.AreEqual(1, start.Children.Count);
        }

        /// <summary>
        /// S -> A
        /// </summary>
        [Test]
        public void AddChildrenDouble()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.NotNull(aBranch);

            Assert.IsTrue(start.AddChild(aBranch));
            Assert.IsFalse(start.AddChild(aBranch));

            Assert.IsTrue(aBranch.Parents.Contains(start));
            Assert.AreEqual(1, aBranch.Parents.Count);
            Assert.AreEqual(0, aBranch.Children.Count);

            Assert.IsTrue(start.Children.Contains(aBranch));
            Assert.AreEqual(0, start.Parents.Count);
            Assert.AreEqual(1, start.Children.Count);
        }

        /// <summary>
        /// S -> A
        /// </summary>
        [Test]
        public void AddParent()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.NotNull(aBranch);

            Assert.IsTrue(aBranch.AddParent(start));

            Assert.IsTrue(aBranch.Parents.Contains(start));
            Assert.AreEqual(1, aBranch.Parents.Count);
            Assert.AreEqual(0, aBranch.Children.Count);

            Assert.IsTrue(start.Children.Contains(aBranch));
            Assert.AreEqual(0, start.Parents.Count);
            Assert.AreEqual(1, start.Children.Count);
        }

        /// <summary>
        /// S -> A
        /// </summary>
        [Test]
        public void AddParentDouble()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();

            DialogTreeBranch aBranch = tree.CreateNewBranch();
            Assert.NotNull(aBranch);

            Assert.IsTrue(aBranch.AddParent(start));
            Assert.IsFalse(aBranch.AddParent(start));

            Assert.IsTrue(aBranch.Parents.Contains(start));
            Assert.AreEqual(1, aBranch.Parents.Count);
            Assert.AreEqual(0, aBranch.Children.Count);

            Assert.IsTrue(start.Children.Contains(aBranch));
            Assert.AreEqual(0, start.Parents.Count);
            Assert.AreEqual(1, start.Children.Count);
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
    }
}
