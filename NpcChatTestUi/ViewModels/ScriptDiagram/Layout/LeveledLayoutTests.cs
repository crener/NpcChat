using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DynamicData;
using NodeNetwork.ViewModels;
using NpcChat.ViewModels.Panels.ScriptDiagram.Layout;
using NpcChat.ViewModels.Panels.ScriptDiagram.Node;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.ScriptDiagram.Layout
{
    public class LeveledLayoutTests : LayoutTests
    {
        protected override ILayout CreateLayoutInstance()
        {
            return new LeveledLayout();
        }
    }

    public abstract class LayoutTests
    {
        private const string c_startBranch = "Start";
        private const string c_branch = "branch ";

        protected abstract ILayout CreateLayoutInstance();

        [Test]
        public void NoNetwork()
        {
            ILayout layout = CreateLayoutInstance();
            layout.Layout(null);

            Assert.AreEqual(layout.Columns, 0);
            Assert.AreEqual(layout.Rows, 0);
        }

        [Test]
        public void NoNodes()
        {
            NetworkViewModel network = new NetworkViewModel();
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(layout.Columns, 0);
            Assert.AreEqual(layout.Rows, 0);
        }

        /// <summary>
        /// basic tree with a single node
        ///  s
        /// </summary>
        [Test]
        public void BasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();

            start.Name = c_branch + "s";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(1, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(1, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestRow(layout, 0, start);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch going to two
        ///  s -> 1
        ///    -> 2
        /// </summary>
        [Test]
        public void BasicLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(2, layout.Columns, "Unexpected amount of columns!");
            Assert.GreaterOrEqual(layout.Rows, 2, "Unexpected amount of rows!");
            Assert.LessOrEqual(layout.Rows, 3, "Unexpected amount of rows!");

            TestRow(layout, 0, start);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            int branch2Row = TestRowAny(layout, branch2, 0, 1, -1);
            Assert.AreNotEqual(branch1Row, branch2Row);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch going to tree
        ///    -> 1
        ///  s -> 2
        ///    -> 3
        /// </summary>
        [Test]
        public void BasicLayout3()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch3 = tree.CreateNewBranch(start);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(2, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(3, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch2, branch3);

            TestRow(layout, 0, start);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            int branch2Row = TestRowAny(layout, branch2, 0, 1, -1);
            int branch3Row = TestRowAny(layout, branch3, 0, 1, -1);
            Assert.AreNotEqual(branch1Row, branch2Row);
            Assert.AreNotEqual(branch1Row, branch3Row);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch going to tree
        ///    -> 1
        ///  s -> 2 -> 4 (2 in middle due to longest dependency chain)
        ///    -> 3
        /// </summary>
        [Test]
        public void BasicLayout4()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch3 = tree.CreateNewBranch(start);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch2);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(3, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch2, branch3);
            TestColumn(layout, 2, branch4);

            TestRow(layout, 0, start, branch2);
            int branch1Row = TestRowAny(layout, branch1, 1, -1);
            int branch3Row = TestRowAny(layout, branch3, 1, -1);
            Assert.AreNotEqual(branch1Row, branch3Row);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch going to tree
        ///         -> 2
        ///  s -> 1 -> 3
        ///         -> 4
        /// </summary>
        [Test]
        public void BasicLayout5()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch1);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(3, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2, branch3, branch4);

            TestRow(layout, 0, start, branch2);
            int branch2Row = TestRowAny(layout, branch2, 0, 1, -1);
            int branch3Row = TestRowAny(layout, branch3, 0, 1, -1);
            int branch4Row = TestRowAny(layout, branch4, 0, 1, -1);
            Assert.AreNotEqual(branch2Row, branch3Row);
            Assert.AreNotEqual(branch3Row, branch4Row);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch going to tree
        /// 
        ///  s -> 1 -> 2
        ///        \    \
        ///          3 -> 4
        /// </summary>
        [Test]
        public void Medium()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch2);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(4, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2, branch3);
            TestColumn(layout, 3, branch4);

            TestRow(layout, 0, start, branch1);
            int branch1Row = TestRowAny(layout, branch2, 0, 1, -1);
            int branch3Row = TestRowAny(layout, branch3, 0, 1, -1);
            Assert.AreNotEqual(branch1Row, branch3Row);

            TestRowAny(layout, branch4, branch1Row, branch3Row);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// Long linear chain for branches
        ///  s -> x -> x -> x -> x -> x (etc...)
        /// </summary>
        [TestCase(3)]
        [TestCase(18)]
        public void ChainLayout(int length)
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            List<DialogTreeBranchIdentifier> branches = new List<DialogTreeBranchIdentifier>(length) {start};
            for (; branches.Count < length;)
                branches.Add(tree.CreateNewBranch(branches.Last()));

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(length, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(1, layout.Rows, "Unexpected amount of rows!");

            for (int i = 0; i < length; i++)
            {
                Assert.AreSame(branches[i], ((BranchNode) layout[i, 0]).Branch);
            }

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///  s -> 1
        ///   \    \
        ///    ---> 2
        /// </summary>
        [Test]
        public void SkipBasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            branch1.AddChild(branch2);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2);

            TestRow(layout, 0, start, branch1);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowAny(layout, branch2, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch2); // branch 2 shouldn't be in same row as 1 as this hides the arrow 

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///  s -> 1
        ///   \  /
        ///    2
        /// </summary>
        [Test]
        public void SkipBasicLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            branch2.AddChild(branch1);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch2);
            TestColumn(layout, 2, branch1);

            TestRow(layout, 0, start, branch1);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowAny(layout, branch2, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch2); // branch 2 shouldn't be in same row as 1 as this hides the arrow

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///  1 -> s
        ///   \  / 
        ///    2
        /// </summary>
        [Test]
        public void BackPropagationSkipBasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            DialogTreeBranch branch2 = tree.CreateNewBranch();
            branch1.AddChild(branch2);
            branch1.AddChild(start);
            branch2.AddChild(start);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, -1, branch2);
            TestColumn(layout, -2, branch1);

            TestRow(layout, 0, start, branch1);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowAny(layout, branch2, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch2); // branch 2 shouldn't be in same row as 1 as this hides the arrow


            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        /// 2 ---
        ///  \    \
        ///   1 -> s
        /// </summary>
        [Test]
        public void BackPropagationSkipBasicLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            DialogTreeBranch branch2 = tree.CreateNewBranch();
            branch1.AddChild(start);
            branch2.AddChild(start);
            branch2.AddChild(branch1);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, -1, branch1);
            TestColumn(layout, -2, branch2);

            TestRow(layout, 0, start);
            TestRowAny(layout, branch2, 0, 1, -1);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///  1 -> s -> 3
        ///   \  / \  /
        ///    2    4
        /// </summary>
        [Test]
        public void SkipDoubleLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            DialogTreeBranch branch2 = tree.CreateNewBranch();
            branch1.AddChild(branch2);
            branch1.AddChild(start);
            branch2.AddChild(start);

            DialogTreeBranch branch3 = tree.CreateNewBranch();
            DialogTreeBranch branch4 = tree.CreateNewBranch();
            start.AddChild(branch3);
            start.AddChild(branch4);
            branch4.AddChild(branch3);

            start.Name = c_branch + "Start";
            branch1.Name = c_branch + "Branch 1";
            branch2.Name = c_branch + "Branch 2";
            branch3.Name = c_branch + "Branch 3";
            branch4.Name = c_branch + "Branch 4";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.GreaterOrEqual(layout.Rows, 2, "Unexpected amount of rows!");
            Assert.LessOrEqual(layout.Rows, 3, "Unexpected amount of rows!");

            Assert.AreEqual(5, layout.Columns);
            TestColumn(layout, 2, branch3);
            TestColumn(layout, 1, branch4);
            TestColumn(layout, 0, start);
            TestColumn(layout, -1, branch2);
            TestColumn(layout, -2, branch1);

            TestRow(layout, 0, start);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch1);
            int branch3Row = TestRowAny(layout, branch3, 0, 1, -1);
            TestRowDifferent(layout, branch3Row, branch4);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///
        ///    1 -> 3 -> 4 -> 5
        ///   /            \ 
        /// s -> 2 -------> 6 -> 7 
        /// </summary>
        [Test]
        public void SkipComplexLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch4);
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch4);
            branch6.AddParent(branch4);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";
            branch6.Name = c_branch + "6";
            branch7.Name = c_branch + "7";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(6, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch2);
            TestColumn(layout, 2, branch3);
            TestColumn(layout, 3, branch4);
            TestColumn(layout, 4, branch5, branch6);
            TestColumn(layout, 5, branch7);

            TestRow(layout, 0, start);
            int mainBranch = TestRowAny(layout, branch1, 0, -1, 1);
            TestRow(layout, mainBranch, branch1, branch3, branch4, branch5);
            TestRowDifferent(layout, 0, branch1, branch3, branch4, branch5);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///
        ///    1 -> 3 -> 4 -> 5
        ///   /            \ 
        /// s -> 2 -------> 6 -> 7 -> 8 (longest gets center)
        /// </summary>
        [Test]
        public void SkipComplexLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch4);
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch4);
            branch6.AddParent(branch4);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6);
            DialogTreeBranch branch8 = tree.CreateNewBranch(branch7);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";
            branch6.Name = c_branch + "6";
            branch7.Name = c_branch + "7";
            branch8.Name = c_branch + "8";

            // build network and layout
            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(6, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch2);
            TestColumn(layout, 2, branch3);
            TestColumn(layout, 3, branch4);
            TestColumn(layout, 4, branch5, branch6);
            TestColumn(layout, 5, branch7);
            TestColumn(layout, 6, branch8);

            TestRow(layout, 0, start, branch2, branch6, branch7, branch8);
            TestRowDifferent(layout, 0, branch1, branch3, branch4, branch5);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// link that goes behind the start node
        ///  1 -> s
        /// </summary>
        [Test]
        public void BackPropagationBasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            branch1.AddChild(start);

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(2, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(1, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, -1, branch1);

            TestRow(layout, 0, start, branch1);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// link that goes behind the start node
        /// 4 -> 3
        ///       \
        ///    s -> 1 -> 2
        /// </summary>
        [Test]
        public void BackPropagationBasicLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch();
            branch1.AddParent(branch3);
            DialogTreeBranch branch4 = tree.CreateNewBranch();
            branch3.AddParent(branch4);

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(4, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, -1, branch4);
            TestColumn(layout, 0, start, branch3);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2);

            TestRow(layout, 0, start, branch1, branch2);
            int branch3Row = TestRowAny(layout, branch3, 1, -1);
            int branch4Row = TestRowAny(layout, branch4, 1, -1);
            Assert.AreEqual(branch3Row, branch4Row, "branch 3 and 4 should be in line");

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// link that goes behind the start node
        /// 4 -> 3
        ///       \
        ///    s -> 1 -> 2
        /// </summary>
        [Test]
        public void BackPropagationBasicLayout2Alt()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch3 = tree.CreateNewBranch();
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            branch1.AddParent(branch3);

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(4, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, -1, branch4);
            TestColumn(layout, 0, start, branch3);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2);

            TestRow(layout, 0, start, branch1, branch2);
            int branch3Row = TestRowAny(layout, branch3, 1, -1);
            int branch4Row = TestRowAny(layout, branch4, 1, -1);
            Assert.AreEqual(branch3Row, branch4Row, "branch 3 and 4 should be in line");

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// start with a few nodes which then link backwards in a new set of branches
        ///      b1 -> b2
        ///               \
        ///  s -> 1 -> 2 -> 3
        /// </summary>
        [Test]
        public void BackPropagationComplexLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch2);
            DialogTreeBranch back1 = tree.CreateNewBranch();
            DialogTreeBranch back2 = tree.CreateNewBranch(back1);
            back2.AddChild(branch3);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            back1.Name = c_branch + "b1";
            back2.Name = c_branch + "b2";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(4, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, back1);
            TestColumn(layout, 2, branch2, back2);
            TestColumn(layout, 3, branch3);

            TestRow(layout, 0, start, branch1, branch2, branch3);
            TestRowDifferent(layout, 0, back1, back2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// start with a few nodes which then link backwards in a new set of branches
        ///  s
        ///   \
        ///    1
        ///   /
        ///  2
        ///   \
        ///    3
        ///   /
        ///  4
        ///   \
        ///    5  (etc...)
        /// </summary>
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(8)]
        [TestCase(17)]
        public void BackPropagationZigZagLayout(int length)
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            List<DialogTreeBranchIdentifier> starts = new List<DialogTreeBranchIdentifier>(length / 2);
            List<DialogTreeBranchIdentifier> ends = new List<DialogTreeBranchIdentifier>(length / 2);
            List<DialogTreeBranch> branches = new List<DialogTreeBranch>(length);

            DialogTreeBranch start = tree.GetStart();
            start.Name = c_branch + "Start";
            branches.Add(start);
            starts.Add(start);

            for (; branches.Count <= length;)
            {
                DialogTreeBranch newBranch = tree.CreateNewBranch();
                newBranch.Name = c_branch + "Branch " + branches.Count;

                if(branches.Count % 2 == 1)
                {
                    newBranch.AddParent(branches.Last());
                    branches.Add(newBranch);
                    ends.Add(newBranch);
                }
                else
                {
                    newBranch.AddChild(branches.Last());
                    branches.Add(newBranch);
                    starts.Add(newBranch);
                }
            }

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(2, layout.Columns, "Unexpected amount of columns!");

            TestColumn(layout, 0, starts.ToArray());
            TestColumn(layout, 1, ends.ToArray());

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to two (1, 2) then converging back to one branch (3)
        /// 
        ///   1
        ///  / \
        /// s   3
        ///  \ /
        ///   2
        /// </summary>
        [Test]
        public void ConvergenceLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            branch3.AddParent(branch2);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(3, layout.Columns, "Unexpected amount of columns!");
            Assert.GreaterOrEqual(layout.Rows, 1, "Unexpected amount of rows!");
            Assert.LessOrEqual(layout.Rows, 3, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch2);
            TestColumn(layout, 2, branch3);

            TestRow(layout, 0, start, branch3);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowAny(layout, branch2, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree which has many branches
        ///
        ///        2 -> 3   10 -> 11
        ///       /         /
        /// s -> 1 -> 6 -> 7 -> 9 -> 12 (longest path in center)
        ///       \
        ///        4 -> 5 -> 8
        /// </summary>
        [Test]
        public void Large1()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch2);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch4);
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6);
            DialogTreeBranch branch8 = tree.CreateNewBranch(branch5);
            DialogTreeBranch branch9 = tree.CreateNewBranch(branch7);
            DialogTreeBranch branch10 = tree.CreateNewBranch(branch7);
            DialogTreeBranch branch11 = tree.CreateNewBranch(branch10);
            DialogTreeBranch branch12 = tree.CreateNewBranch(branch9);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";
            branch6.Name = c_branch + "6";
            branch7.Name = c_branch + "7";
            branch8.Name = c_branch + "8";
            branch9.Name = c_branch + "9";
            branch10.Name = c_branch + "10";
            branch11.Name = c_branch + "11";
            branch12.Name = c_branch + "12";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(6, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(3, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2, branch6, branch4);
            TestColumn(layout, 3, branch3, branch7, branch5);
            TestColumn(layout, 4, branch10, branch9, branch8);
            TestColumn(layout, 5, branch11, branch12);

            TestRow(layout, 0, start, branch1, branch6, branch7, branch9, branch12);

            int branch2Row = TestRowAny(layout, branch2, 1, -1);
            TestRow(layout, branch2Row, branch2, branch3);

            int branch10Row = TestRowAny(layout, branch10, 1, -1);
            TestRow(layout, branch10Row, branch10, branch11);

            int branch4Row = TestRowAny(layout, branch4, 1, -1);
            TestRow(layout, branch4Row, branch4, branch5, branch8);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree which has many branches
        /// 
        /// s -> 1 -> 2 -> 3 -> 9 -> 10 (longest path in center)
        ///  \    \
        ///   \     4
        ///    5 -> 6 -> 7
        ///     \    \
        ///      ---> 8
        /// </summary>
        [Test]
        public void Large2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch2);
            DialogTreeBranch branch9 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch10 = tree.CreateNewBranch(branch9);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch5 = tree.CreateNewBranch(start);
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch5);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6);
            DialogTreeBranch branch8 = tree.CreateNewBranch(branch5);
            branch8.AddChild(branch6);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";
            branch6.Name = c_branch + "6";
            branch7.Name = c_branch + "7";
            branch8.Name = c_branch + "8";
            branch9.Name = c_branch + "9";
            branch10.Name = c_branch + "10";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(6, layout.Columns, "Unexpected amount of columns!");
            Assert.GreaterOrEqual(layout.Rows, 3, "Unexpected amount of rows!");
            Assert.LessOrEqual(layout.Rows, 4, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch5);
            TestColumn(layout, 2, branch2, branch6, branch4);
            TestColumn(layout, 3, branch3, branch7, branch8);
            TestColumn(layout, 4, branch9);
            TestColumn(layout, 5, branch10);

            TestRow(layout, 0, start, branch1, branch2, branch3, branch9, branch10);
            TestRowAny(layout, branch4, 1, 2, 3, -1, 2, -3);

            int branch5Row = TestRowAny(layout, branch5, 1, 2, -1, -2);
            TestRow(layout, branch5Row, branch5, branch6, branch7);

            TestRowDifferent(layout, 0, branch8);
            TestRowDifferent(layout, branch5Row, branch8);
            TestRowAny(layout, branch8, branch5Row - 1, branch5Row + 1);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree which has many branches
        ///        8         10 -> 11       12
        ///       /         /              /
        /// s -> 1 -> 2 -> 3 -> 4 -> 5 -> 6 -> 7
        ///                 \    \
        ///                  9    13
        /// </summary>
        [Test]
        public void Large3()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch2);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch4);
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch5);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6);
            DialogTreeBranch branch8 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch9 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch10 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch11 = tree.CreateNewBranch(branch10);
            DialogTreeBranch branch12 = tree.CreateNewBranch(branch6);
            DialogTreeBranch branch13 = tree.CreateNewBranch(branch4);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";
            branch6.Name = c_branch + "6";
            branch7.Name = c_branch + "7";
            branch8.Name = c_branch + "8";
            branch9.Name = c_branch + "9";
            branch10.Name = c_branch + "10";
            branch11.Name = c_branch + "11";
            branch12.Name = c_branch + "12";
            branch12.Name = c_branch + "13";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(8, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(3, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1);
            TestColumn(layout, 2, branch2, branch8);
            TestColumn(layout, 3, branch3);
            TestColumn(layout, 4, branch10, branch4, branch9);
            TestColumn(layout, 5, branch11, branch5, branch13);
            TestColumn(layout, 6, branch6);
            TestColumn(layout, 7, branch12, branch7);

            TestRow(layout, 0, start, branch1, branch2, branch3, branch4, branch5, branch6);
            int branch7Row = TestRowAny(layout, branch7, 0, 1, -1);
            int branch12Row = TestRowAny(layout, branch12, 0, 1, -1);
            Assert.AreNotEqual(branch7Row, branch12Row, "7 and 12 shouldn't be on the same row");

            TestRowAny(layout, branch8, 0, 1, -1);
            TestRowAny(layout, branch9, 0, 1, -1);
            TestRowAny(layout, branch13, 0, 1, -1);

            int branch10Row = TestRowAny(layout, branch10, 0, 1, -1);
            TestRow(layout, branch10Row, branch10, branch11);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to two (1, 2) then converging back to one branch (3)
        /// 
        ///   1 -------> 3
        ///  /            \
        /// s -> 2 -> 5 -> 4 (should be in center as it contains more nodes than top)
        /// </summary>
        [Test]
        public void ConvergenceDelayedLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch2);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            branch4.AddParent(branch5);

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(5, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch1, branch2);
            TestColumn(layout, 2, branch5);
            TestColumn(layout, 3, branch3);
            TestColumn(layout, 4, branch4);

            TestRow(layout, 0, start, branch2, branch5, branch4);
            TestRowDifferent(layout, 0, branch1, branch3);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// start with a few nodes which then link backwards in a new set of branches
        ///           4   s -> 1
        ///         /  \      /
        ///        5 -> 3 -> 2  
        /// </summary>
        [Test]
        public void BackPropagationSkipLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch();
            branch2.AddChild(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch();
            branch3.AddChild(branch2);
            DialogTreeBranch branch4 = tree.CreateNewBranch();
            branch4.AddChild(branch3);
            DialogTreeBranch branch5 = tree.CreateNewBranch();
            branch5.AddChild(branch4);
            branch5.AddChild(branch3);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(4, layout.Columns, "Unexpected amount of columns!");
            Assert.GreaterOrEqual(layout.Rows, 2, "Unexpected amount of rows!");
            Assert.LessOrEqual(layout.Rows, 3, "Unexpected amount of rows!");

            TestColumn(layout, 1, branch1);
            TestColumn(layout, 0, start, branch2);
            TestColumn(layout, -1, branch3);
            TestColumn(layout, -2, branch4);
            TestColumn(layout, -3, branch5);

            TestRow(layout, 0, start, branch1);
            TestRowDifferent(layout, 0, branch2, branch3, branch5);
            TestRowAny(layout, branch4, 0, 2, -2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// start with a few nodes which then link backwards in a new set of branches
        ///               s -> 1
        ///                   /
        ///        5 -> 3 -> 2
        ///         \  /
        ///          4
        /// </summary>
        [Test]
        public void BackPropagationSkipLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch();
            branch2.AddChild(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch();
            branch3.AddChild(branch2);
            DialogTreeBranch branch5 = tree.CreateNewBranch();
            branch5.AddChild(branch3);
            DialogTreeBranch branch4 = tree.CreateNewBranch();
            branch4.AddChild(branch3);
            branch5.AddChild(branch4);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(4, layout.Columns, "Unexpected amount of columns!");
            Assert.GreaterOrEqual(layout.Rows, 2, "Unexpected amount of rows!");
            Assert.LessOrEqual(layout.Rows, 3, "Unexpected amount of rows!");

            TestColumn(layout, 1, branch1);
            TestColumn(layout, 0, start, branch2);
            TestColumn(layout, -1, branch3);
            TestColumn(layout, -2, branch4);
            TestColumn(layout, -3, branch5);

            TestRow(layout, 0, start, branch1);
            TestRowDifferent(layout, 0, branch2, branch3, branch5);
            TestRowAny(layout, branch4, 0, 2, -2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to 3 (1, 2, 5)
        /// 
        ///    5
        ///  /  \
        /// s -> 1 -> 3 -> 4
        ///  \    \  /
        ///   ---> 2
        /// </summary>
        [Test]
        public void ComplexLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            branch1.AddChild(branch2);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            branch2.AddChild(branch3);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            DialogTreeBranch branch5 = tree.CreateNewBranch(start);
            branch5.AddChild(branch1);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(6, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(3, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            //TestColumn(layout, 1, branch5); // 5 and 1 could switch depending on implementation
            //TestColumn(layout, 2, branch1); // 5 and 1 could switch depending on implementation
            TestColumn(layout, 3, branch2);
            TestColumn(layout, 4, branch3);
            TestColumn(layout, 5, branch4);

            // testing rows is complex as there can be multiple valid combinations between branches 1, 2 and 5
            TestRow(layout, 0, start, branch3, branch4);
            //int branch5Row = TestRowAny(layout, branch5, 0, 1, -1);
            //TestRowAny(layout, branch1, 0, 1, -1);
            //TestRowDifferent(layout, branch5Row, branch1);
            //TestRowAny(layout, branch2, 1, -1);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to two (2, 5)
        /// s -> 5 -> 1 -> 3 -> 4
        ///  \         \  /
        ///   --------> 2
        /// </summary>
        [Test]
        public void ComplexLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch5 = tree.CreateNewBranch(start);
            DialogTreeBranch branch1 = tree.CreateNewBranch(branch5);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            branch1.AddChild(branch2);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            branch2.AddChild(branch3);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);

            start.Name = c_startBranch;
            branch1.Name = c_branch + "1";
            branch2.Name = c_branch + "2";
            branch3.Name = c_branch + "3";
            branch4.Name = c_branch + "4";
            branch5.Name = c_branch + "5";

            PrintKey(tree);
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            ILayout layout = CreateLayoutInstance();
            layout.Layout(network);

            Assert.AreEqual(6, layout.Columns, "Unexpected amount of columns!");
            Assert.AreEqual(2, layout.Rows, "Unexpected amount of rows!");

            TestColumn(layout, 0, start);
            TestColumn(layout, 1, branch5);
            TestColumn(layout, 2, branch1);
            TestColumn(layout, 3, branch2);
            TestColumn(layout, 4, branch3);
            TestColumn(layout, 5, branch4);

            TestRow(layout, 0, start, branch1, branch5, branch3, branch4);
            int branch1Row = TestRowAny(layout, branch1, 0, 1, -1);
            TestRowAny(layout, branch2, 0, 1, -1);
            TestRowDifferent(layout, branch1Row, branch2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// Prints a useful key for when your trying to decypher logs
        /// </summary>
        /// <param name="tree"></param>
        private void PrintKey(DialogTree tree)
        {
            Console.WriteLine($"--- Lookup for Tree {tree.Id.ToString("h")} Count: {tree.Branches.Count} ---");
            foreach (DialogTreeBranchIdentifier branch in tree.Branches.OrderBy(t => t.ToString()))
            {
                Console.WriteLine($"{branch.ToString("")}\t=> \t{branch.ToString("h")}");
            }

            Console.WriteLine($"-------------------------------------------------------------------");
        }

        /// <summary>
        /// make sure that all <paramref name="expectedBranches"/> are in <paramref name="column"/> and none are missing
        /// </summary>
        /// <param name="layout">layout to check</param>
        /// <param name="column">column to look in</param>
        /// <param name="expectedBranches">all nodes that should be in the column</param>
        private void TestColumn(ILayout layout, int column, params DialogTreeBranchIdentifier[] expectedBranches)
        {
            Assert.IsTrue(layout.ColumnMin <= column && layout.ColumnMin + layout.Columns > column, $"Missing column {column}");

            HashSet<DialogTreeBranchIdentifier> remainingBranches = new HashSet<DialogTreeBranchIdentifier>(expectedBranches);
            for (int row = layout.RowMin; row < layout.RowMin + layout.Rows; row++)
            {
                BranchNode model = layout[column, row] as BranchNode;
                if(model == null) continue;

                DialogTreeBranchIdentifier foundBranch = model.Branch;
                Assert.IsTrue(expectedBranches.Contains(foundBranch),
                    $"Column {column} contains unexpected branch, '{foundBranch.ToString("h")}'");
                remainingBranches.Remove(foundBranch);
            }

            Assert.IsEmpty(remainingBranches, $"Column {column} didn't contain all expected branches");
        }

        /// <summary>
        /// make sure that all <paramref name="expectedBranches"/> are in <paramref name="row"/> and none are missing
        /// </summary>
        /// <param name="layout">layout to check</param>
        /// <param name="row">row to look in</param>
        /// <param name="expectedBranches">all nodes that should be in the row</param>
        private void TestRow(ILayout layout, int row, params DialogTreeBranchIdentifier[] expectedBranches)
        {
            Assert.IsTrue(layout.RowMin <= row && layout.RowMin + layout.Rows >= row, $"Missing row {row}");

            HashSet<DialogTreeBranchIdentifier> remainingBranches = new HashSet<DialogTreeBranchIdentifier>(expectedBranches);

            for (int column = layout.ColumnMin; column < layout.ColumnMin + layout.Columns; column++)
            {
                DialogTreeBranchIdentifier foundBranch = (layout[column, row] as BranchNode)?.Branch;
                if(foundBranch == null) continue;

                remainingBranches.Remove(foundBranch);
            }

            Assert.IsEmpty(remainingBranches, $"Row {row} didn't contain all expected branches");
        }

        /// <summary>
        /// make sure that all <paramref name="expectedBranches"/> are not in <paramref name="avoidRow"/>
        /// </summary>
        /// <param name="layout">layout to check</param>
        /// <param name="avoidRow">row to look in</param>
        /// <param name="expectedBranches">all nodes that shouldn't be in the row</param>
        private void TestRowDifferent(ILayout layout, int avoidRow, params DialogTreeBranchIdentifier[] expectedBranches)
        {
            Assert.IsTrue(layout.RowMin <= avoidRow && layout.RowMin + layout.Rows > avoidRow, $"Missing row {avoidRow}");

            foreach (DialogTreeBranchIdentifier branch in expectedBranches)
            {
                // try to find the branch
                int foundRow = -1;
                for (int column = 0; column < layout.ColumnMin + layout.Columns; column++)
                {
                    //row = layout.NodeLevels[column].FindIndex(n => (n as BranchNode)?.Branch == branch);
                    for (int row = layout.RowMin; row < layout.RowMin + layout.Rows; row++)
                    {
                        if((layout[column, row] as BranchNode)?.Branch == branch)
                        {
                            foundRow = row;
                            break;
                        }
                    }

                    if(foundRow >= 0)
                    {
                        break;
                    }
                }

                Assert.GreaterOrEqual(foundRow, 0, $"Failed to find branch '{branch.ToString("h")}'");
                Assert.AreNotEqual(foundRow, avoidRow,
                    $"Found branch ({branch.ToString("h")}) on row {avoidRow} where it was not expected to be");
            }
        }

        /// <summary>
        /// make sure that <paramref name="branch"/> is in one of the <paramref name="possibleRows"/>
        /// </summary>
        /// <param name="layout">layout to check</param>
        /// <param name="branch">branch to check</param>
        /// <param name="possibleRows">row to look in</param>
        /// <returns>Row that node was found on</returns>
        private int TestRowAny(ILayout layout, DialogTreeBranchIdentifier branch, params int[] possibleRows)
        {
            // try to find the branch
            int? foundRow = null;
            for (int column = layout.ColumnMin; column < layout.ColumnMin + layout.Columns; column++)
            {
                for (int row = layout.RowMin; row < layout.RowMin + layout.Rows; row++)
                {
                    if((layout[column, row] as BranchNode)?.Branch == branch)
                    {
                        foundRow = row;
                        break;
                    }
                }

                if(foundRow != null) break;
            }

            Assert.NotNull(foundRow, $"Failed to find branch '{branch.ToString("h")}'");
            Assert.IsTrue(possibleRows.Contains(foundRow.Value),
                $"Branch ({branch.ToString("h")}) found on row '{foundRow}' not an expected row");
            return foundRow.Value;
        }

        private void TestSpacing(ILayout layout, DialogTree tree)
        {
            List<BranchNode> nodes = new List<BranchNode>();
            for (int y = layout.RowMin; y < layout.RowMin + layout.Rows; y++)
            for (int x = layout.ColumnMin; x < layout.ColumnMin + layout.Columns; x++)
            {
                BranchNode branchNode = layout[x, y] as BranchNode;
                if(branchNode != null) nodes.Add(branchNode);
            }

            List<DialogTreeBranchIdentifier> containedNodes = nodes.Distinct().Select(n => n.Branch).ToList();
            Assert.AreEqual(tree.Branches.Count, containedNodes.Count, "Unexpected amount of nodes");
            foreach (DialogTreeBranchIdentifier id in tree.Branches)
                Assert.IsTrue(containedNodes.Contains(id));
        }

        private NetworkViewModel CreateNetworkForTree(NpcChatProject project, DialogTree tree)
        {
            Dictionary<DialogTreeBranchIdentifier, BranchNode> branchNodes = new Dictionary<DialogTreeBranchIdentifier, BranchNode>();
            NetworkViewModel network = new NetworkViewModel();

            // find all branches and create nodes
            foreach (DialogTreeBranchIdentifier branch in tree.Branches)
            {
                DialogTreeBranch treeBranch = project[branch];
                if(treeBranch == null) continue;

                BranchNode branchNode = new BranchNode(project, branch);
                branchNodes.Add(branch, branchNode);
                network.Nodes.Add(branchNode);
            }

            // link the branches
            foreach (DialogTreeBranchIdentifier branch in tree.Branches)
            {
                BranchNode node = branchNodes[branch];

                foreach (DialogTreeBranchIdentifier child in project[branch].Children)
                {
                    BranchNode childNode = branchNodes[child];
                    network.Connections.Add(new ConnectionViewModel(network, childNode.ParentPin, node.ChildPin));
                }
            }

            return network;
        }
    }
}