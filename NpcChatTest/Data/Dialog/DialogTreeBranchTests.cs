using System.Linq;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTest.Data.Dialog.DialogTreeItems
{
    class DialogTreeBranchTests
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
            DialogSegment one = branch.CreateNewDialog(12);
            DialogSegment two = branch.CreateNewDialog(13);

            Assert.AreNotEqual(one.Id, two.Id);
        }

        [Test]
        public void Contains()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);
            DialogSegment two = branch.CreateNewDialog(13);

            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            Assert.IsTrue(branch.ContainsDialogSegment(two));
            Assert.IsTrue(branch.ContainsDialogSegment(two.Id));
        }

        [Test]
        public void Contains2()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);

            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            Assert.IsTrue(branch.RemoveDialog(one));

            Assert.IsFalse(branch.ContainsDialogSegment(one));
            Assert.IsFalse(branch.ContainsDialogSegment(one.Id));
        }

        [Test]
        public void Contains3()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);

            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            //make an id that has a different initial identifier but an id of an existing segment
            DialogTree tree2 = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch2 = tree2.CreateNewBranch();
            DialogSegmentIdentifier fakeId = new DialogSegmentIdentifier(branch2.Id, one.Id.DialogSegmentId);

            Assert.IsFalse(branch.ContainsDialogSegment(fakeId));
        }

        [Test]
        public void NoRemove()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);

            Assert.IsTrue(branch.ContainsDialogSegment(one));
            Assert.IsTrue(branch.ContainsDialogSegment(one.Id));

            // remove existing
            Assert.IsTrue(branch.RemoveDialog(one.Id));

            Assert.IsFalse(branch.ContainsDialogSegment(one));
            Assert.IsFalse(branch.ContainsDialogSegment(one.Id));

            // remove not existing
            Assert.IsFalse(branch.RemoveDialog(one));
        }

        [Test]
        public void NullRemove()
        {
            DialogTree tree = m_project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch branch = tree.CreateNewBranch();
            DialogSegment one = branch.CreateNewDialog(12);

            // remove existing
            Assert.IsFalse(branch.RemoveDialog((DialogSegment)null));
            Assert.IsFalse(branch.RemoveDialog((DialogSegmentIdentifier)null));
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
        public void RemoveParent()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch aBranch = tree.CreateNewBranch();

            Assert.IsTrue(aBranch.AddParent(start));
            Assert.AreEqual(1, aBranch.Parents.Count);
            Assert.AreEqual(1, start.Children.Count);

            Assert.IsTrue(aBranch.RemoveParent(start));
            Assert.AreEqual(0, aBranch.Parents.Count);
            Assert.AreEqual(0, start.Children.Count);
        }

        [Test]
        public void RemoveChild()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "Start";
            DialogTreeBranch aBranch = tree.CreateNewBranch();
            aBranch.Name = "aBranch";

            Assert.IsTrue(aBranch.AddParent(start));
            Assert.AreEqual(1, aBranch.Parents.Count);
            Assert.AreEqual(1, start.Children.Count);

            Assert.IsTrue(start.RemoveChild(aBranch));
            Assert.AreEqual(0, aBranch.Parents.Count);
            Assert.AreEqual(0, start.Children.Count);
        }

        [Test]
        public void AddParentCallback()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch aBranch = tree.CreateNewBranch();

            bool parentCallback = false, childCallback = false;
            aBranch.OnBranchParentAdded += id => parentCallback = true;
            start.OnBranchChildAdded += id => childCallback = true;

            Assert.IsTrue(aBranch.AddParent(start));
            Assert.IsTrue(parentCallback, "Parent Added Callback failed");
            Assert.IsTrue(childCallback, "Child Added Callback failed");
        }

        [Test]
        public void AddChildCallback()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch aBranch = tree.CreateNewBranch();

            bool parentCallback = false, childCallback = false;
            aBranch.OnBranchParentAdded += id => parentCallback = true;
            start.OnBranchChildAdded += id => childCallback = true;

            Assert.IsTrue(start.AddChild(aBranch));
            Assert.IsTrue(parentCallback, "Parent Added Callback failed");
            Assert.IsTrue(childCallback, "Child Added Callback failed");
        }

        [Test]
        public void RemoveParentCallback()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch aBranch = tree.CreateNewBranch();

            Assert.IsTrue(start.AddChild(aBranch), "Cannot test removal if you can't add branches");
            Assert.AreEqual(1, start.Children.Count);
            Assert.AreEqual(1, aBranch.Parents.Count);

            bool parentCallback = false, childCallback = false;
            aBranch.OnBranchParentRemoved += id => parentCallback = true;
            start.OnBranchChildRemoved += id => childCallback = true;

            Assert.IsTrue(aBranch.RemoveParent(start));
            Assert.IsTrue(parentCallback, "Parent Added Callback failed");
            Assert.IsTrue(childCallback, "Child Added Callback failed");
        }

        [Test]
        public void RemoveChildCallback()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch aBranch = tree.CreateNewBranch();

            Assert.IsTrue(start.AddChild(aBranch), "Cannot test removal if you can't add branches");
            Assert.AreEqual(1, start.Children.Count);
            Assert.AreEqual(1, aBranch.Parents.Count);

            bool parentCallback = false, childCallback = false;
            aBranch.OnBranchParentRemoved += id => parentCallback = true;
            start.OnBranchChildRemoved += id => childCallback = true;

            Assert.IsTrue(start.RemoveChild(aBranch));
            Assert.IsTrue(parentCallback, "Parent Added Callback failed");
            Assert.IsTrue(childCallback, "Child Added Callback failed");
        }
    }
}
