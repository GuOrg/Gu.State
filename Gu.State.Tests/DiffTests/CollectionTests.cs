namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NUnit.Framework;

    using static DiffTypes;

    public abstract class CollectionTests
    {
        public abstract bool DiffMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", true)]
        [TestCase("1, 2, 3", "1, 2", false)]
        [TestCase("1, 2", "1, 2, 3", false)]
        [TestCase("5, 2, 3", "1, 2, 3", false)]
        public void ArrayOfIntsStructural(string xs, string ys, bool expected)
        {
            var x = xs.Split(',').Select(int.Parse).ToArray();
            var y = ys.Split(',').Select(int.Parse).ToArray();
            Assert.AreEqual(expected, this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [Test]
        public void ListOfIntsToEmpty()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListOfIntsEmptyToEmpty()
        {
            var x = new List<int>();
            var y = new List<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfIntsToLonger()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int> { 1, 2, 3, 4 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ListOfWithSimples()
        {
            var x = new List<WithSimpleProperties> { new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var y = new List<WithSimpleProperties> { new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfComplexSameItems()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType>(x);
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ObservableCollectionOfIntsToEmpty()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ObservableCollectionOfIntsToLonger()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int> { 1, 2, 3, 4 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ObservableCollectionOfComplexType()
        {
            var x = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ArrayOfInts()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfImmutable()
        {
            var source = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var target = new[] { new Immutable(4), new Immutable(5), new Immutable(6) };
            this.DiffMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(1, "one", true)]
        [TestCase(2, "one", false)]
        [TestCase(1, "two", false)]
        public void DictionaryToSameLength(int key, string value, bool expected)
        {
            var x = new Dictionary<int, string> { { key, value } };
            var y = new Dictionary<int, string> { { 1, "one" } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DictionaryToLonger()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void HashSetOfInts()
        {
            Assert.Inconclusive("Reminder");
        }

        [TestCase("1, 2, 3", "1, 2, 3", true)]
        [TestCase("1, 2, 3", "1, 2", false)]
        [TestCase("1, 2", "1, 2, 3", false)]
        [TestCase("5, 2, 3", "1, 2, 3", false)]
        public void EnumarebleStructural(string xs, string ys, bool expected)
        {
            var x = xs.Split(',').Select(int.Parse);
            var y = ys.Split(',').Select(int.Parse);
            Assert.AreEqual(expected, this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [TestCase(0, 0, 0, 0, true)]
        [TestCase(0, 1, 0, 1, true)]
        [TestCase(1, 1, 1, 1, true)]
        [TestCase(0, 2, 0, 1, false)]
        [TestCase(0, 1, 0, 2, false)]
        [TestCase(1, 1, 0, 1, false)]
        [TestCase(0, 1, 1, 1, false)]
        public void EnumarebleRepeatStructural(int startX, int countX, int startY, int countY, bool expected)
        {
            var x = Enumerable.Repeat(startX, countX);
            var y = Enumerable.Repeat(startY, countY);
            Assert.AreEqual(expected, this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [Test]
        public void EnumarebleNullsStructural()
        {
            var x = new object[] { 1, null }.Select(z => z);
            var y = new object[] { 1, null }.Select(z => z);
            Assert.AreEqual(true, this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural));

            x = new object[] { 1 }.Select(z => z);
            y = new object[] { 1, null }.Select(z => z);
            Assert.AreEqual(false, this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural));

            x = new object[] { 1, null }.Select(z => z);
            y = new object[] { 1 }.Select(z => z);
            Assert.AreEqual(false, this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }
    }
}