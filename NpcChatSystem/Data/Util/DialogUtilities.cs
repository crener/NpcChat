using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpcChatSystem.Data.Dialog;
using NpcChatSystem.Identifiers;

namespace NpcChatSystem.Data.Util
{
    public static class DialogUtilities
    {
        /// <summary>
        /// Would adding <see cref="potentialChild"/> as a child of <see cref="parent"/> result in a circular dependency?
        /// </summary>
        /// <returns>true if adding this item would have a circular dependency</returns>
        public static bool CheckForCircularDependency(this DialogTree tree, DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier potentialChild)
        {
            if (!tree.Id.Compatible(parent) || !tree.Id.Compatible(potentialChild) || 
                !tree.HasBranch(parent) || !tree.HasBranch(potentialChild))
            {
                // If the segment isn't compatible with the tree there can't be a circular dependency
                return false;
            }

            if(parent == potentialChild)
            {
                // Adding yourself would defiantly cause a circular dependency
                return true;
            }

            foreach (DialogTreeBranchIdentifier childParent in tree[parent].Parents)
            {
                if(childParent == potentialChild ||
                   CheckCircularDependencyParents(tree, childParent, potentialChild))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CheckCircularDependencyParents(DialogTree tree, DialogTreeBranchIdentifier parent, DialogTreeBranchIdentifier potentialChild)
        {
            foreach (DialogTreeBranchIdentifier childParent in tree[parent].Parents)
            {
                if (childParent == potentialChild ||
                    CheckCircularDependencyParents(tree, childParent, potentialChild))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
