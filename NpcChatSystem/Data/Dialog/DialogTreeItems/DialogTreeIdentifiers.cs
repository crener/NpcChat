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
    public class DialogTreePartIdentifier : DialogTreeIdentifier
    {
        public int DialogTreePartId { get; }

        public DialogTreePartIdentifier(DialogTreeIdentifier dialogTree, int dialogTreePartId)
            : base(dialogTree.DialogTreeId)
        {
            DialogTreePartId = dialogTreePartId;
        }

        public DialogTreePartIdentifier(int dialogTreeId, int dialogTreePartId)
            : base(dialogTreeId)
        {
            DialogTreePartId = dialogTreePartId;
        }

        public bool Compatible(DialogTreePartIdentifier diag)
        {
            if(!base.Compatible(diag)) return false;
            if(DialogTreePartId != diag.DialogTreePartId) return false;

            return true;
        }
    }

    /// <summary>
    /// Identifier for a dialog segment, also identifies a <see cref="DialogSegment"/>s <see cref="DialogTree"/> and <see cref="DialogTreeBranch"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/> => <see cref="DialogSegment"/>
    /// </summary>
    public class DialogSegmentIdentifier : DialogTreePartIdentifier
    {
        public int DialogSegmentId { get; }

        public DialogSegmentIdentifier(DialogTreePartIdentifier dialogTree, int dialogSegmentId)
            : base(dialogTree.DialogTreeId, dialogTree.DialogTreePartId)
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
