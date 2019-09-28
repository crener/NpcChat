using System.Diagnostics;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Data.Dialog.DialogTreeItems;

namespace NpcChatSystem.Identifiers
{
    /// <summary>
    /// Identifier for a <see cref="DialogTree"/>
    /// </summary>
    [DebuggerDisplay("Tree: {DialogTreeId}")]
    public class DialogTreeIdentifier
    {
        public int DialogTreeId { get; }

        public DialogTreeIdentifier(int dialogTreeId)
        {
            DialogTreeId = dialogTreeId;
        }

        /// <summary>
        /// Are both tree identifiers referencing the same <see cref="DialogTree"/>?
        /// </summary>
        /// <param name="tree">other <see cref="DialogTreeIdentifier"/> to compare</param>
        /// <returns>true if identifiers reference the same <see cref="DialogTree"/></returns>
        public bool Compatible(DialogTreeIdentifier tree)
        {
            if (DialogTreeId != tree.DialogTreeId) return false;

            return true;
        }
    }

    /// <summary>
    /// Identifier for a dialog tree part, also identifies a <see cref="DialogTreeBranch"/>s <see cref="DialogTree"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/>
    /// </summary>
    [DebuggerDisplay("Tree: {DialogTreeId}, Branch: {DialogTreeBranchId}")]
    public class DialogTreeBranchIdentifier : DialogTreeIdentifier
    {
        public int DialogTreeBranchId { get; }

        public DialogTreeBranchIdentifier(DialogTreeIdentifier dialogTree, int dialogTreeBranchId)
            : base(dialogTree.DialogTreeId)
        {
            DialogTreeBranchId = dialogTreeBranchId;
        }

        public DialogTreeBranchIdentifier(int dialogTreeId, int dialogTreeBranchId)
            : base(dialogTreeId)
        {
            DialogTreeBranchId = dialogTreeBranchId;
        }

        /// <summary>
        /// Are both branch identifiers referencing the same <see cref="DialogTreeBranch"/>?
        /// </summary>
        /// <param name="branch">other <see cref="DialogTreeBranchIdentifier"/> to compare</param>
        /// <returns>true if identifiers reference the same <see cref="DialogTreeBranch"/></returns>
        public bool Compatible(DialogTreeBranchIdentifier branch)
        {
            if(!base.Compatible(branch)) return false;
            if(DialogTreeBranchId != branch.DialogTreeBranchId) return false;

            return true;
        }
    }

    /// <summary>
    /// Identifier for a dialog segment, also identifies a <see cref="DialogSegment"/>s <see cref="DialogTree"/> and <see cref="DialogTreeBranch"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/> => <see cref="DialogSegment"/>
    /// </summary>
    [DebuggerDisplay("Tree: {DialogTreeId}, Branch: {DialogTreeBranchId}, Segment: {DialogSegmentId}")]
    public class DialogSegmentIdentifier : DialogTreeBranchIdentifier
    {
        public int DialogSegmentId { get; }

        public DialogSegmentIdentifier(DialogTreeBranchIdentifier dialogTree, int dialogSegmentId)
            : base(dialogTree.DialogTreeId, dialogTree.DialogTreeBranchId)
        {
            DialogSegmentId = dialogSegmentId;
        }

        public DialogSegmentIdentifier(int tree, int branch, int dialogSegmentId)
            : base(tree, branch)
        {
            DialogSegmentId = dialogSegmentId;
        }

        /// <summary>
        /// Are both segment identifiers referencing the same <see cref="DialogSegment"/>?
        /// </summary>
        /// <param name="segment">other <see cref="DialogSegmentIdentifier"/> to compare</param>
        /// <returns>true if identifiers reference the same <see cref="DialogSegment"/></returns>
        public bool Compatible(DialogSegmentIdentifier segment)
        {
            if (!base.Compatible(segment)) return false;
            if (DialogSegmentId != segment.DialogSegmentId) return false;

            return true;
        }
    }
}
