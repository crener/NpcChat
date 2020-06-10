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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(null);

            Assert.IsEmpty(nodes);
        }

        [Test]
        public void NoNodes()
        {
            NetworkViewModel network = new NetworkViewModel();
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            Assert.IsEmpty(nodes);
        }

        /// <summary>
        /// basic tree with one branch going to two
        ///  s -> x
        ///    -> x
        /// </summary>
        [Test]
        public void BasicLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            Assert.AreEqual(2, nodes.Select(n => n.X).Distinct().Count());
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch1, branch2);

            TestSpacing(nodes, tree);
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
            List<DialogTreeBranchIdentifier> branches = new List<DialogTreeBranchIdentifier>(length) { start };
            for (; branches.Count < length;)
                branches.Add(tree.CreateNewBranch(branches.Last()));

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(length, numLevels);
            for (int i = 0; i < length; i++)
            {
                var nodesInLevel = nodes.Where(n => n.X == i).ToArray();
                Assert.IsNotEmpty(nodesInLevel, $"Missing level for {i}");
                Assert.AreEqual(1, nodesInLevel.Length);
                Assert.AreSame(branches[i], ((BranchNode)nodesInLevel[0].ViewModel).Branch);
            }

            TestSpacing(nodes, tree);
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(3, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch1);
            TestLevel(nodes, 2, branch2);

            TestSpacing(nodes, tree);
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(3, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch2);
            TestLevel(nodes, 2, branch1);

            TestSpacing(nodes, tree);
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

            start.Name = "Start";
            branch1.Name = "Branch 1";
            branch2.Name = "Branch 2";

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(3, numLevels);
            TestLevel(nodes, 2, start);
            TestLevel(nodes, 1, branch2);
            TestLevel(nodes, 0, branch1);

            TestSpacing(nodes, tree);
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

            start.Name = "Start";
            branch1.Name = "Branch 1";
            branch2.Name = "Branch 2";

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(3, numLevels);
            TestLevel(nodes, 2, start);
            TestLevel(nodes, 1, branch1);
            TestLevel(nodes, 0, branch2);

            TestSpacing(nodes, tree);
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

            start.Name = "Start";
            branch1.Name = "Branch 1";
            branch2.Name = "Branch 2";
            branch3.Name = "Branch 3";
            branch4.Name = "Branch 4";

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(5, numLevels);
            TestLevel(nodes, 4, branch3);
            TestLevel(nodes, 3, branch4);
            TestLevel(nodes, 2, start);
            TestLevel(nodes, 1, branch2);
            TestLevel(nodes, 0, branch1);

            TestSpacing(nodes, tree);
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

            // build network and layout
            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(6, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch1, branch2);
            TestLevel(nodes, 2, branch3);
            TestLevel(nodes, 3, branch4);
            TestLevel(nodes, 4, branch5, branch6);
            TestLevel(nodes, 5, branch7);

            TestSpacing(nodes, tree);
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(2, numLevels);
            TestLevel(nodes, 1, start);
            TestLevel(nodes, 0, branch1);

            TestSpacing(nodes, tree);
        }

        /// <summary>
        /// link that goes behind the start node
        /// 4 -> 3 \
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(2, numLevels);
            TestLevel(nodes, 1, start);
            TestLevel(nodes, 0, branch1);

            TestSpacing(nodes, tree);
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

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(4, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch1, back1);
            TestLevel(nodes, 2, branch2, back2);
            TestLevel(nodes, 3, branch3);

            TestSpacing(nodes, tree);
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(2, numLevels);
            TestLevel(nodes, 0, starts.ToArray());
            TestLevel(nodes, 1, ends.ToArray());

            TestSpacing(nodes, tree);
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(3, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch1, branch2);
            TestLevel(nodes, 2, branch3);

            TestSpacing(nodes, tree);
        }

        /// <summary>
        /// basic tree with one branch (s) going to two (1, 2) then converging back to one branch (3)
        /// 
        ///   1 --> 3
        ///  /       \
        /// s -> 2 -> 4
        /// </summary>
        [Test]
        public void ConvergenceDelayedLayout()
        {
            NpcChatProject project = new NpcChatProject();
            DialogTree tree = project.ProjectDialogs.CreateNewDialogTree();

            DialogTreeBranch start = tree.GetStart();
            DialogTreeBranch branch1 = tree.CreateNewBranch(start);
            DialogTreeBranch branch2 = tree.CreateNewBranch(start);
            DialogTreeBranch branch3 = tree.CreateNewBranch(branch1);
            DialogTreeBranch branch4 = tree.CreateNewBranch(branch3);
            branch4.AddParent(branch2);

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(4, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch1);
            TestLevel(nodes, 2, branch2, branch3);
            TestLevel(nodes, 3, branch4);

            TestSpacing(nodes, tree);
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

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(5, numLevels);
            TestLevel(nodes, 4, branch1);
            TestLevel(nodes, 3, start, branch2);
            TestLevel(nodes, 2, branch3);
            TestLevel(nodes, 1, branch4);
            TestLevel(nodes, 0, branch5);

            TestSpacing(nodes, tree);
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

            NetworkViewModel network = CreateNetworkForTree(project, tree);
            LeveledLayout layout = new LeveledLayout();
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(5, numLevels);
            TestLevel(nodes, 4, branch1);
            TestLevel(nodes, 3, start, branch2);
            TestLevel(nodes, 2, branch3);
            TestLevel(nodes, 1, branch4);
            TestLevel(nodes, 0, branch5);
            TestSpacing(nodes, tree);
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
            IReadOnlyList<LeveledLayout.Node> nodes = layout.Layout(network);

            var numLevels = nodes.Select(n => n.X).Distinct().Count();
            Assert.AreEqual(6, numLevels);
            TestLevel(nodes, 0, start);
            TestLevel(nodes, 1, branch5);
            TestLevel(nodes, 2, branch1);
            TestLevel(nodes, 3, branch2);
            TestLevel(nodes, 4, branch3);
            TestLevel(nodes, 5, branch4);
            TestSpacing(nodes, tree);
        }


        private void TestLevel(IReadOnlyList<LeveledLayout.Node> nodes, int level, params DialogTreeBranchIdentifier[] expectedBranches)
        {
            var nodesInLevel = nodes.Where(n => n.X == level).ToArray();
            Assert.IsNotEmpty(nodesInLevel, $"Missing level for {level}");
            Assert.AreEqual(expectedBranches.Length, nodesInLevel.Length,
                $"Expected {expectedBranches.Length} branches at level {level} but found {nodesInLevel.Length} branches");

            HashSet<DialogTreeBranchIdentifier> remainingBranches = new HashSet<DialogTreeBranchIdentifier>(expectedBranches);
            for (int i = 0; i < nodesInLevel.Length; i++)
            {
                DialogTreeBranchIdentifier foundBranch = ((BranchNode)nodesInLevel[i].ViewModel).Branch;
                Assert.IsTrue(expectedBranches.Contains(foundBranch), $"Level {level} contains unexpected branch");
                remainingBranches.Remove(foundBranch);
            }

            Assert.IsEmpty(remainingBranches, $"Branch level {level} didn't contain all expected branches");
        }

        private void TestSpacing(IReadOnlyList<LeveledLayout.Node> nodes, DialogTree tree)
        {
            List<BranchNode> branchNodes = new List<BranchNode>();
            foreach (var node in nodes)
            {
                branchNodes.Add(node.ViewModel as BranchNode);
            }

            List<DialogTreeBranchIdentifier> containedNodes = branchNodes.Distinct().Select(n => n.Branch).ToList();
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
