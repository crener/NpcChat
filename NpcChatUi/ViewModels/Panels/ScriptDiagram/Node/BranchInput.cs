using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetwork;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Util;
using NpcChatSystem.Identifiers;
using ReactiveUI;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Node
{
    public class BranchInput : NodeInputViewModel
    {
        static BranchInput()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeInputView(), typeof(IViewFor<BranchInput>));
        }

        public BranchNode Node { get; }
        public DialogTreeBranchIdentifier Branch => Node?.Branch;
        private readonly NpcChatProject m_project;

        public BranchInput(NpcChatProject project)
        {
            Name = "Parents";
            MaxConnections = int.MaxValue;
            m_project = project;

            ConnectionValidator = ValidatePendingConnection;
        }

        public BranchInput(NpcChatProject project, BranchNode branch)
            : this(project)
        {
            Node = branch;
        }

        private ConnectionValidationResult ValidatePendingConnection(PendingConnectionViewModel arg)
        {
            DialogTree tree = m_project[(DialogTreeIdentifier)Branch];
            BranchOutput branchOutput = arg.Output as BranchOutput;

            if (tree == null || branchOutput == null || branchOutput.Branch == null ||
                tree.CheckForCircularDependency(branchOutput.Branch, Branch))
            {
                return new ConnectionValidationResult(false, null);
            }

            return new ConnectionValidationResult(true, null);
        }
    }
}
