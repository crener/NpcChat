using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Configuration;
using NpcChatSystem.Data.Dialog;

namespace NpcChatSystem.Identifiers
{
    /// <summary>
    /// Identifier for a <see cref="DialogTree"/>
    /// </summary>
    [DebuggerDisplay("Tree: {TreeName}")]
    public class DialogTreeIdentifier
    {
        public int DialogTreeId { get; }

        public string TreeName => m_tree?.TreeName ?? DialogTreeId.ToString();

        private readonly DialogTree m_tree; 
        
        public DialogTreeIdentifier(int dialogTreeId, DialogTree tree = null)
        {
            DialogTreeId = dialogTreeId;
            m_tree = tree;
        }

        /// <summary>
        /// Are both tree identifiers referencing the same <see cref="DialogTree"/>?
        /// </summary>
        /// <param name="tree">other <see cref="DialogTreeIdentifier"/> to compare</param>
        /// <returns>true if identifiers reference the same <see cref="DialogTree"/></returns>
        public bool Compatible(DialogTreeIdentifier tree)
        {
            if(ReferenceEquals(tree, null)) return false;

            if(DialogTreeId != tree.DialogTreeId) return false;
            return true;
        }

        public static bool operator ==(DialogTreeIdentifier a, DialogTreeIdentifier b)
        {
            bool aNull = ReferenceEquals(a, null);
            bool bNull = ReferenceEquals(b, null);

            if(aNull && bNull) return true;
            if(aNull || bNull) return false;

            return a.Equals(b);
        }

        public static bool operator !=(DialogTreeIdentifier a, DialogTreeIdentifier b)
        {
            return !(a == b);
        }

        protected bool Equals(DialogTreeIdentifier other)
        {
            return Compatible(other);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((DialogTreeIdentifier) obj);
        }

        public override string ToString()
        {
            return $"T{DialogTreeId}";
        }
        
        public string ToString(string format)
        {
            if(string.IsNullOrEmpty(format)) return ToString();

            switch (format.ToLower())
            {
                case "h":
                case "human":
                    return $"Tree:{TreeName}";
                default:
                    return ToString();
            }
        }

        public override int GetHashCode()
        {
            return DialogTreeId;
        }
    }

    /// <summary>
    /// Identifier for a dialog tree part, also identifies a <see cref="DialogTreeBranch"/>s <see cref="DialogTree"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/>
    /// </summary>
    [DebuggerDisplay("Tree: {TreeName}, Branch: {BranchName}")]
    public class DialogTreeBranchIdentifier : DialogTreeIdentifier
    {
        public int DialogTreeBranchId { get; }

        public string BranchName => m_branch?.Name ?? DialogTreeBranchId.ToString();

        private readonly DialogTreeBranch m_branch;
        
        public DialogTreeBranchIdentifier(DialogTreeIdentifier dialogTree, int dialogTreeBranchId, DialogTree tree = null, DialogTreeBranch branch = null)
            : this(dialogTree.DialogTreeId, dialogTreeBranchId, tree, branch) { }
        
        public DialogTreeBranchIdentifier(DialogTree tree, int dialogTreeBranchId, DialogTreeBranch branch = null)
            : this(tree.Id.DialogTreeId, dialogTreeBranchId, tree, branch) { }

        public DialogTreeBranchIdentifier(int dialogTreeId, int dialogTreeBranchId, DialogTree tree = null, DialogTreeBranch branch = null)
            : base(dialogTreeId, tree)
        {
            DialogTreeBranchId = dialogTreeBranchId;
            m_branch = branch;
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

        public static bool operator ==(DialogTreeBranchIdentifier a, DialogTreeBranchIdentifier b)
        {
            bool aNull = ReferenceEquals(a, null);
            bool bNull = ReferenceEquals(b, null);

            if(aNull && bNull) return true;
            if(aNull || bNull) return false;

            return a.Equals(b);
        }

        public static bool operator !=(DialogTreeBranchIdentifier a, DialogTreeBranchIdentifier b)
        {
            return !(a == b);
        }

        protected bool Equals(DialogTreeBranchIdentifier other)
        {
            return Compatible(other);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((DialogTreeBranchIdentifier) obj);
        }

        public override string ToString()
        {
            return $"{base.ToString()}.B{DialogTreeBranchId}";
        }

        public new string ToString(string format)
        {
            if(string.IsNullOrEmpty(format)) return ToString();

            switch (format.ToLower())
            {
                case "h":
                case "human":
                    return $"{base.ToString(format)}, Branch:{BranchName}";
                default:
                    return ToString();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ DialogTreeBranchId;
            }
        }
    }

    /// <summary>
    /// Identifier for a dialog segment, also identifies a <see cref="DialogSegment"/>s <see cref="DialogTree"/> and <see cref="DialogTreeBranch"/>.
    ///
    /// <see cref="DialogTree"/> => <see cref="DialogTreeBranch"/> => <see cref="DialogSegment"/>
    /// </summary>
    [DebuggerDisplay("Tree: {TreeName}, Branch: {BranchName}, Segment: {DialogSegmentId}")]
    public class DialogSegmentIdentifier : DialogTreeBranchIdentifier
    {
        public int DialogSegmentId { get; }

        public DialogSegmentIdentifier(DialogTreeBranch branch, int dialogSegmentId, DialogTree tree = null)
            : this(branch.Id.DialogTreeId, branch.Id.DialogTreeBranchId, dialogSegmentId, tree, branch) { }
        
        public DialogSegmentIdentifier(DialogTreeBranchIdentifier dialogTree, int dialogSegmentId, DialogTree tree = null, DialogTreeBranch branch = null)
            : this(dialogTree.DialogTreeId, dialogTree.DialogTreeBranchId, dialogSegmentId, tree,  branch) { }

        public DialogSegmentIdentifier(int treeId, int branchId, int dialogSegmentId, DialogTree tree = null, DialogTreeBranch branch = null)
            : base(treeId, branchId, tree, branch)
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
            if(!base.Compatible(segment)) return false;
            if(DialogSegmentId != segment.DialogSegmentId) return false;

            return true;
        }

        public static bool operator ==(DialogSegmentIdentifier a, DialogSegmentIdentifier b)
        {
            if(ReferenceEquals(null, a)) return false;
            if(ReferenceEquals(null, b)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(DialogSegmentIdentifier a, DialogSegmentIdentifier b)
        {
            return !(a == b);
        }

        protected bool Equals(DialogSegmentIdentifier other)
        {
            return Compatible(other);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((DialogSegmentIdentifier) obj);
        }

        public override string ToString()
        {
            return $"{base.ToString()}.S{DialogSegmentId}";
        }

        public new string ToString(string format)
        {
            if(string.IsNullOrEmpty(format)) return ToString();

            switch (format.ToLower())
            {
                case "h":
                case "human":
                    return $"{base.ToString(format)}, Segment: {DialogSegmentId}";
                default:
                    return ToString();
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ DialogTreeBranchId;
            }
        }
    }
}