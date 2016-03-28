namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.State.Tests.EqualByTests;

    using NUnit.Framework;

    public abstract class CollectionTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

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
            var x = new List<int> ();
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
            var x = new List<EqualByTypes.WithSimpleProperties> { new EqualByTypes.WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var y = new List<EqualByTypes.WithSimpleProperties> { new EqualByTypes.WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<EqualByTypes.ComplexType> { new EqualByTypes.ComplexType("b", 2), new EqualByTypes.ComplexType("c", 3) };
            var y = new List<EqualByTypes.ComplexType> { new EqualByTypes.ComplexType("b", 2), new EqualByTypes.ComplexType("c", 3) };
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
            var x = new List<EqualByTypes.ComplexType> { new EqualByTypes.ComplexType("b", 2), new EqualByTypes.ComplexType("c", 3) };
            var y = new List<EqualByTypes.ComplexType>(x);
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
            var x = new ObservableCollection<EqualByTypes.ComplexType> { new EqualByTypes.ComplexType("b", 2), new EqualByTypes.ComplexType("c", 3) };
            var y = new ObservableCollection<EqualByTypes.ComplexType> { new EqualByTypes.ComplexType("b", 2), new EqualByTypes.ComplexType("c", 3) };
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
            var source = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            var target = new[] { new EqualByTypes.Immutable(4), new EqualByTypes.Immutable(5), new EqualByTypes.Immutable(6) };
            this.EqualByMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(1,"one", true)]
        [TestCase(2,"one", false)]
        [TestCase(1,"two", false)]
        public void DictionaryToSameLength(int key, string value, bool expected)
        {
            var x = new Dictionary<int, string> { { key, value } };
            var y = new Dictionary<int, string> { { 1, "one" } };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string>();
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DictionaryToLonger()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void HashSetOfInts()
        {
            Assert.Inconclusive("Reminder");
        }
    }
}