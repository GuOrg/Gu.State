namespace Gu.State.Tests.Internals.Collections
{
    using System;
    using Moq;
    using NUnit.Framework;

    public class DisposingCollectionTests
    {
        [Test]
        public void SetDisposesOld()
        {
            var disposableMock1 = new Mock<IDisposable>();
            var collection = new DisposingCollection<int, IDisposable>
            {
                [1] = disposableMock1.Object
            };

            disposableMock1.Verify(x => x.Dispose(), Times.Never);
            Assert.AreSame(disposableMock1.Object, collection[1]);
            var disposableMock2 = new Mock<IDisposable>();
            collection[1] = disposableMock2.Object;

            disposableMock1.Verify(x => x.Dispose(), Times.Once);
            disposableMock2.Verify(x => x.Dispose(), Times.Never);
            Assert.AreSame(disposableMock2.Object, collection[1]);
        }

        [Test]
        public void SetNullDisposesOld()
        {
            var disposableMock1 = new Mock<IDisposable>();
            var collection = new DisposingCollection<int, IDisposable>
            {
                [1] = disposableMock1.Object
            };

            disposableMock1.Verify(x => x.Dispose(), Times.Never);
            Assert.AreSame(disposableMock1.Object, collection[1]);
            collection[1] = null;

            disposableMock1.Verify(x => x.Dispose(), Times.Once);
            Assert.AreEqual(null, collection[1]);
        }

        [Test]
        public void Dispose()
        {
            var disposableMock1 = new Mock<IDisposable>();
            var collection = new DisposingCollection<int, IDisposable>
            {
                [1] = disposableMock1.Object,
            };

            collection.Dispose();
            disposableMock1.Verify(x=>x.Dispose(), Times.Once);
        }
    }
}
