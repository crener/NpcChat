using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Util;
using NUnit.Framework;

namespace NpcChatTest.Data.Util
{
    public class DialogUtilitiesTests
    {
        [Test]
        public void DependencyCheckBasic()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch a = tree.GetStart();
            DialogTreeBranch b = tree.CreateNewBranch();
            DialogTreeBranch c = tree.CreateNewBranch();

            a.Name = "a";
            b.Name = "b";
            c.Name = "c";

            Assert.IsFalse(DialogUtilities.CheckForCircularDependency(tree, a, b));

            a.AddChild(b);

            Assert.IsTrue(DialogUtilities.CheckForCircularDependency(tree, b, a));
            Assert.IsFalse(DialogUtilities.CheckForCircularDependency(tree, b, c));

            b.AddChild(c);

            Assert.IsTrue(DialogUtilities.CheckForCircularDependency(tree, c, a));
        }

        [Test]
        public void DependencyCheckBranched()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch a = tree.GetStart();
            DialogTreeBranch b = tree.CreateNewBranch();
            DialogTreeBranch c = tree.CreateNewBranch();
            DialogTreeBranch d = tree.CreateNewBranch();

            a.Name = "a";
            b.Name = "b";
            c.Name = "c";
            c.Name = "d";

            a.AddChild(b);
            a.AddChild(c);

            Assert.IsTrue(DialogUtilities.CheckForCircularDependency(tree, c, a));
            Assert.IsFalse(DialogUtilities.CheckForCircularDependency(tree, c, d));
            Assert.IsFalse(DialogUtilities.CheckForCircularDependency(tree, c, b));
        }

        [Test]
        public void DependencyCheckBranchedChildren()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch a1 = tree.CreateNewBranch();
            DialogTreeBranch a2 = tree.CreateNewBranch();
            DialogTreeBranch b1 = tree.CreateNewBranch();
            DialogTreeBranch b2 = tree.CreateNewBranch();

            start.Name = nameof(start);
            a1.Name = nameof(a1);
            a2.Name = nameof(a2);
            b1.Name = nameof(b1);
            b2.Name = nameof(b2);

            start.AddChild(a1);
            a1.AddChild(a2);
            b1.AddChild(b2);

            Assert.IsFalse(DialogUtilities.CheckForCircularDependency(tree, a2, b1));
            a2.AddChild(b1);
            Assert.IsTrue(DialogUtilities.CheckForCircularDependency(tree, b2, a1));
        }

        [Test]
        public void DependencyCheckManyChildren()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();
            DialogTreeBranch a = tree.GetStart();
            DialogTreeBranch b = tree.CreateNewBranch();
            DialogTreeBranch c = tree.CreateNewBranch();

            a.Name = "a";
            b.Name = "b";
            c.Name = "c";

            a.AddChild(b);
            b.AddChild(c);

            Assert.IsFalse(DialogUtilities.CheckForCircularDependency(tree, a, c));
            Assert.IsTrue(DialogUtilities.CheckForCircularDependency(tree, c, a));
            Assert.IsTrue(DialogUtilities.CheckForCircularDependency(tree, c, b));
        }
    }
}
