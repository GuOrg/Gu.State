namespace Gu.State.Tests.Sandbox
{
    using Moq;

    using NUnit.Framework;

    public class TreeTests
    {
        [Test]
        public void TestName()
        {
            var changeTrackerMock = new Mock<ITracker>();
            var root = TrackerNode.CreateRoot(Mock.Of<IReference>(), changeTrackerMock.Object);
        }
    }
}