namespace Gu.State.Internals
{
    using System;

    using Moq;

    using NUnit.Framework;

    public class RefcounterTests
    {
        [Test]
        public void DisposingDecrements()
        {
            var disposableMock = new Mock<IDisposable>(MockBehavior.Strict);
            IDisposer<IDisposable> disposer1;
            Assert.AreEqual(true, disposableMock.Object.TryRefCount(out disposer1));

            IDisposer<IDisposable> disposer2;
            Assert.AreEqual(true, disposableMock.Object.TryRefCount(out disposer2));
            Assert.AreSame(disposer1, disposer2);

            disposableMock.Setup(x => x.Dispose());
            disposer1.Dispose();
            disposableMock.Verify(x => x.Dispose(), Times.Never);

            disposer2.Dispose();
            disposableMock.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void TryRefCountDisposedReturnsFalse()
        {
            var disposableMock = new Mock<IDisposable>(MockBehavior.Strict);
            IDisposer<IDisposable> disposer1;
            Assert.AreEqual(true, disposableMock.Object.TryRefCount(out disposer1));

            disposableMock.Setup(x => x.Dispose());
            disposer1.Dispose();
            disposableMock.Verify(x => x.Dispose(), Times.Once);

            IDisposer<IDisposable> disposer3;
            Assert.AreEqual(false, disposableMock.Object.TryRefCount(out disposer3));
        }
    }
}
