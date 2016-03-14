namespace Gu.State.Tests.Internals.Collections
{
    using System;

    using Moq;

    using NUnit.Framework;

    public class RefCountCollectionTests
    {
        [Test]
        public void GetOrAddSameReturnsSame()
        {
            var collection = new RefCountCollection<IDisposable>();
            var s1 = new object();
            var tracker1 = collection.GetOrAdd(s1, () => new Mock<IDisposable>().Object);
            var tracker2 = collection.GetOrAdd(s1, () => new Mock<IDisposable>().Object);
            Assert.AreSame(tracker1, tracker2);
        }

        [Test]
        public void GetOrAddDifferentReturnsDifferent()
        {
            var collection = new RefCountCollection<IDisposable>();
            var s1 = new object();
            var tracker1 = collection.GetOrAdd(s1, () => new Mock<IDisposable>().Object);
            var s2 = new object();
            var tracker2 = collection.GetOrAdd(s2, () => new Mock<IDisposable>().Object);
            Assert.AreNotSame(tracker1, tracker2);
        }

        [Test]
        public void Dispose()
        {
            var collection = new RefCountCollection<IDisposable>();
            var trackers = new[] { new Mock<IDisposable>(), new Mock<IDisposable>() };
            var s1 = new object();
            var tracker1 = collection.GetOrAdd(s1, () => trackers[0].Object);
            var s2 = new object();
            var tracker2 = collection.GetOrAdd(s2, () => trackers[1].Object);
            Assert.AreNotSame(tracker1, tracker2);
            collection.Dispose();
            trackers[0].Verify(x => x.Dispose(), Times.Once);
            trackers[1].Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void DisposeItem()
        {
            var collection = new RefCountCollection<IDisposable>();
            var s1 = new object();
            var tracker1 = collection.GetOrAdd(s1, () => new Mock<IDisposable>().Object);
            tracker1.Dispose();
            var tracker2 = collection.GetOrAdd(s1, () => new Mock<IDisposable>().Object);
            Assert.AreNotSame(tracker1, tracker2);
        }
    }
}