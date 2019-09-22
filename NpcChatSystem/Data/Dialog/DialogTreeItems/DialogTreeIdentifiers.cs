namespace NpcChatSystem.Data.Dialog.DialogTreeItems
{
    /// <summary>
    /// Identifier for a <see cref="DialogTree"/>
    /// </summary>
    public class DialogTreeIdentifier
    {
        public int DialogTreeId { get; }

        public DialogTreeIdentifier(int dialogTreeId)
        {
            DialogTreeId = dialogTreeId;
        }

        public bool Compatible(DialogTreeIdentifier diag)
        {
            if (DialogTreeId != diag.DialogTreeId) return false;

            return true;
        }
    }

    /// <summary>
    /// Identifier for a dialog tree part, also identifies a <see cref="DialogTreeBranch"/>s <see cref="DialogTree"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/>
    /// </summary>
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

        public bool Compatible(DialogTreeBranchIdentifier diag)
        {
            if(!base.Compatible(diag)) return false;
            if(DialogTreeBranchId != diag.DialogTreeBranchId) return false;

            return true;
        }
    }

    /// <summary>
    /// Identifier for a dialog segment, also identifies a <see cref="DialogSegment"/>s <see cref="DialogTree"/> and <see cref="DialogTreeBranch"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/> => <see cref="DialogSegment"/>
    /// </summary>
    public class DialogSegmentIdentifier : DialogTreeBranchIdentifier
    {
        public int DialogSegmentId { get; }

        public DialogSegmentIdentifier(DialogTreeBranchIdentifier dialogTree, int dialogSegmentId)
            : base(dialogTree.DialogTreeId, dialogTree.DialogTreeBranchId)
        {
            DialogSegmentId = dialogSegmentId;
        }

        public bool Compatible(DialogSegmentIdentifier diag)
        {
            if (!base.Compatible(diag)) return false;
            if (DialogSegmentId != diag.DialogSegmentId) return false;

            return true;
        }
    }
}
