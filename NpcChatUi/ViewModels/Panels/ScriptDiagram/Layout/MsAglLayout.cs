using System;
using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Layered;
using NodeNetwork.ViewModels;
using Point = System.Windows.Point;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Layout
{
    /// <summary>
    /// Layout designed to produce a compact cluster of nodes going from left to right (with the last items being on the right)
    /// </summary>
    public class MsAglLayout : ILayout
    {
        public void Layout(NetworkViewModel network)
        {
            if(network == null || network.Nodes.Count <= 0) return;
            
            GeometryGraph graph = new GeometryGraph();

            Dictionary<NodeViewModel, Microsoft.Msagl.Core.Layout.Node> layoutLookup =
                new Dictionary<NodeViewModel, Microsoft.Msagl.Core.Layout.Node>();

            const double nodeWidth = 230;
            const double nodeHeight = 100;
            const double spacingHorizontal = 90;
            const double spacingVertical = 60;
            
            // build collection of all nodes
            foreach (NodeViewModel node in network.Nodes.Items)
            {
                ICurve box = CurveFactory.CreateRectangle(nodeWidth, nodeHeight, new Microsoft.Msagl.Core.Geometry.Point());
                Microsoft.Msagl.Core.Layout.Node layoutNode = new Microsoft.Msagl.Core.Layout.Node(box)
                {
                    UserData = node
                };

                layoutLookup.Add(node, layoutNode);
                graph.Nodes.Add(layoutNode);
            }

            foreach(ConnectionViewModel items in network.Connections.Items)
            {
                Microsoft.Msagl.Core.Layout.Node inNode = layoutLookup[items.Input.Parent];
                Microsoft.Msagl.Core.Layout.Node outNode = layoutLookup[items.Output.Parent];
                Edge edge = new Edge(inNode, outNode);
                graph.Edges.Add(edge);
            }

            // perform layout operation
            SugiyamaLayoutSettings settings = new SugiyamaLayoutSettings
            {
                Transformation = PlaneTransformation.Rotation(-90 * (Math.PI / 180)), // left to right
                NodeSeparation = spacingHorizontal,
                LayerSeparation = spacingVertical,
            };
            LayeredLayout layeredLayout = new LayeredLayout(graph, settings);
            layeredLayout.Run();

            // apply the node positions to the real graph
            foreach (KeyValuePair<NodeViewModel, Microsoft.Msagl.Core.Layout.Node> node in layoutLookup)
            {
                node.Key.Position = new Point(node.Value.BoundingBox.Center.X, node.Value.BoundingBox.Center.Y);
            }
        }
    }
}
