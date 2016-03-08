namespace Gu.State.Tests.Internals.Tree
{
    using Moq;

    using NUnit.Framework;

    public class TreeTests
    {
        [Test]
        public void TestName()
        {
            var rootTrackerMock = new Mock<ITracker>();
            var rootRef = Mock.Of<IReference>();
            var root = TrackerNode.CreateRoot(rootRef, rootTrackerMock.Object);
            var childRef = Mock.Of<IReference>();
            var childTrackerMock = new Mock<ITracker>();
            root.AddChild(childRef, () => childTrackerMock.Object);

        }
    }
}