using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicData;
using NodeNetwork;
using NodeNetwork.Toolkit;
using NodeNetwork.ViewModels;
using NpcChat.ViewModels.Panels.ScriptEditor;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using NpcChatSystem.Utilities;
using Xceed.Wpf.AvalonDock.Layout;

namespace NpcChat.ViewModels.Panels.ScriptDiagram
{
    public class ScriptDiagramVM : LayoutDocument
    {
        public DialogTree Tree => m_tree;
        public NetworkViewModel Network { get; }

        private NpcChatProject m_project { get; }
        private DialogTree m_tree;
        private readonly Dictionary<DialogTreeBranchIdentifier, BranchNode> m_branchNodes = new Dictionary<DialogTreeBranchIdentifier, BranchNode>();

        public ScriptDiagramVM(NpcChatProject project, DialogTreeIdentifier dialog = null)
        {
            Title = "Script Visualizer";
            ToolTip = "Script Visualizer";
            CanClose = true;
            m_project = project;

            Network = new NetworkViewModel();
            /*Network.Validator = n =>
            {
                // don't allow loops
                if (GraphAlgorithms.FindLoops(n).Any())
                    return new NetworkValidationResult(false, false, new ErrorMessageViewModel("Network contains loops!"));

                return new NetworkValidationResult(true, true, null);
            };*/

            SetDialogTree(dialog);
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

            this.Network.Nodes.Clear();
            m_branchNodes.Clear();

            // find all branches and create nodes
            foreach (DialogTreeBranchIdentifier branch in m_tree.Branches)
            {
                BranchNode branchNode = new BranchNode(m_project, branch);
                m_branchNodes.Add(branch, branchNode);
                Network.Nodes.Add(branchNode);
            }

            // link the branches
            /*foreach (DialogTreeBranchIdentifier branch in m_tree.Branches)
            {
                BranchNode node = m_branchNodes[branch];

                foreach (DialogTreeBranchIdentifier child in m_project[branch].Children)
                {
                    BranchNode childNode = m_branchNodes[child];
                    Network.Connections.Add(new ConnectionViewModel(Network, node.ParentPin, childNode.ChildPin));
                }
            }*/

            Logging.Logger.Info("Created new Script Dialog Panel");
            Logging.Logger.Info("Node Count: " + Network.Nodes.Count);
            Logging.Logger.Info("Connection Count: " + Network.Connections.Count);
            RaisePropertyChanged(nameof(Tree));
            RaisePropertyChanged(nameof(Network));
        }

        private void OnBranchCreated(DialogTreeBranch obj)
        {

        }

        private void OnBranchRemoved(DialogTreeBranch removed)
        {

        }
    }
}
