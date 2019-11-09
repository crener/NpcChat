using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using ReactiveUI;

namespace NpcChat.ViewModels.Panels.ScriptDiagram
{
    public class BranchNode : NodeViewModel
    {
        static BranchNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<BranchNode>));
        }

        public NodeInputViewModel ParentPin { get; }
        public NodeOutputViewModel ChildPin { get; }

        private NpcChatProject m_project;
        private DialogTreeBranchIdentifier m_branch;

        public BranchNode(NpcChatProject project, DialogTreeBranchIdentifier branchId)
        {
            m_project = project;
            m_branch = branchId;

            ParentPin = new NodeInputViewModel()
            {
                MaxConnections = int.MaxValue,
                Name = "Parents",
            };
            ChildPin = new NodeOutputViewModel()
            {
                MaxConnections = int.MaxValue,
                Name = "Children",
            };

            Inputs.Add(ParentPin);
            Outputs.Add(ChildPin);
            CanBeRemovedByUser = false;

            if (branchId == null)
            {
                Name = "Branch";
                return;
            }

            DialogTreeBranch branch = project[branchId];
            Name = branch.Name;
        }
    }
}
