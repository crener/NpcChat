using NpcChatSystem.Identifiers;
using NUnit.Framework;

namespace NpcChatTest.Identifiers
{
    public class DialogIdentifierTests
    {
        [Test]
        public void Tree()
        {
            DialogTreeIdentifier id1 = new DialogTreeIdentifier(23);
            DialogTreeIdentifier id2 = new DialogTreeIdentifier(666);

            Assert.IsFalse(id1.Compatible(id2));
            Assert.IsTrue(id1.Compatible(new DialogTreeIdentifier(id1.DialogTreeId)));
        }

        [Test]
        public void Branch()
        {
            DialogTreeBranchIdentifier id1 = new DialogTreeBranchIdentifier(23, 2);
            DialogTreeBranchIdentifier id2 = new DialogTreeBranchIdentifier(666, 2);

            Assert.IsFalse(id1.Compatible(id2));

            id2 = new DialogTreeBranchIdentifier(23, 2);
            Assert.IsTrue(id1.Compatible(id2));

            id2 = new DialogTreeBranchIdentifier(23, 23);
            Assert.IsFalse(id1.Compatible(id2));
        }

        [Test]
        public void SegmentBranch()
        {
            DialogSegmentIdentifier id1 = new DialogSegmentIdentifier(23, 2, 30);
            DialogSegmentIdentifier id2 = new DialogSegmentIdentifier(666, 2, 30);

            Assert.IsFalse(id1.Compatible(id2));

            id2 = new DialogSegmentIdentifier(23, 2, 30);
            Assert.IsTrue(id1.Compatible(id2));

            id2 = new DialogSegmentIdentifier(23, 23, 30);
            Assert.IsFalse(id1.Compatible(id2));
            id2 = new DialogSegmentIdentifier(23, 56, 30);
            Assert.IsFalse(id1.Compatible(id2));
        }

        [Test]
        public void BranchToTree()
        {
            DialogTreeIdentifier id1 = new DialogTreeIdentifier(23);
            DialogTreeBranchIdentifier id2 = new DialogTreeBranchIdentifier(666, 2);

            Assert.IsFalse(id1.Compatible(id2));

            id2 = new DialogTreeBranchIdentifier(23, 2);
            Assert.IsTrue(id1.Compatible(id2));
        }

        [Test]
        public void SegmentToTree()
        {
            DialogTreeIdentifier id1 = new DialogTreeIdentifier(23);
            DialogSegmentIdentifier id2 = new DialogSegmentIdentifier(666, 2, 5);

            Assert.IsFalse(id1.Compatible(id2));

            id2 = new DialogSegmentIdentifier(23, 2, 5);
            Assert.IsTrue(id1.Compatible(id2));
        }

        [Test]
        public void SegmentToBranch()
        {
            DialogTreeBranchIdentifier id1 = new DialogTreeBranchIdentifier(23, 2);
            DialogSegmentIdentifier id2 = new DialogSegmentIdentifier(666, 2, 5);

            Assert.IsFalse(id1.Compatible(id2));

            id2 = new DialogSegmentIdentifier(23, 2, 5);
            Assert.IsTrue(id1.Compatible(id2));
            id2 = new DialogSegmentIdentifier(23, 2, 343);
            Assert.IsTrue(id1.Compatible(id2));
        }
    }
}
