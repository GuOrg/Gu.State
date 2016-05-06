//namespace Gu.State.Tests.Internals.Collections
//{
//    using Moq;
//    using NUnit.Framework;

//    public class RefCountCollectionTests
//    {
//        [Test]
//        public void GetOrAddSameReturnsSame()
//        {
//            var collection = new RefCountCollection<IRefCountable>();
//            var s1 = new object();
//            var tracker1 = collection.GetOrAdd(this, s1, () => new Mock<IRefCountable>().Object);
//            var tracker2 = collection.GetOrAdd(this, s1, () => new Mock<IRefCountable>().Object);
//            Assert.AreSame(tracker1, tracker2);
//        }

//        [Test]
//        public void GetOrAddDifferentReturnsDifferent()
//        {
//            var collection = new RefCountCollection<IRefCountable>();
//            var s1 = new object();
//            var tracker1 = collection.GetOrAdd(this, s1, () => new Mock<IRefCountable>().Object);
//            var s2 = new object();
//            var tracker2 = collection.GetOrAdd(this, s2, () => new Mock<IRefCountable>().Object);
//            Assert.AreNotSame(tracker1, tracker2);
//        }

//        [Test]
//        public void Dispose()
//        {
//            var collection = new RefCountCollection<IRefCountable>();
//            var trackers = new[] { new Mock<IRefCountable>(), new Mock<IRefCountable>() };
//            var s1 = new object();
//            var tracker1 = collection.GetOrAdd(this, s1, () => trackers[0].Object);
//            var s2 = new object();
//            var tracker2 = collection.GetOrAdd(this, s2, () => trackers[1].Object);
//            Assert.AreNotSame(tracker1, tracker2);
//            collection.Dispose();
//            trackers[0].Verify(x => x.Dispose(), Times.Once);
//            trackers[1].Verify(x => x.Dispose(), Times.Once);
//        }

//        [Test]
//        public void RemoveOwner()
//        {
//            var collection = new RefCountCollection<IRefCountable>();
//            var s1 = new object();
//            var tracker1 = collection.GetOrAdd(this, s1, () => new Mock<IRefCountable>().Object);
//            tracker1.RemoveOwner(this);
//            var tracker2 = collection.GetOrAdd(this, s1, () => new Mock<IRefCountable>().Object);
//            Assert.AreNotSame(tracker1, tracker2);
//        }
//    }
//}