using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetwork.ViewModels;
using NodeNetwork.Views;
using NpcChatSystem.Identifiers;
using ReactiveUI;

namespace NpcChat.ViewModels.Panels.ScriptDiagram.Node
{
    [DebuggerDisplay("{(Node?.Name ?? nameof(BranchOutput))}")]
    public class BranchOutput : NodeOutputViewModel
    {
        static BranchOutput()
        {
            Splat.Locator.CurrentMutable.Register(() => new NodeOutputView(), typeof(IViewFor<BranchOutput>));
        }

        public BranchNode Node { get; }
        public DialogTreeBranchIdentifier Branch => Node?.Branch;

        public BranchOutput()
        {
            Name = "Children";
            MaxConnections = int.MaxValue;
        }

        public BranchOutput(BranchNode branch) : this()
        {
            Node = branch;
        }
    }
}
