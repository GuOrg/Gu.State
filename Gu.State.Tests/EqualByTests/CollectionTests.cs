namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class CollectionTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", true)]
        [TestCase("1, 2, 3", "1, 2", false)]
        [TestCase("1, 2", "1, 2, 3", false)]
        [TestCase("5, 2, 3", "1, 2, 3", false)]
        public void ArrayOfIntsStructural(string xs, string ys, bool expected)
        {
            var x = xs.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var y = ys.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            Assert.AreEqual(expected, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [Test]
        public void ListOfIntsToEmpty()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int>();
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListOfIntsEmptyToEmpty()
        {
            var x = new List<int>();
            var y = new List<int>();
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfIntsToLonger()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int> { 1, 2, 3, 4 };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListOfWithSimples()
        {
            var x = new List<WithSimpleProperties>
                        {
                            new WithSimpleProperties(
                                1,
                                2,
                                "a",
                                StringSplitOptions.RemoveEmptyEntries)
                        };
            var y = new List<WithSimpleProperties>
                        {
                            new WithSimpleProperties(
                                1,
                                2,
                                "a",
                                StringSplitOptions.RemoveEmptyEntries)
                        };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfComplexSameItems()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType>(x);
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ObservableCollectionOfIntsToEmpty()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int>();
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ObservableCollectionOfIntsToLonger()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int> { 1, 2, 3, 4 };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ObservableCollectionOfComplexType()
        {
            var x = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ArrayOfInts()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfImmutable()
        {
            var source = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var target = new[] { new Immutable(4), new Immutable(5), new Immutable(6) };
            this.EqualByMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }
    }
}