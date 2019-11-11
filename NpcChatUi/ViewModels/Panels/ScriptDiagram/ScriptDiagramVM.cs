using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DynamicData;
using NodeNetwork;
using NodeNetwork.Toolkit;
using NodeNetwork.Toolkit.NodeList;
using NodeNetwork.ViewModels;
using NpcChat.ViewModels.Panels.ScriptDiagram.Layout;
using NpcChat.ViewModels.Panels.ScriptDiagram.Node;
using NpcChat.ViewModels.Panels.ScriptEditor;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Prism.Commands;
using Xceed.Wpf.AvalonDock.Layout;

namespace NpcChat.ViewModels.Panels.ScriptDiagram
{
    public class ScriptDiagramVM : LayoutDocument
    {
        public DialogTree Tree => m_tree;
        public NetworkViewModel Network { get; }
        public NodeListViewModel NodeList { get; }

        public ICommand ForceLayoutCommand { get; }

        private NpcChatProject m_project { get; }
        private DialogTree m_tree;
        private LeveledLayout m_layouter = new LeveledLayout();
        private readonly Dictionary<DialogTreeBranchIdentifier, BranchNode> m_branchNodes = new Dictionary<DialogTreeBranchIdentifier, BranchNode>();
        private bool m_ignoreBranchEvents = false;

        public ScriptDiagramVM(NpcChatProject project, DialogTreeIdentifier dialog = null)
        {
            Title = "Script Visualizer";
            ToolTip = "Script Visualizer";
            CanClose = true;
            m_project = project;

            ForceLayoutCommand = new DelegateCommand(() => m_layouter.Layout(Network));

            Network = new NetworkViewModel();
            SetDialogTree(dialog);
            Network.Validator = n =>
            {
                // don't allow loops
                if (GraphAlgorithms.FindLoops(n).Any())
                    return new NetworkValidationResult(false, false, new ErrorMessageViewModel("Network contains loops!"));

                return new NetworkValidationResult(true, true, null);
            };

            IObservable<IChangeSet<ConnectionViewModel>> connections = Network.Connections.Connect();
            connections.Subscribe(ConnectionChange);

            // stops new branch being created for the side bar visuals
            bool nodeListInitialized = false;

            NodeList = new NodeListViewModel();
            NodeList.AddNodeType(() =>
            {
                DialogTreeBranch branch = null;
                BranchNode node;

                if (nodeListInitialized)
                {
                    m_ignoreBranchEvents = true;
                    branch = m_tree.CreateNewBranch();
                    m_ignoreBranchEvents = false;

                    node = new BranchNode(project, branch);
                    m_branchNodes.Add(branch, node);
                }
                else node = new BranchNode(project);

                return node;
            });
            nodeListInitialized = true;
        }

        /// <summary>
        /// Update the projects with the current state of the diagram when changed
        /// </summary>
        private void ConnectionChange(IChangeSet<ConnectionViewModel> change)
        {
            foreach (Change<ConnectionViewModel> changeCollection in change)
            {
                foreach (ConnectionViewModel connection in changeCollection?.Range ?? (IEnumerable<ConnectionViewModel>)new[] { changeCollection.Item.Current })
                {
                    BranchInput input = connection.Input as BranchInput;
                    BranchOutput output = connection.Output as BranchOutput;

                    if (input == null || output == null) continue;

                    DialogTreeBranch parent = m_project[output.Branch];
                    switch (changeCollection.Reason)
                    {
                        case ListChangeReason.Add:
                        case ListChangeReason.AddRange:
                            if (!parent.Children.Any(b => b == input.Branch))
                            {   // new link needs to be added in project
                                parent.AddChild(input.Branch);
                            }
                            break;
                        case ListChangeReason.Clear:
                        case ListChangeReason.Remove:
                        case ListChangeReason.RemoveRange:
                            if (parent.Children.Any(b => b == input.Branch))
                            {   // link needs to be removed in project
                                parent.RemoveChild(input.Branch);
                            }
                            break;
                        case ListChangeReason.Refresh:
                        case ListChangeReason.Moved:
                            break; // ignore as connections are still the same
                        case ListChangeReason.Replace:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        /// <summary>
        /// sets the dialog tree to display
        /// </summary>
        /// <param name="dialogTreeId">Dialog Tree ID</param>
        public void SetDialogTree(DialogTreeIdentifier dialogTreeId)
        {
            m_tree = m_project.ProjectDialogs.GetDialog(dialogTreeId);
            m_tree.OnBranchCreated += OnBranchCreated;
            m_tree.OnBranchRemoved += OnBranchRemoved;

            Title = string.IsNullOrWhiteSpace(m_tree.TreeName) ? "Script Visualizer" : m_tree.TreeName;
            ContentId = $"'{dialogTreeId}' SV";

            Network.Nodes.Clear();
            m_branchNodes.Clear();

            // find all branches and create nodes
            foreach (DialogTreeBranchIdentifier branch in m_tree.Branches)
            {
                DialogTreeBranch treeBranch = m_project[branch];
                if (treeBranch == null) continue;

                treeBranch.OnBranchChildAdded += (child) => TreeBranchOnBranchChildAdded(treeBranch, child);
                treeBranch.OnBranchChildRemoved += (child) => TreeBranchOnBranchChildRemoved(treeBranch, child);

                BranchNode branchNode = new BranchNode(m_project, branch);
                m_branchNodes.Add(branch, branchNode);
                Network.Nodes.Add(branchNode);
            }

            // link the branches
            foreach (DialogTreeBranchIdentifier branch in m_tree.Branches)
            {
                BranchNode node = m_branchNodes[branch];

                foreach (DialogTreeBranchIdentifier child in m_project[branch].Children)
                {
                    BranchNode childNode = m_branchNodes[child];
                    Network.Connections.Add(new ConnectionViewModel(Network, childNode.ParentPin, node.ChildPin));
                }
            }

            Logging.Logger.Info("Created new Script Dialog Panel");
            Logging.Logger.Info("Node Count: " + Network.Nodes.Count);
            Logging.Logger.Info("Connection Count: " + Network.Connections.Count);

            RaisePropertyChanged(nameof(Tree));
        }

        private void OnBranchCreated(DialogTreeBranch branch)
        {
            if (m_ignoreBranchEvents) return;
            BranchNode branchNode = new BranchNode(m_project, branch);

            branch.OnBranchChildAdded += (child) => TreeBranchOnBranchChildAdded(branch, child);
            branch.OnBranchChildRemoved += (child) => TreeBranchOnBranchChildRemoved(branch, child);

            m_branchNodes.Add(branch, branchNode);
            Network.Nodes.Add(branchNode);
        }

        private void OnBranchRemoved(DialogTreeBranch removed)
        {
            if (m_ignoreBranchEvents) return;

            Network.Nodes.Remove(m_branchNodes[removed]);
            m_branchNodes.Remove(removed);
        }

        private void TreeBranchOnBranchChildAdded(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
        {
            BranchNode node = m_branchNodes[parent];
            BranchNode childNode = m_branchNodes[child];

            if (Network.Connections.Items.Any(c => c.Input == childNode.ParentPin && c.Output == node.ChildPin))
                return; // connection already exists

            Network.Connections.Add(new ConnectionViewModel(Network, childNode.ParentPin, node.ChildPin));
        }

        private void TreeBranchOnBranchChildRemoved(DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier child)
        {
            BranchNode node = m_branchNodes[parent];
            BranchNode childNode = m_branchNodes[child];

            if (node == null || childNode == null) return;

            ConnectionViewModel connection = Network.Connections.Items
                .FirstOrDefault(c => c.Input == childNode.ParentPin && c.Output == node.ChildPin);
            Network.Connections.Remove(connection);
        }
    }
}
