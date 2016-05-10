namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class CustomComparerTests
    {
        public abstract bool EqualMethod<T, TValue>(
            T x,
            T y,
            IEqualityComparer<TValue> comparer,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural)
            where T : class;

        [TestCase(true)]
        [TestCase(false)]
        public void WithSimpleHappyPath(bool expected)
        {
            var x = new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var comparerMock = new Mock<IEqualityComparer<WithSimpleProperties>>(MockBehavior.Strict);
            comparerMock.Setup(c => c.Equals(x, y))
                        .Returns(expected);
            var result = this.EqualMethod(x, y, comparerMock.Object);
            Assert.AreEqual(expected, result);
            comparerMock.Verify(c => c.Equals(It.IsAny<WithSimpleProperties>(), It.IsAny<WithSimpleProperties>()), Times.Once);

            result = this.EqualMethod(x, y, comparerMock.Object, ReferenceHandling.Throw);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(x, y, comparerMock.Object, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(x, y, comparerMock.Object, ReferenceHandling.References);
            Assert.AreEqual(expected, result);
        }

        [TestCase("b", "b", true)]
        [TestCase("b", "c", false)]
        public void WithWithSimpleHappyPath(string xn, string yn, bool expected)
        {
            var x = new With<WithSimpleProperties>(new WithSimpleProperties(1, 2, xn, StringSplitOptions.RemoveEmptyEntries));
            var y = new With<WithSimpleProperties>(new WithSimpleProperties(1, 2, yn, StringSplitOptions.RemoveEmptyEntries));
            var comparerMock = new Mock<IEqualityComparer<WithSimpleProperties>>(MockBehavior.Strict);
            comparerMock.Setup(c => c.Equals(x.Value, y.Value))
                        .Returns(expected);
            var result = this.EqualMethod(x, y, comparerMock.Object, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
            comparerMock.Verify(c => c.Equals(It.IsAny<WithSimpleProperties>(), It.IsAny<WithSimpleProperties>()), Times.Once);
        }
    }
}