using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using NodeNetwork.ViewModels;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Layout
{
    public class LeveledLayout
    {
        public IReadOnlyList<Node> Layout(NetworkViewModel networkViewModel)
        {
            if (networkViewModel is null) return Array.Empty<Node>();

            var nodes = GetNodes(networkViewModel);

            // Find all root nodes i.e. nodes with no inputs
            var rootNodes = nodes
                .Where(node => !node.Inputs.Any())
                .ToArray();

            // On each root node, do a depth first search and set the x position of each nth order output node to n.
            foreach (var rootNode in rootNodes)
            {
                SetInitialXPosition(rootNode, 0);
            }

            // For each node move all of its outputs that are behind it in front
            // Keep looping over all nodes until no changes are made
            var finished = false;
            while (!finished)
            {
                finished = true;
                foreach (var node in nodes)
                {
                    foreach (var output in node.Outputs)
                    {
                        if (output.X <= node.X)
                        {
                            // Move output node forward
                            output.X = node.X + 1;
                            finished = false;
                        }
                    }
                }
            }

            // Bunch all the nodes as close together as possible
            finished = false;
            while (!finished)
            {
                finished = true;
                foreach (var node in nodes)
                {
                    if (node.Outputs.Any())
                    {
                        var closestOutputX = node.Outputs.Min(c => c.X);
                        if (node.X != closestOutputX - 1)
                        {
                            node.X = closestOutputX - 1;
                            finished = false;
                        }
                    }
                }
            }

            // I don't think this can fail, otherwise the graph would be cyclic.
            Debug.Assert(!nodes.Any() || nodes.Min(n => n.X) == 0);

            // Assign the Y position using a DFS. This should be pretty optimal, but could be optimized depending on
            // how pretty the graph should look.
            foreach (var rootNode in rootNodes)
            {
                SetYPosition(nodes, rootNode);
            }

            // Apply the new positions
            foreach (var node in nodes)
            {
                node.ApplyPosition();
            }

            // Return the graph (to make unit testing easier)
            return nodes;
        }

        private static void SetInitialXPosition(Node node, int depth)
        {
            node.X = depth;
            foreach (var output in node.Outputs)
            {
                SetInitialXPosition(output, depth + 1);
            }
        }

        private static void SetYPosition(IReadOnlyList<Node> allNodes, Node node)
        {
            // Only set the Y position if it hasn't already been set
            if (node.Y < 0)
            {
                // Set the Y position to the next free position in the column
                node.Y = allNodes.Where(n => n.X == node.X).Max(n => n.Y) + 1;
            }

            foreach (var output in node.Outputs)
            {
                SetYPosition(allNodes, output);
            }
        }

        private static IReadOnlyList<Node> GetNodes(NetworkViewModel network)
        {
            // Generate a graph data structure from the network view model

            var nodes = network.Nodes.Items
                .Select(nodeViewModel => new Node(nodeViewModel))
                .ToList();

            // Populate the inputs/outputs
            var inputLookup = network.Nodes.Items
                .SelectMany(node => node.Inputs.Items
                    .Select(input => new KeyValuePair<NodeInputViewModel, NodeViewModel>(input, node)))
                .ToDictionary(p => p.Key, p => p.Value);

            var outputLookup = network.Nodes.Items
                .SelectMany(node => node.Outputs.Items
                    .Select(output => new KeyValuePair<NodeOutputViewModel, NodeViewModel>(output, node)))
                .ToDictionary(p => p.Key, p => p.Value);

            foreach (var node in nodes)
            {
                node.Outputs = node.ViewModel.Outputs.Items
                    .SelectMany(port => port.Connections.Items
                            .Select(connection => nodes
                                    .Single(n => n.ViewModel == inputLookup[connection.Input])))
                    .ToArray();

                node.Inputs = node.ViewModel.Inputs.Items
                    .SelectMany(port => port.Connections.Items
                            .Select(connection => nodes
                                    .Single(n => n.ViewModel == outputLookup[connection.Output])))
                    .ToArray();
            }

            return nodes;
        }

        public class Node
        {
            public NodeViewModel ViewModel { get; }
            public IReadOnlyList<Node> Inputs { get; set; }
            public IReadOnlyList<Node> Outputs { get; set; }

            public int X { get; set; }
            public int Y { get; set; }

            public Node(NodeViewModel viewModel)
            {
                this.ViewModel = viewModel;
                // Set the X and Y to -1 initially so we can keep track of whether they were set or not
                this.X = -1;
                this.Y = -1;
            }

            public void ApplyPosition()
            {
                // Width and height of each 'cell' in the grid
                const int colWidth = 250;
                const int rowHeight = 150;
                this.ViewModel.Position = new Point(this.X * colWidth, this.Y * rowHeight);
            }
        }
    }
}
