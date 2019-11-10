using System.ComponentModel;
using DynamicData;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using NpcChatSystem;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;
using ReactiveUI;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Node
{
    public class BranchNode : NodeViewModel
    {
        static BranchNode()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeView(), typeof(IViewFor<BranchNode>));
        }

        public BranchInput ParentPin { get; }
        public BranchOutput ChildPin { get; }
        public DialogTreeBranchIdentifier Branch => m_branch;

        private NpcChatProject m_project;
        private readonly DialogTreeBranchIdentifier m_branch;

        public BranchNode(NpcChatProject project)
        {
            m_project = project;
            m_branch = null;

            ParentPin = new BranchInput(m_project, this);
            ChildPin = new BranchOutput(this);
            Inputs.Add(ParentPin);
            Outputs.Add(ChildPin);
            CanBeRemovedByUser = false;

            Name = "Dialog Branch";
        }

        public BranchNode(NpcChatProject project, DialogTreeBranchIdentifier branchId)
            : this(project)
        {
            if (branchId != null)
            {
                m_branch = branchId;

                DialogTreeBranch branch = project[branchId];
                Name = branch.Name;
                branch.PropertyChanged += BranchChanged;
            }
        }

        private void BranchChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DialogTreeBranch.Name))
            {
                Name = m_project[Branch].Name;
            }
        }
    }
}
