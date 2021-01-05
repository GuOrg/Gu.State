namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class CollectionTests
    {
        public abstract bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void WithEquatableIntCollection(ReferenceHandling referenceHandling)
        {
            var x = new With<EquatableIntCollection>(new EquatableIntCollection(1));
            var y = new With<EquatableIntCollection>(new EquatableIntCollection(1));
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithIntCollectionReferences()
        {
            var x = new With<IntCollection>(new IntCollection(1));
            var y = new With<IntCollection>(new IntCollection(1));
            var result = this.EqualBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListOfWithSimpleProperties()
        {
            var x = new List<WithSimpleProperties> { new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var y = new List<WithSimpleProperties> { new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithListOfPoints()
        {
            var x = new With<List<Point>>(new List<Point> { new Point(1, 2), new Point(1, 2) });
            var y = new With<List<Point>>(new List<Point> { new Point(1, 2), new Point(1, 2) });

            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListOfComplexSameItems()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType>(x);
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }
    }
}
