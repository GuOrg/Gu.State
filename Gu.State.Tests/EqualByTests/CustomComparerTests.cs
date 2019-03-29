// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests
{
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class CustomComparerTests
    {
        public abstract bool EqualBy<T, TValue>(
            T x,
            T y,
            IEqualityComparer<TValue> comparer,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural)
            where T : class;

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithSimpleHappyPath(ReferenceHandling referenceHandling)
        {
            var x = new WithSimpleProperties();
            var y = new WithSimpleProperties();
            foreach (var expected in new[] { true, false })
            {
                var comparerMock = new Mock<IEqualityComparer<WithSimpleProperties>>(MockBehavior.Strict);
                comparerMock.Setup(c => c.Equals(x, y))
                            .Returns(expected);
                var result = this.EqualBy(x, y, comparerMock.Object, referenceHandling);
                Assert.AreEqual(expected, result);
                comparerMock.Verify(
                    c => c.Equals(It.IsAny<WithSimpleProperties>(), It.IsAny<WithSimpleProperties>()),
                    Times.Once);
            }
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithWithSimpleHappyPath(ReferenceHandling referenceHandling)
        {
            var x = new With<WithSimpleProperties>(new WithSimpleProperties());
            var y = new With<WithSimpleProperties>(new WithSimpleProperties());
            foreach (var expected in new[] { true, false })
            {
                var comparerMock = new Mock<IEqualityComparer<WithSimpleProperties>>(MockBehavior.Strict);
                comparerMock.Setup(c => c.Equals(x.Value, y.Value))
                            .Returns(expected);
                var result = this.EqualBy(x, y, comparerMock.Object, referenceHandling);
                Assert.AreEqual(expected, result);
                comparerMock.Verify(
                    c => c.Equals(It.IsAny<WithSimpleProperties>(), It.IsAny<WithSimpleProperties>()),
                    Times.Once);
            }
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithIntCollectionWithExplicitComparer(ReferenceHandling referenceHandling)
        {
            var x = new With<IntCollection>(new IntCollection(1));
            var y = new With<IntCollection>(new IntCollection(1));
            foreach (var expected in new[] { true, false })
            {
                var comparerMock = new Mock<IEqualityComparer<IntCollection>>(MockBehavior.Strict);
                comparerMock.Setup(c => c.Equals(x.Value, y.Value))
                            .Returns(expected);
                var result = this.EqualBy(x, y, comparerMock.Object, referenceHandling);
                Assert.AreEqual(expected, result);
                comparerMock.Verify(c => c.Equals(It.IsAny<IntCollection>(), It.IsAny<IntCollection>()), Times.Once);
            }
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void IntCollectionWithExplicitComparer(ReferenceHandling referenceHandling)
        {
            var x = new IntCollection(1);
            var y = new IntCollection(1);
            foreach (var expected in new[] { true, false })
            {
                var comparerMock = new Mock<IEqualityComparer<IntCollection>>(MockBehavior.Strict);
                comparerMock.Setup(c => c.Equals(x, y))
                            .Returns(expected);
                var result = this.EqualBy(x, y, comparerMock.Object, referenceHandling);
                Assert.AreEqual(expected, result);
                comparerMock.Verify(c => c.Equals(It.IsAny<IntCollection>(), It.IsAny<IntCollection>()), Times.Once);
            }
        }
    }
}