using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using NodeNetwork.ViewModels;
using NpcChat.ViewModels.Panels.ScriptDiagram.Layout;
using NpcChat.ViewModels.Panels.ScriptDiagram.Node;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using NUnit.Framework;

namespace NpcChatTestUi.ViewModels.ScriptDiagram.Layout
{
    public class LeveledLayoutTests
    {
        [Test]
        public void NoNetwork()
        {
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(null);

            Assert.IsEmpty(layout.NodeLevels);
        }

        [Test]
        public void NoNodes()
        {
            NetworkViewModel network = new NetworkViewModel();
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);

            Assert.IsEmpty(layout.NodeLevels);
        }

        /// <summary>
        ///  s -> x
        /// </summary>
        [Test]
        public void BasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(2, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch going to two
        ///    1
        ///   /
        ///  s
        ///   \
        ///    2
        /// </summary>
        [Test]
        public void BasicLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "Branch 1");
            DialogTreeBranch branch2 = tree.CreateNewBranch(start, "Branch 2");

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(2, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1, branch2);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// 1 -> 2 -> 3 -> 4
        ///       \  /
        ///        s -> 5
        /// </summary>
        [Test]
        public void ComplexLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch("branch 1");
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1, "branch 2");
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch2, "branch 3");
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3, "branch 4");
            DialogTreeBranch branch5 = tree.CreateNewBranch(start, "branch 5");
            branch2.AddChild(start);
            branch3.AddParent(start);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(5, layout.NodeLevels.Count);
            TestLevel(layout, -2, branch1);
            TestLevel(layout, -1, branch2);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch3, branch5);
            TestLevel(layout, 2, branch4);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// convergence on the start node
        /// 
        ///  1 -> 3
        ///        \
        ///         s -> 5 -> 6
        ///        /
        ///  2 -> 4
        /// </summary>
        [Test]
        public void ConvergencePreStartLayout()
        {
            Console.WriteLine("Expected:");
            Console.WriteLine("1 -> 3");
            Console.WriteLine("      \\");
            Console.WriteLine("       s -> 5 -> 6");
            Console.WriteLine("      /");
            Console.WriteLine("2 -> 4\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch branch1 = tree.CreateNewBranch("branch 1");
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1, "branch 3");
            DialogTreeBranch branch2 = tree.CreateNewBranch("branch 2");
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch2, "branch 4");
            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch5 = tree.CreateNewBranch(start, "branch 5");
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch5, "branch 6");
            start.AddParent(branch3);
            start.AddParent(branch4);
            start.Name = "Start";

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(5, layout.NodeLevels.Count);
            TestLevel(layout, -2, branch1, branch2);
            TestLevel(layout, -1, branch3, branch4);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch5);
            TestLevel(layout, 2, branch6);

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
            start.Name = "start";

            List<DialogTreeBranchIdentifier> branches = new List<DialogTreeBranchIdentifier>(length) { start };
            for (; branches.Count < length;)
            {
                DialogTreeBranch branch = tree.CreateNewBranch(branches.Last());
                branch.Name = $"chain {branches.Count}";
                branches.Add(branch);
            }

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(length, layout.NodeLevels.Count);
            for (int i = 0; i < length; i++)
            {
                Assert.IsTrue(layout.NodeLevels.ContainsKey(i), $"Missing level for {i}");
                Assert.AreEqual(1, layout.NodeLevels[i].Count);
                Assert.AreSame(branches[i], ((BranchNode)layout.NodeLevels[i][0]).Branch);
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

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(3, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1);
            TestLevel(layout, 2, branch2);

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

            start.Name = "Start";
            branch1.Name = "Branch 1";
            branch2.Name = "Branch 2";

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(3, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch2);
            TestLevel(layout, 2, branch1);

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
            Console.WriteLine("Expected:");
            Console.WriteLine(" 1 -> s");
            Console.WriteLine("  \\  / ");
            Console.WriteLine("   2\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "S";
            DialogTreeBranch branch1 = tree.CreateNewBranch("1");
            DialogTreeBranch branch2 = tree.CreateNewBranch("2");
            branch1.AddChild(branch2);
            branch1.AddChild(start);
            branch2.AddChild(start);

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(3, layout.NodeLevels.Count);
            TestLevel(layout, -2, branch1);
            TestLevel(layout, -1, branch2);
            TestLevel(layout, 0, start);

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
            Console.WriteLine("Expected:\n" +
                              "   1 -> 3 -> 4 -> 5\n" +
                              "  /            \\ \n" +
                              "s -> 2 -------> 6 -> 7\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "S";
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "1");
            DialogTreeBranch branch2 = tree.CreateNewBranch(start, "2");
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1, "3");
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3, "4");
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch4, "5");
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch4, "6");
            branch6.AddParent(branch2);
            branch6.AddParent(branch4);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6, "7");

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(6, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1, branch2);
            TestLevel(layout, 2, branch3);
            TestLevel(layout, 3, branch4);
            TestLevel(layout, 4, branch5, branch6);
            TestLevel(layout, 5, branch7);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// tree with one branch going to two but where one branch references the other
        ///
        /// s -> 1 -> 3 -> 4 -> 5
        ///  \              \
        ///   2 ------------ 6 -> 7
        /// </summary>
        [Test]
        public void SkipComplexLayout2()
        {
            Console.WriteLine("Expected:");
            Console.WriteLine("s -> 1 -> 3 -> 4 -> 5");
            Console.WriteLine(" \\              \\");
            Console.WriteLine("  2 ------------ 6 -> 7\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "S";
            DialogTreeBranch branch2 = tree.CreateNewBranch(start, "2");
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "1");
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1, "3");
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3, "4");
            DialogTreeBranch branch5 = tree.CreateNewBranch(branch4, "5");
            DialogTreeBranch branch6 = tree.CreateNewBranch(branch4, "6");
            branch6.AddParent(branch2);
            branch6.AddParent(branch4);
            DialogTreeBranch branch7 = tree.CreateNewBranch(branch6, "7");

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(6, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1, branch2);
            TestLevel(layout, 2, branch3);
            TestLevel(layout, 3, branch4);
            TestLevel(layout, 4, branch5, branch6);
            TestLevel(layout, 5, branch7);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// link that goes behind the start node
        ///  x -> s
        /// </summary>
        [Test]
        public void BackPropagationBasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            branch1.AddChild(start);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(2, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, -1, branch1);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// link that goes behind the start node
        ///  4 -> 3
        ///        \
        ///    s -> 1 -> 2
        /// </summary>
        [Test]
        public void BackPropagationBasicLayout2()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch();
            branch1.AddChild(start);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(2, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, -1, branch1);

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
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "1");
            DialogTreeBranch branch2 = tree.CreateNewBranch(branch1, "2");
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch2, "3");
            DialogTreeBranch back1 = tree.CreateNewBranch("b1");
            DialogTreeBranch back2 = tree.CreateNewBranch(back1, "b2");
            back2.AddChild(branch3);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(4, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1, back1);
            TestLevel(layout, 2, branch2, back2);
            TestLevel(layout, 3, branch3);

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
            start.Name = "Start";
            branches.Add(start);
            starts.Add(start);

            for (; branches.Count < length;)
            {
                DialogTreeBranch newBranch = tree.CreateNewBranch();
                newBranch.Name = "Branch: " + branches.Count;

                if (branches.Count % 2 == 1)
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

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(2, layout.NodeLevels.Count);
            TestLevel(layout, 0, starts.ToArray());
            TestLevel(layout, 1, ends.ToArray());

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

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(3, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1, branch2);
            TestLevel(layout, 2, branch3);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to two (1, 2) then converging back to one branch (3)
        /// 
        ///   1 --> 3
        ///  /       \
        /// s -> 2 -> 4
        /// </summary>
        [Test]
        public void DelayedConvergenceLayout()
        {
            Console.WriteLine("  1 --> 3\n" +
                              " /       \\\n" +
                              "s -> 2 -> 4\n\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "s";
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "1");
            DialogTreeBranch branch2 = tree.CreateNewBranch(start, "2");
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1, "3");
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3, "4");
            branch4.AddParent(branch2);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(4, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch1, branch2);
            TestLevel(layout, 2, branch3);
            TestLevel(layout, 3, branch4);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// start with a few nodes which then link backwards in a new set of branches
        ///     4   s -> 1
        ///   /  \      /
        ///  5 -> 3 -> 2  
        /// </summary>
        [Test]
        public void BackPropagationSkipLayout()
        {
            Console.WriteLine("Expected:");
            Console.WriteLine("    4   s -> 1");
            Console.WriteLine("  /  \\      /");
            Console.WriteLine(" 5 -> 3 -> 2\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "S";
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "1");
            DialogTreeBranch branch2 = tree.CreateNewBranch("2");
            branch2.AddChild(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch("3");
            branch3.AddChild(branch2);
            DialogTreeBranch branch4 = tree.CreateNewBranch("4");
            branch4.AddChild(branch3);
            DialogTreeBranch branch5 = tree.CreateNewBranch("5");
            branch5.AddChild(branch4);
            branch5.AddChild(branch3);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(4, layout.NodeLevels.Count);
            TestLevel(layout, 1, branch1);
            TestLevel(layout, 0, start, branch2);
            TestLevel(layout, -1, branch3);
            TestLevel(layout, -2, branch4);
            TestLevel(layout, -3, branch5);

            TestSpacing(layout, tree);
        }

        /// <summary>
        /// start with a few nodes which then link backwards in a new set of branches
        ///         s -> 1
        ///             /
        ///  5 -> 3 -> 2
        ///   \  /
        ///    4
        /// </summary>
        [Test]
        public void BackPropagationSkipLayout2()
        {
            Console.WriteLine("Expected:");
            Console.WriteLine("        s -> 1");
            Console.WriteLine("            /");
            Console.WriteLine(" 5 -> 3 -> 2");
            Console.WriteLine("  \\  /");
            Console.WriteLine("   4\n");

            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            start.Name = "S";
            DialogTreeBranch branch1 = tree.CreateNewBranch(start, "1");
            DialogTreeBranch branch2 = tree.CreateNewBranch("2");
            branch2.AddChild(branch1);
            DialogTreeBranch branch3 = tree.CreateNewBranch("3");
            branch3.AddChild(branch2);
            DialogTreeBranch branch5 = tree.CreateNewBranch("4");
            branch5.AddChild(branch3);
            DialogTreeBranch branch4 = tree.CreateNewBranch("5");
            branch4.AddChild(branch3);
            branch5.AddChild(branch4);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(4, layout.NodeLevels.Count);
            TestLevel(layout, 1, branch1);
            TestLevel(layout, 0, start, branch2);
            TestLevel(layout, -1, branch3);
            TestLevel(layout, -2, branch4);
            TestLevel(layout, -3, branch5);
            TestSpacing(layout, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to two (1, 2) then converging back to one branch (3)
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

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            layout.Layout(network);
            PrintLayoutResult(layout);

            Assert.AreEqual(6, layout.NodeLevels.Count);
            TestLevel(layout, 0, start);
            TestLevel(layout, 1, branch5);
            TestLevel(layout, 2, branch1);
            TestLevel(layout, 3, branch2);
            TestLevel(layout, 4, branch3);
            TestLevel(layout, 5, branch4);
            TestSpacing(layout, tree);
        }


        private void TestLevel(LeveledLayout layout, int level, params DialogTreeBranchIdentifier[] expectedBranches)
        {
            Assert.IsTrue(layout.NodeLevels.ContainsKey(level), $"Missing level for {level}");
            Assert.AreEqual(expectedBranches.Length, layout.NodeLevels[level].Count,
                $"Expected {expectedBranches.Length} branches at level {level} but found {layout.NodeLevels[level].Count} branches");

            HashSet<DialogTreeBranchIdentifier> remainingBranches = new HashSet<DialogTreeBranchIdentifier>(expectedBranches);
            for (int i = 0; i < layout.NodeLevels[level].Count; i++)
            {
                DialogTreeBranchIdentifier foundBranch = ((BranchNode)layout.NodeLevels[level][i]).Branch;
                Assert.IsTrue(expectedBranches.Contains(foundBranch), $"Level {level} contains unexpected branch");
                remainingBranches.Remove(foundBranch);
            }

            Assert.IsEmpty(remainingBranches, $"Branch level {level} didn't contain all expected branches");
        }

        private void TestSpacing(LeveledLayout layout, DialogTree tree)
        {
            List<BranchNode> nodes = new List<BranchNode>();
            for (int y = 0; y < layout.NodeLayout.GetLength(1); y++)
                for (int x = 0; x < layout.NodeLayout.GetLength(0); x++)
                {
                    BranchNode branchNode = layout.NodeLayout[x, y] as BranchNode;
                    if (branchNode != null) nodes.Add(branchNode);
                }

            List<DialogTreeBranchIdentifier> containedNodes = nodes.Distinct().Select(n => n.Branch).ToList();
            Assert.AreEqual(tree.Branches.Count, containedNodes.Count, "Unexpected amount of nodes");
            foreach (DialogTreeBranchIdentifier id in tree.Branches)
                Assert.IsTrue(containedNodes.Contains(id));
        }

        private void PrintLayoutResult(LeveledLayout layout)
        {
            try
            {
                int[] headerWidths = new int[layout.NodeLevels.Count];
                int[] levelKeys = layout.NodeLevels.Keys.ToArray();
                for (int index = 0; index < layout.NodeLevels.Count; index++)
                    headerWidths[index] = layout.NodeLevels[levelKeys[index]].Select(n => n.Name.Length).Max();

                // Header
                Console.WriteLine("Result:");
                List<int> lineWidth = new List<int>();
                StringBuilder header = new StringBuilder(layout.NodeLevels.Count);
                for (int i = 0; i < layout.NodeLevels.Count; i++)
                {
                    header.Append(' ', (int)Math.Ceiling(headerWidths[i] / 2f))
                        .Append(i)
                        .Append(' ', (int)Math.Ceiling(headerWidths[i] / 2f));
                    lineWidth.Add(header.Length);
                    if (i < layout.NodeLevels.Count - 1) header.Append("|");
                }

                Console.WriteLine(header);

                // Separator
                StringBuilder separator = new StringBuilder(header.Length);
                for (int i = 0; i < lineWidth.Count; i++)
                {
                    separator.Append('-', lineWidth[i] - separator.Length);
                    if (i < layout.NodeLevels.Count - 1) separator.Append("+");
                }

                Console.WriteLine(separator);

                // Main body
                StringBuilder[] lines = new StringBuilder[layout.NodeLayout.GetLength(1)];
                for (int i = 0; i < lines.Length; i++) lines[i] = new StringBuilder(header.Length);

                for (int x = 0; x < layout.NodeLayout.GetLength(0); x++)
                {
                    int expectedWidth = lineWidth[x];

                    for (int y = 0; y < layout.NodeLayout.GetLength(1); y++)
                    {
                        NodeViewModel node = layout.NodeLayout[x, y];
                        if (node != null) lines[y].Append(" ").Append(node.Name);

                        lines[y].Append(' ', expectedWidth - lines[y].Length);
                        if (x < layout.NodeLevels.Count - 1) lines[y].Append("|");
                    }
                }

                foreach (StringBuilder builder in lines)
                    Console.WriteLine(builder);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to print layout results");
            }
        }

        private NetworkViewModel CreateNetworkForTree(NpcChatProject project, DialogTree tree)
        {
            Dictionary<DialogTreeBranchIdentifier, BranchNode> branchNodes = new Dictionary<DialogTreeBranchIdentifier, BranchNode>();
            NetworkViewModel network = new NetworkViewModel();

            // find all branches and create nodes
            foreach (DialogTreeBranchIdentifier branch in tree.Branches)
            {
                DialogTreeBranch treeBranch = project[branch];
                if (treeBranch == null) continue;

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
