namespace Gu.ChangeTracking.Tests.CopyTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public abstract class CollectionTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [Test]
        public void ListOfIntsToEmpty()
        {
            var source = new List<int> { 1, 2, 3 };
            var target = new List<int>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { 1, 2, 3 };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ListOfImmutablesToEmpty(ReferenceHandling referenceHandling)
        {
            var source = new List<CopyTypes.Immutable> { new CopyTypes.Immutable(1), new CopyTypes.Immutable(2) };
            var target = new List<CopyTypes.Immutable>();
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { new CopyTypes.Immutable(1), new CopyTypes.Immutable(2) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
            Assert.AreSame(source[0], target[0]);
            Assert.AreSame(source[1], target[1]);
        }

        [Test]
        public void ListOfIntsToLonger()
        {
            var source = new List<int> { 1, 2, 3 };
            var target = new List<int> { 1, 2, 3, 4 };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source, target);
        }

        [Test]
        public void ListOfWithSimplesToEmpty()
        {
            var source = new List<CopyTypes.WithSimpleProperties> { new CopyTypes.WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var target = new List<CopyTypes.WithSimpleProperties>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new CopyTypes.WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            CollectionAssert.AreEqual(expected, source, CopyTypes.WithSimpleProperties.Comparer);
            CollectionAssert.AreEqual(expected, target, CopyTypes.WithSimpleProperties.Comparer);
        }

        [Test]
        public void ListOfComplexToLonger()
        {
            var source = new List<CopyTypes.ComplexType> { new CopyTypes.ComplexType("a", 1) };
            var target = new List<CopyTypes.ComplexType> { new CopyTypes.ComplexType("b", 2), new CopyTypes.ComplexType("c", 3) };
            var item = target[0];
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, CopyTypes.ComplexType.Comparer);
            Assert.AreSame(item, target[0]);
            Assert.AreNotSame(source[0], target[0]);
        }

        [Test]
        public void ObservableCollectionOfIntsToEmpty()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var target = new ObservableCollection<int>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source, target);
        }

        [Test]
        public void ObservableCollectionOfIntsToLonger()
        {
            var source = new ObservableCollection<int> { 1, 2, 3 };
            var target = new ObservableCollection<int> { 1, 2, 3, 4 };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source, target);
        }

        [Test]
        public void ObservableCollectionOfComplexType()
        {
            var source = new ObservableCollection<CopyTypes.ComplexType> { new CopyTypes.ComplexType("a", 1) };
            var target = new ObservableCollection<CopyTypes.ComplexType> { new CopyTypes.ComplexType("b", 2), new CopyTypes.ComplexType("c", 3) };
            var item = target[0];
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, CopyTypes.ComplexType.Comparer);
            Assert.AreSame(item, target[0]);
            Assert.AreNotSame(source[0], target[0]);
        }

        [Test]
        public void ArrayOfInts()
        {
            var source = new[] { 1, 2, 3 };
            var target = new[] { 4, 5, 6 };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { 1, 2, 3 };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfImmutable()
        {
            var source = new[] { new CopyTypes.Immutable(1), new CopyTypes.Immutable(2), new CopyTypes.Immutable(3) };
            var target = new[] { new CopyTypes.Immutable(4), new CopyTypes.Immutable(5), new CopyTypes.Immutable(6) };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new CopyTypes.Immutable(1), new CopyTypes.Immutable(2), new CopyTypes.Immutable(3) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void DictionaryToSameLength()
        {
            var source = new Dictionary<int, string> { { 1, "one" } };
            var target = new Dictionary<int, string> { { 1, "ett" } };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new KeyValuePair<int, string>(1, "one"), };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var source = new Dictionary<int, string> { { 1, "one" } };
            var target = new Dictionary<int, string>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new KeyValuePair<int, string>(1, "one"), };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void DictionaryToLonger()
        {
            var source = new Dictionary<int, string> { { 1, "one" } };
            var target = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new KeyValuePair<int, string>(1, "one") };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void HashSetOfInts()
        {
            Assert.Inconclusive("Reminder");
        }
    }
}