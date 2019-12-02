using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using DynamicData.Annotations;
using NodeNetwork.ViewModels;
using NpcChatSystem.Utilities;
using ReactiveUI;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Layout
{
    /// <summary>
    /// Layout designed to be used to organize nodes into levels depending on dependencies and links to other nodes,
    /// so that nodes with no dependencies are on the left and the end of a dependency chain on the right
    /// </summary>
    public class LeveledLayout
    {
        /// <summary>
        /// The depth relative to the center node that nodes on the network are
        /// </summary>
        public readonly Dictionary<int, List<NodeViewModel>> NodeLevels = new Dictionary<int, List<NodeViewModel>>();

        /// <summary>
        /// Proposed layout of the node network on a 2D plane from the last <see cref="Layout"/> call
        /// </summary>
        public NodeViewModel[,] NodeLayout { get; protected set; }

        private Dictionary<NodeViewModel, int> m_nodeLevelLookup = new Dictionary<NodeViewModel, int>();
        private Dictionary<NodeInputViewModel, NodeViewModel> m_inputLookup = new Dictionary<NodeInputViewModel, NodeViewModel>();
        private Dictionary<NodeOutputViewModel, NodeViewModel> m_outputLookup = new Dictionary<NodeOutputViewModel, NodeViewModel>();

        private const double c_nodeSpacerX = 8d;
        private const double c_nodeSpacerY = 4d;
        private Dictionary<int, double> m_levelWidth = new Dictionary<int, double>();


        public void Layout(NetworkViewModel network)
        {
            NodeLayout = null;
            NodeLevels.Clear();
            m_nodeLevelLookup.Clear();
            m_inputLookup.Clear();
            m_outputLookup.Clear();
            m_levelWidth.Clear();

            NodeViewModel first = network?.Nodes?.Items.FirstOrDefault();
            if (first == null) return;

            BuildNodeLevels(network, first);
            NodeLayout = LayoutNodesInNetwork(network, first);
            RepositionNodes(NodeLayout, first);
        }

        /// <summary>
        /// Move nodes around the network according to the given layout
        /// </summary>
        /// <param name="layout">positions of </param>
        /// <param name="center"></param>
        protected void RepositionNodes(NodeViewModel[,] layout, NodeViewModel center)
        {
            double xPos = 0f;

            //find the position in the layout
            int minLevel = NodeLevels.Select(n => n.Key).Min();
            int centerLevel = m_nodeLevelLookup[center];
            for (int i = minLevel; i < centerLevel; i++)
            {
                xPos -= m_levelWidth[i];
                xPos -= c_nodeSpacerX;
            }
            xPos += c_nodeSpacerX;

            for (int x = 0; x < layout.GetLength(0); x++)
            {
                double yPos = 0f;
                for (int y = 0; y < layout.GetLength(1); y++)
                {
                    NodeViewModel model = layout[x, y];
                    if (model != null) model.Position = new Point(xPos, yPos);
                    yPos += center.Size.Height + c_nodeSpacerY;
                }
                xPos += m_levelWidth[minLevel + x] + c_nodeSpacerX;
            }
        }

        #region layout generation

        protected virtual NodeViewModel[,] LayoutNodesInNetwork(NetworkViewModel network, NodeViewModel centerNode)
        {
            // find all starting points
            List<NodeViewModel> startPoint = new List<NodeViewModel>();
            foreach (NodeViewModel node in network.Nodes.Items)
            {
                if (node.Inputs.Items.All(i => i.Connections.Count > 0)) continue;
                if (startPoint.Contains(node)) continue;
                startPoint.Add(node);
            }

            HashSet<NodeViewModel> seen = new HashSet<NodeViewModel>(m_nodeLevelLookup.Count);
            List<NodeViewModel> criticalPath = FindMainNodeSet(centerNode, seen);

            //int xKeyOffset = Math.Abs(Math.Min(0, NodeLevels.Keys.Min()));
            int height = FindSpaceUsageHeight();
            NodeViewModel[,] spaceUsage = new NodeViewModel[NodeLevels.Count, height];
            int middle = height / 2;

            // align the main points
            int centerOffset = Math.Abs(NodeLevels.Select(l => l.Key).Min());
            List<KeyValuePair<NodeViewModel, NodeViewModel>> search = new List<KeyValuePair<NodeViewModel, NodeViewModel>>();
            int? lastDepth = null;
            for (int x = 0; x < criticalPath.Count; x++)
            {
                NodeViewModel model = criticalPath[x];
                seen.Add(model);
                search.AddRange(OutputNodes(model).Select(m => new KeyValuePair<NodeViewModel, NodeViewModel>(model, m)));
                search.AddRange(InputNodes(model).Select(m => new KeyValuePair<NodeViewModel, NodeViewModel>(model, m)));

                int nodeLevel = m_nodeLevelLookup[model];
                spaceUsage[centerOffset + nodeLevel, middle] = model;

                int currentDepth = nodeLevel;
                if (lastDepth != null)
                {
                    for (int cx = lastDepth.Value + 1; cx < currentDepth; cx++)
                    {
                        // take the slot to allow clear connections
                        // todo change this to use a fake model rather than the actual model to avoid duplication
                        spaceUsage[centerOffset + cx, middle] = model;
                    }
                }

                lastDepth = currentDepth;
            }

            for (int i = search.Count - 1; i >= 0; i--)
                if (seen.Contains(search[i].Value)) search.RemoveAt(i);

            // go forward through the added nodes and add children
            while (search.Count != 0)
            {
                NodeViewModel currentModel = search[0].Value;
                NodeViewModel modelParent = search[0].Key;
                int currentDepth = NodeDepth(currentModel);

                for (int i = 1; i < search.Count; i++)
                {
                    int checkDepth = NodeDepth(search[i].Value);
                    if (currentDepth < checkDepth)
                    {
                        currentDepth = checkDepth;
                        currentModel = search[i].Value;
                        modelParent = search[i].Key;
                    }
                }

                seen.Add(currentModel);

                //find the closest possible empty slot to 'currentModel' in 'spaceUsage'
                XyPosition? pos = FindClosest(spaceUsage, modelParent, currentModel);
                if (pos != null)
                {
                    spaceUsage[pos.Value.X, pos.Value.Y] = currentModel;

                    search.AddRange(OutputNodes(currentModel).Select(m => new KeyValuePair<NodeViewModel, NodeViewModel>(currentModel, m)));
                    search.AddRange(InputNodes(currentModel).Select(m => new KeyValuePair<NodeViewModel, NodeViewModel>(currentModel, m)));
                }
                else
                {
                    Logging.Logger.Warn($"Failed to correctly layout {currentModel}");
                }

                for (int i = search.Count - 1; i >= 0; i--)
                    if (seen.Contains(search[i].Value)) search.RemoveAt(i);

                /*if (search.Count == 0)
                {
                    search.AddRange(startPoint.Select(m => new KeyValuePair<NodeViewModel, NodeViewModel>(null, m)));

                    for (int i = search.Count - 1; i >= 0; i--)
                        if (seen.Contains(search[i].Value)) search.RemoveAt(i);
                }*/
            }

            return spaceUsage;
        }

        private XyPosition? FindClosest(NodeViewModel[,] spaceUsage, NodeViewModel origin, NodeViewModel destination)
        {
            XyPosition pos = SpaceUsagePosition(spaceUsage, origin);
            int originLevel = m_nodeLevelLookup[origin];
            int destinationLevel = m_nodeLevelLookup[destination];

            if (originLevel > destinationLevel)
            {
                return FindClosest(spaceUsage, pos, pos.X - 1);
            }
            else if (originLevel < destinationLevel)
            {
                return FindClosest(spaceUsage, pos, pos.X + 1);
            }

            return null;
        }

        private XyPosition? FindClosest(NodeViewModel[,] spaceUsage, XyPosition pos, int searchAxis)
        {
            int height = spaceUsage.GetLength(1);

            int? closestY = null;
            int distance = int.MaxValue;
            for (int i = 0; i < height; i++)
            {
                NodeViewModel model = spaceUsage[searchAxis, i];
                if (model != null) continue;

                int testDistance = Math.Abs(pos.Y - i);
                if (testDistance < distance)
                {
                    distance = testDistance;
                    closestY = i;
                }
            }

            if (closestY != null)
            {
                return new XyPosition(searchAxis, closestY.Value);
            }

            return null;
        }

        private XyPosition SpaceUsagePosition(NodeViewModel[,] spaceUsage, NodeViewModel currentModel)
        {
            int height = spaceUsage.GetLength(1);
            int centerOffset = Math.Abs(NodeLevels.Select(l => l.Key).Min());
            int x = centerOffset + m_nodeLevelLookup[currentModel];
            int y = 0;

            for (int i = 0; i < height; i++)
                if (spaceUsage[x, i] == currentModel)
                {
                    y = i;
                    break;
                }

            return new XyPosition(x, y);
        }

        protected virtual int FindSpaceUsageHeight()
        {
            HashSet<NodeViewModel> seen = new HashSet<NodeViewModel>(m_nodeLevelLookup.Count);
            Dictionary<int, int> levelHeight = new Dictionary<int, int>();
            foreach (KeyValuePair<int, List<NodeViewModel>> level in NodeLevels.OrderBy(n => n.Key))
            {
                foreach (NodeViewModel node in level.Value)
                {
                    if (seen.Contains(node)) continue;
                    seen.Add(node);

                    levelHeight[level.Key] = (levelHeight.ContainsKey(level.Key) ? levelHeight[level.Key] + 1 : 1);
                    List<NodeViewModel> children = OutputNodes(node).ToList();
                    foreach (NodeViewModel child in children)
                    {
                        int nodeLevel = m_nodeLevelLookup[child];
                        if (nodeLevel > level.Key + 1)
                        {
                            // add height between the nodes for spacing
                            for (int i = level.Key + 1; i <= nodeLevel - 1; i++)
                            {
                                int childLevel = level.Key + i;
                                levelHeight[childLevel] = (levelHeight.ContainsKey(childLevel) ? levelHeight[childLevel] + 1 : 1);
                            }
                        }
                    }
                }
            }

            //int height = NodeLevels.Values.Select(l => l.Count).Max();
            int height = levelHeight.Select(p => p.Value).Max();
            if (height % 2 == 0) height++;
            return height;
        }

        /// <summary>
        /// Builds a list of the critical path according to the metrics returned by the 
        /// </summary>
        /// <param name="start">node to look through</param>
        /// <param name="ignore">set of nodes to ignore, can be null</param>
        /// <returns>list of connected nodes by longest length</returns>
        private List<NodeViewModel> FindMainNodeSet(NodeViewModel start, HashSet<NodeViewModel> ignore)
        {
            List<NodeViewModel> nodes = new List<NodeViewModel>();
            nodes.Add(start);

            NodeViewModel checkNode = start;
            HashSet<NodeViewModel> checkNodes = new HashSet<NodeViewModel>();

            while (checkNode != null)
            {
                int level = m_nodeLevelLookup[checkNode];
                foreach (NodeOutputViewModel output in checkNode.Outputs.Items)
                {
                    foreach (ConnectionViewModel connection in output.Connections.Items)
                    {
                        NodeViewModel connected = m_inputLookup[connection.Input];
                        if (ignore == null || ignore.Contains(connected)) continue;
                        if (!checkNodes.Contains(connected) && m_nodeLevelLookup[connected] >= level)
                            checkNodes.Add(connected);
                    }
                }

                if (checkNodes.Count > 0)
                {
                    NodeViewModel nextNode = NextNodeInMainPath(checkNodes);
                    if (nextNode == null) throw new NullReferenceException($"Node was Null... this should never happen");

                    checkNode = nextNode;
                    nodes.Add(nextNode);
                    checkNodes.Clear();
                }
                else
                {
                    checkNode = null;
                }
            }

            return nodes;
        }

        /// <summary>
        /// Main heuristic for determining which node to add too the main line of nodes
        /// </summary>
        /// <param name="checkNodes">possible nodes</param>
        /// <returns>next node to place from available options</returns>
        protected virtual NodeViewModel NextNodeInMainPath(HashSet<NodeViewModel> checkNodes)
        {
            NodeViewModel bestNode = null;
            int bestDepth = -1;
            int bestLevel = -1;

            foreach (NodeViewModel node in checkNodes)
            {
                int depth = NodeDepth(node);
                int level = GreatestNodeChildLevel(node);

                if (bestLevel < level ||
                    bestLevel == level && bestDepth > depth)
                {
                    bestDepth = depth;
                    bestLevel = level;
                    bestNode = node;
                }
            }

            return bestNode;
        }
        #endregion

        #region Node levels

        private void BuildNodeLevels([NotNull] NetworkViewModel network, NodeViewModel mainNode)
        {
            CalculateNodeLevels(network, mainNode);
            DetermineLevelWidth();
        }

        private void DetermineLevelWidth()
        {
            foreach (KeyValuePair<int, List<NodeViewModel>> pair in NodeLevels)
            {
                double width = pair.Value.Select(n => n.Size.Width).Max();
                m_levelWidth.Add(pair.Key, width);
            }
        }

        /// <summary>
        /// Breaks down the current network into levels with the <see cref="search"/> node being at level 0
        /// </summary>
        /// <param name="network">Network to look through</param>
        /// <param name="search">Node to put into the middle of the network</param>
        private void CalculateNodeLevels(NetworkViewModel network, NodeViewModel search)
        {
            //build lookup for connections
            foreach (NodeViewModel node in network.Nodes.Items)
            {
                foreach (NodeInputViewModel input in node.Inputs.Items)
                    m_inputLookup.Add(input, node);
                foreach (NodeOutputViewModel output in node.Outputs.Items)
                    m_outputLookup.Add(output, node);
            }

            FindSegments(network);
            RecenterStart(search);

            //build the node level
            foreach (KeyValuePair<NodeViewModel, int> level in m_nodeLevelLookup)
            {
                if (!NodeLevels.ContainsKey(level.Value))
                    NodeLevels.Add(level.Value, new List<NodeViewModel>());

                NodeLevels[level.Value].Add(level.Key);
            }
        }

        private void FindSegments(NetworkViewModel network)
        {
            //find the start nodes
            List<NodeViewModel> startNodes = new List<NodeViewModel>();
            foreach (NodeViewModel node in network.Nodes.Items)
                if (!HasParent(node)) startNodes.Add(node);

            if (startNodes.Count == 0) return;

            // find all possible link paths
            List<NodeViewModel[]> paths = new List<NodeViewModel[]>();
            foreach (NodeViewModel start in startNodes)
                paths.AddRange(BuildFullPath(start));

            TraversalReduction(paths);

            // find the levels inside the paths
            foreach (NodeViewModel[] branchTraversal in paths)
            {
                if (m_nodeLevelLookup.Count == 0)
                {
                    // add first path straight away
                    foreach (NodeViewModel node in branchTraversal)
                        m_nodeLevelLookup[node] = m_nodeLevelLookup.Count;
                    continue;
                }

                // determine depth by finding earliest node
                int? depth = null;
                int foundDepth = 0;
                for (int i = 0; i < branchTraversal.Length; i++)
                {
                    NodeViewModel node = branchTraversal[i];
                    if (m_nodeLevelLookup.ContainsKey(node))
                    {
                        int nodeDepth = m_nodeLevelLookup[node];
                        if (depth == null || nodeDepth < depth)
                        {
                            depth = nodeDepth;
                            foundDepth = i;
                        }
                    }
                }
                if (depth == null) depth = 0;
                else depth -= foundDepth;

                bool preExisting = false;
                for (int i = 0; i < branchTraversal.Length; i++)
                {
                    if (m_nodeLevelLookup.ContainsKey(branchTraversal[i]))
                    {
                        depth = m_nodeLevelLookup[branchTraversal[i]];
                        preExisting = true;
                        continue;
                    }

                    if (preExisting)
                    {
                        //it's an insert if the next node merges back into the existing level
                        ICollection<NodeViewModel> children = ExtractChildNodes(branchTraversal[i]);
                        HashSet<NodeViewModel> valid = m_nodeLevelLookup.Where(l => l.Value == depth + 1)
                            .Select(l => l.Key).ToHashSet();
                        bool insert = children.Any(c => valid.Contains(c));

                        if (insert)
                        {
                            foreach (NodeViewModel node in m_nodeLevelLookup
                                .Where(b => b.Value > depth)
                                .Select(b => b.Key).ToArray())
                            {
                                m_nodeLevelLookup[node] = m_nodeLevelLookup[node] + 1;
                            }
                        }

                        m_nodeLevelLookup[branchTraversal[i]] = (depth.Value) + 1;
                    }
                    else m_nodeLevelLookup[branchTraversal[i]] = depth.Value;

                    depth++;
                }
            }
        }

        private void TraversalReduction(List<NodeViewModel[]> paths)
        {

        }

        /// <summary>
        /// Recenter the <see cref="m_nodeLevelLookup"/> nodes so that <paramref name="start"/> is at 0
        /// </summary>
        /// <param name="start">node to recenter</param>
        private void RecenterStart(NodeViewModel start)
        {
            // recenter onto the start node
            int offset = m_nodeLevelLookup[start];
            if (offset == 0) return;

            foreach (NodeViewModel node in m_nodeLevelLookup.Keys.ToArray())
            {
                m_nodeLevelLookup[node] -= offset;
            }
        }

        private List<NodeViewModel[]> BuildFullPath(NodeViewModel start, NodeViewModel[] traversed = null)
        {
            List<NodeViewModel> traversedNodes = new List<NodeViewModel>(traversed?.Length ?? 3);
            if (traversed != null) traversedNodes.AddRange(traversed);
            traversedNodes.Add(start);

            if (!HasChildren(start))
                return new List<NodeViewModel[]>(new[] { traversedNodes.ToArray() });

            List<NodeViewModel[]> buildPaths = new List<NodeViewModel[]>();
            foreach (NodeViewModel child in ExtractChildNodes(start))
            {
                buildPaths.AddRange(BuildFullPath(child, traversedNodes.ToArray()));
            }

            return buildPaths;
        }

        private ICollection<NodeViewModel> ExtractChildNodes(NodeViewModel node)
        {
            HashSet<NodeViewModel> linkedNodes = new HashSet<NodeViewModel>();

            foreach (NodeOutputViewModel output in node.Outputs.Items)
                foreach (ConnectionViewModel connection in output.Connections.Items)
                {
                    NodeViewModel linkNode = m_inputLookup[connection.Input];
                    if (!linkedNodes.Contains(linkNode)) linkedNodes.Add(linkNode);
                }

            return linkedNodes;
        }

        /// <summary>
        /// Checks if <paramref cref="node"/> has any parents
        /// </summary>
        /// <param name="node">node to check</param>
        /// <returns>true if <paramref name="node"/> has any parents</returns>
        private static bool HasParent(NodeViewModel node) =>
            node.Inputs.Items.Any(i => i.Connections.Items.Any());

        /// <summary>
        /// Checks if <paramref cref="node"/> has any children
        /// </summary>
        /// <param name="node">node to check</param>
        /// <returns>true if <paramref name="node"/> has any parents</returns>
        private static bool HasChildren(NodeViewModel node) =>
            node.Outputs.Items.Any(i => i.Connections.Items.Any());

        #endregion

        /// <summary>
        /// finds the largest amount of children that <see cref="node"/> has without circular dependencies
        /// </summary>
        /// <param name="node">Node to search</param>
        /// <returns>returns longest output chain node amount</returns>
        protected int NodeDepth(NodeViewModel node)
        {
            List<KeyValuePair<int, NodeViewModel>> children = new List<KeyValuePair<int, NodeViewModel>>();
            foreach (NodeOutputViewModel pin in node.Outputs.Items)
                foreach (ConnectionViewModel connection in pin.Connections.Items)
                {
                    NodeViewModel lookupNode = m_inputLookup[connection.Input];
                    children.Add(new KeyValuePair<int, NodeViewModel>(NodeDepth(lookupNode), lookupNode));
                }

            if (children.Count > 0)
            {
                List<KeyValuePair<int, NodeViewModel>> sorted = children.OrderBy(c => c.Key).ToList();
                return sorted[0].Key + 1;
            }

            return 0;
        }
        /// <summary>
        /// finds the largest amount of children that <see cref="node"/> has without circular dependencies
        /// </summary>
        /// <param name="node">Node to search</param>
        /// <returns>returns longest output chain node amount</returns>
        protected int GreatestNodeChildLevel(NodeViewModel node)
        {
            List<KeyValuePair<int, NodeViewModel>> children = new List<KeyValuePair<int, NodeViewModel>>();
            foreach (NodeOutputViewModel pin in node.Outputs.Items)
                foreach (ConnectionViewModel connection in pin.Connections.Items)
                {
                    NodeViewModel lookupNode = m_inputLookup[connection.Input];
                    children.Add(new KeyValuePair<int, NodeViewModel>(GreatestNodeChildLevel(lookupNode), lookupNode));
                }

            if (children.Count > 0)
            {
                List<KeyValuePair<int, NodeViewModel>> sorted = children.OrderBy(c => c.Key).ToList();
                return sorted[0].Key;
            }

            return m_nodeLevelLookup[node];
        }

        protected IEnumerable<NodeViewModel> OutputNodes(NodeViewModel node)
        {
            List<NodeViewModel> outNodes = new List<NodeViewModel>(node.Outputs.Count);

            foreach (NodeOutputViewModel pin in node.Outputs.Items)
                foreach (ConnectionViewModel connection in pin.Connections.Items)
                {
                    NodeViewModel lookupNode = m_inputLookup[connection.Input];
                    outNodes.Add(lookupNode);
                }

            return outNodes;
        }

        protected IEnumerable<NodeViewModel> InputNodes(NodeViewModel node)
        {
            List<NodeViewModel> outNodes = new List<NodeViewModel>(node.Outputs.Count);

            foreach (NodeInputViewModel pin in node.Inputs.Items)
                foreach (ConnectionViewModel connection in pin.Connections.Items)
                {
                    NodeViewModel lookupNode = m_outputLookup[connection.Output];
                    outNodes.Add(lookupNode);
                }

            return outNodes;
        }

        [DebuggerDisplay("X; {X}, Y: {Y}")]
        public struct XyPosition
        {
            public int X { get; }
            public int Y { get; }

            public XyPosition(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
