using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void Layout(NetworkViewModel network)
        {
            SplitIntoLevels(network);
        }

        private void SplitIntoLevels([NotNull] NetworkViewModel network)
        {
            NodeLevels.Clear();
            m_nodeLevelLookup.Clear();
            m_inputLookup.Clear();
            m_outputLookup.Clear();

            NodeViewModel first = network?.Nodes?.Items.FirstOrDefault();
            if (first == null) return;
            CalculateNodeLevels(network, first);


        }

        /// <summary>
        /// Breaks down the current network into levels with the <see cref="search"/> node being at level 0
        /// </summary>
        /// <param name="network"></param>
        /// <param name="search"></param>
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
    }
}
