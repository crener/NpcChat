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
using ReactiveUI;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Layout
{
    public class LeveledLayout
    {
        public Dictionary<int, List<NodeViewModel>> NodeLevels = new Dictionary<int, List<NodeViewModel>>();

        private Dictionary<NodeViewModel, int> m_nodeLevelLookup = new Dictionary<NodeViewModel, int>();
        private Dictionary<NodeInputViewModel, NodeViewModel> m_inputLookup = new Dictionary<NodeInputViewModel, NodeViewModel>();
        private Dictionary<NodeOutputViewModel, NodeViewModel> m_outputLookup = new Dictionary<NodeOutputViewModel, NodeViewModel>();

        private const double c_nodeSpacerX = 8d;
        private const double c_nodeSpacerY = 4d;
        private Dictionary<int, double> m_levelWidth = new Dictionary<int, double>();


        public void Layout(NetworkViewModel network)
        {
            NodeLevels.Clear();
            m_nodeLevelLookup.Clear();
            m_inputLookup.Clear();
            m_outputLookup.Clear();
            m_levelWidth.Clear();

            NodeViewModel first = network?.Nodes?.Items.FirstOrDefault();
            if (first == null) return;

            BuildNodeLevels(network, first);
            NodeViewModel[,] layout = LayoutNodesInNetwork(network, first);
            RepositionNodes(layout, first);
        }

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

        protected virtual NodeViewModel[,] LayoutNodesInNetwork(NetworkViewModel network, NodeViewModel centerNode)
        {
            // find all starting points
            List<NodeViewModel> startPoint = new List<NodeViewModel>();
            startPoint.Add(centerNode);
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
            List<NodeViewModel> search = new List<NodeViewModel>();
            int? lastDepth = null;
            for (int x = 0; x < criticalPath.Count; x++)
            {
                NodeViewModel model = criticalPath[x];
                seen.Add(model);
                search.AddRange(OutputNodes(model));

                spaceUsage[m_nodeLevelLookup[model], middle] = model;

                int currentDepth = m_nodeLevelLookup[model];
                if (lastDepth != null)
                {
                    for (int cx = lastDepth.Value + 1; cx < currentDepth; cx++)
                    {
                        spaceUsage[cx, middle] = model;
                    }
                }

                lastDepth = currentDepth;
            }

            for (int i = search.Count - 1; i >= 0; i--)
                if (seen.Contains(search[i])) search.RemoveAt(i);

            // go forward through the added nodes and add children
            while (search.Count != 0)
            {
                NodeViewModel currentModel = search[0];
                int currentDepth = NodeDepth(currentModel);

                for (int i = 1; i < search.Count; i++)
                {
                    int checkDepth = NodeDepth(search[i]);
                    if (currentDepth < checkDepth)
                    {
                        currentDepth = checkDepth;
                        currentModel = search[i];
                    }
                }

                //find the closest possible empty slot to 'currentModel' in 'spaceUsage'
                XyPosition pos = SpaceUsagePosition(spaceUsage, currentModel);
                //XyPosition? target = FindClosest(spaceUsage, pos);
                spaceUsage[pos.X, middle - pos.Y] = currentModel;

                seen.Add(currentModel);
                search.AddRange(OutputNodes(currentModel));

                for (int i = search.Count - 1; i >= 0; i--)
                    if (seen.Contains(search[i])) search.RemoveAt(i);
            }

            return spaceUsage;
        }

        private XyPosition? FindClosest(NodeViewModel[,] spaceUsage, XyPosition pos)
        {
            int x = pos.X + 1;
            int height = spaceUsage.GetLength(1);

            int? closestY = null;
            int distance = int.MaxValue;
            for (int i = 0; i < height; i++)
            {
                NodeViewModel model = spaceUsage[x, i];
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
                return new XyPosition(x, closestY.Value);
            }

            return null;
        }

        private XyPosition SpaceUsagePosition(NodeViewModel[,] spaceUsage, NodeViewModel currentModel)
        {
            int height = spaceUsage.GetLength(1);
            int x = m_nodeLevelLookup[currentModel];
            int y = -1;

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
                    if (nextNode == null) throw new NullReferenceException($"Node returned by {nameof(NextNodeInMainPath)} was Null... this should never happen");

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

            BreadthNodeLookup(search);

            //build the node level
            foreach (KeyValuePair<NodeViewModel, int> level in m_nodeLevelLookup)
            {
                if (!NodeLevels.ContainsKey(level.Value))
                    NodeLevels.Add(level.Value, new List<NodeViewModel>());

                NodeLevels[level.Value].Add(level.Key);
            }
        }

        /// <summary>
        /// Primary look for finding all of the nodes
        /// </summary>
        /// <param name="search">node to start with</param>
        private void BreadthNodeLookup(NodeViewModel search)
        {
            List<Dictionary<int, Queue<NodeViewModel>>> forwardBuffer = new List<Dictionary<int, Queue<NodeViewModel>>>(1);
            List<Dictionary<int, Queue<NodeViewModel>>> backBuffer = new List<Dictionary<int, Queue<NodeViewModel>>>();
            backBuffer.Add(BreadthForwardTraversal(0, search));

            while (backBuffer.Count != 0)
            {
                foreach (Dictionary<int, Queue<NodeViewModel>> dict in backBuffer)
                    foreach (KeyValuePair<int, Queue<NodeViewModel>> pair in dict)
                        foreach (NodeViewModel searchModel in pair.Value)
                            forwardBuffer.Add(BreadthBackwardTraversal(pair.Key - 1, searchModel));
                backBuffer.Clear();

                foreach (Dictionary<int, Queue<NodeViewModel>> dict in forwardBuffer)
                    foreach (KeyValuePair<int, Queue<NodeViewModel>> pair in dict)
                        foreach (NodeViewModel searchModel in pair.Value)
                            backBuffer.Add(BreadthForwardTraversal(pair.Key + 1, searchModel));
                forwardBuffer.Clear();
            }
        }

        private Dictionary<int, Queue<NodeViewModel>> BreadthForwardTraversal(int initialLevel, NodeViewModel search)
        {
            int level = initialLevel;
            Queue<NodeViewModel> forwardQueue = new Queue<NodeViewModel>();
            Dictionary<int, Queue<NodeViewModel>> backBuffer = new Dictionary<int, Queue<NodeViewModel>>();
            forwardQueue.Enqueue(search);

            NodeViewModel next;
            int currentLevelRemaining = 1, nextLevelCount = 0;
            while (forwardQueue.Count > 0)
            {
                next = forwardQueue.Dequeue();
                currentLevelRemaining--;
                m_nodeLevelLookup[next] = level;

                foreach (NodeOutputViewModel output in next.Outputs.Items)
                    foreach (ConnectionViewModel connection in output.Connections.Items)
                    {
                        NodeViewModel node = m_inputLookup[connection.Input];
                        forwardQueue.Enqueue(node);
                        nextLevelCount++;
                    }

                foreach (NodeInputViewModel input in next.Inputs.Items)
                    foreach (ConnectionViewModel connection in input.Connections.Items)
                    {
                        NodeViewModel node = m_outputLookup[connection.Output];
                        //if (m_nodeLevelLookup.ContainsKey(node)) continue;
                        if (!backBuffer.ContainsKey(level))
                            backBuffer.Add(level, new Queue<NodeViewModel>());
                        backBuffer[level].Enqueue(node);
                    }

                if (currentLevelRemaining <= 0)
                {
                    level++;
                    currentLevelRemaining = nextLevelCount;
                    nextLevelCount = 0;
                }
            }

            return backBuffer;
        }

        private Dictionary<int, Queue<NodeViewModel>> BreadthBackwardTraversal(int initialLevel, NodeViewModel search)
        {
            int level = initialLevel;
            Queue<NodeViewModel> backQueue = new Queue<NodeViewModel>();
            Dictionary<int, Queue<NodeViewModel>> forwardBuffer = new Dictionary<int, Queue<NodeViewModel>>();
            backQueue.Enqueue(search);

            NodeViewModel next;
            int currentLevelRemaining = 1, nextLevelCount = 0;
            while (backQueue.Count > 0)
            {
                next = backQueue.Dequeue();
                currentLevelRemaining--;
                if (m_nodeLevelLookup.ContainsKey(next)) continue;
                m_nodeLevelLookup[next] = level;

                foreach (NodeInputViewModel pin in next.Inputs.Items)
                    foreach (ConnectionViewModel connection in pin.Connections.Items)
                    {
                        NodeViewModel node = m_outputLookup[connection.Output];
                        backQueue.Enqueue(node);
                        nextLevelCount++;
                    }

                foreach (NodeOutputViewModel pin in next.Outputs.Items)
                    foreach (ConnectionViewModel connection in pin.Connections.Items)
                    {
                        NodeViewModel node = m_inputLookup[connection.Input];
                        //if (m_nodeLevelLookup.ContainsKey(node)) continue;
                        if (!forwardBuffer.ContainsKey(level))
                            forwardBuffer.Add(level, new Queue<NodeViewModel>());
                        forwardBuffer[level].Enqueue(node);
                    }

                if (currentLevelRemaining <= 0)
                {
                    level--;
                    currentLevelRemaining = nextLevelCount;
                    nextLevelCount = 0;
                }
            }

            return forwardBuffer;
        }

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
            SortedList<int, NodeViewModel> children = new SortedList<int, NodeViewModel>();
            foreach (NodeOutputViewModel pin in node.Outputs.Items)
                foreach (ConnectionViewModel connection in pin.Connections.Items)
                {
                    NodeViewModel lookupNode = m_inputLookup[connection.Input];
                    children.Add(GreatestNodeChildLevel(lookupNode), lookupNode);
                }

            if (children.Count > 0)
            {
                return children.Keys[0];
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
