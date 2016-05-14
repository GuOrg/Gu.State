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
            using (var map = DisposingMap<IDisposable>.Borrow())
            {
                map.Value.SetValue(0, disposableMock1.Object);
                disposableMock1.Verify(x => x.Dispose(), Times.Never);
                var disposableMock2 = new Mock<IDisposable>();
                map.Value.SetValue(0, disposableMock2.Object);

                disposableMock1.Verify(x => x.Dispose(), Times.Once);
                disposableMock2.Verify(x => x.Dispose(), Times.Never);
            }
        }

        [Test]
        public void SetNullDisposesOld()
        {
            var disposableMock1 = new Mock<IDisposable>();
            using (var map = DisposingMap<IDisposable>.Borrow())
            {
                map.Value.SetValue(0, disposableMock1.Object);

                disposableMock1.Verify(x => x.Dispose(), Times.Never);
                map.Value.SetValue(0, null);

                disposableMock1.Verify(x => x.Dispose(), Times.Once);
            }
        }

        [Test]
        public void Dispose()
        {
            var disposableMock1 = new Mock<IDisposable>();
            using (var map = DisposingMap<IDisposable>.Borrow())
            {
                map.Value.SetValue(0, disposableMock1.Object);
            }

            disposableMock1.Verify(x => x.Dispose(), Times.Once);
        }
    }
}