namespace Gu.State.Tests.Internals.Collections
{
    using System;
    using Moq;
    using NUnit.Framework;

    public class DisposingMapTests
    {
        [Test]
        public void SetDisposesOld()
        {
            var disposableMock1 = new Mock<IDisposable>();
            var collection = new DisposingMap<IDisposable>();
            collection.SetValue(0, disposableMock1.Object);
            disposableMock1.Verify(x => x.Dispose(), Times.Never);
            var disposableMock2 = new Mock<IDisposable>();
            collection.SetValue(0, disposableMock2.Object);

            disposableMock1.Verify(x => x.Dispose(), Times.Once);
            disposableMock2.Verify(x => x.Dispose(), Times.Never);
        }

        [Test]
        public void SetNullDisposesOld()
        {
            var disposableMock1 = new Mock<IDisposable>();
            var collection = new DisposingMap<IDisposable>();
            collection.SetValue(0, disposableMock1.Object);

            disposableMock1.Verify(x => x.Dispose(), Times.Never);
            collection.SetValue(0, null);

            disposableMock1.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void Dispose()
        {
            var disposableMock1 = new Mock<IDisposable>();
            var collection = new DisposingMap<IDisposable>();
            collection.SetValue(0, disposableMock1.Object);

            collection.Dispose();
            disposableMock1.Verify(x => x.Dispose(), Times.Once);
        }
    }
}