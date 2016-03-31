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
        public abstract Diff DiffMethod<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", null)]
        [TestCase("1, 2, 3", "1, 2", "int[] [2] x: 3 y: missing item")]
        [TestCase("1, 2", "1, 2, 3", "int[] [2] x: missing item y: 3")]
        [TestCase("5, 2, 3", "1, 2, 3", "int[] [0] x: 5 y: 1")]
        public void ArrayOfIntsStructural(string xs, string ys, string expected)
        {
            var x = xs.Split(',').Select(int.Parse).ToArray();
            var y = ys.Split(',').Select(int.Parse).ToArray();
            var result = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void ListOfIntsToEmpty()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "List<int> [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "List<int> [0] x: missing item y: 1 [1] x: missing item y: 2 [2] x: missing item y: 3";
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void ListOfIntsEmptyToEmpty()
        {
            var x = new List<int>();
            var y = new List<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void ListOfIntsToLonger()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int> { 1, 2, 3, 4 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "List<int> [3] x: missing item y: 4";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "List<int> [3] x: 4 y: missing item";
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void ListOfWithSimples()
        {
            var x = new List<WithSimpleProperties> { new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var y = new List<WithSimpleProperties> { new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            var expected = "List<ComplexType> [0] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType [1] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void ListOfComplexSameItems()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType>(x);
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void ObservableCollectionOfIntsToEmpty()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "ObservableCollection<int> [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "ObservableCollection<int> [0] x: missing item y: 1 [1] x: missing item y: 2 [2] x: missing item y: 3";
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void ObservableCollectionOfIntsToLonger()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int> { 1, 2, 3, 4 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "ObservableCollection<int> [3] x: missing item y: 4";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "ObservableCollection<int> [3] x: 4 y: missing item";
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void ObservableCollectionOfComplexType()
        {
            var x = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);
        }

        [Test]
        public void ArrayOfInts()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(null, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfEqualImmutable(ReferenceHandling referenceHandling)
        {
            var source = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var target = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var diff = this.DiffMethod(source, target, referenceHandling);
            Assert.IsNull(diff);
        }

        [TestCase(1, "one", null)]
        [TestCase(2, "one", "Dictionary<int, string> [2] x: one y: missing item [1] x: missing item y: one")]
        [TestCase(1, "two", "Dictionary<int, string> [1] x: two y: one")]
        public void DictionaryToSameLength(int key, string value, string expected)
        {
            var x = new Dictionary<int, string> { { key, value } };
            var y = new Dictionary<int, string> { { 1, "one" } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var actual = result?.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, "one", null)]
        [TestCase(2, "one", "Dictionary<int, ComplexType> [2] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: missing item [1] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType")]
        [TestCase(1, "two", "Dictionary<int, ComplexType> [1] <member> x: two y: one")]
        public void DictionaryIntComplex(int key, string value, string expected)
        {
            expected = expected?.Replace("<member>", this is FieldValues.Collections ? "name" : "Name");
            var x = new Dictionary<int, ComplexType> { { key, new ComplexType(value, 1) } };
            var y = new Dictionary<int, ComplexType> { { 1, new ComplexType("one", 1) } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var actual = result?.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "Dictionary<int, string> [1] x: one y: missing item";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "Dictionary<int, string> [1] x: missing item y: one";
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void DictionaryToLonger()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "Dictionary<int, string> [2] x: missing item y: two";
            var actual = result?.ToString("", " ");
            Assert.AreEqual(expected, actual);

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "Dictionary<int, string> [2] x: two y: missing item";
            actual = result?.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void HashSetOfInts()
        {
            Assert.Inconclusive("Reminder");
        }

        [TestCase("1, 2, 3", "1, 2, 3", null)]
        [TestCase("1, 2, 3", "1, 2", "WhereSelectArrayIterator<string, int> [2] x: 3 y: missing item")]
        [TestCase("1, 2", "1, 2, 3", "WhereSelectArrayIterator<string, int> [2] x: missing item y: 3")]
        [TestCase("5, 2, 3", "1, 2, 3", "WhereSelectArrayIterator<string, int> [0] x: 5 y: 1")]
        public void EnumarebleStructural(string xs, string ys, string expected)
        {
            var x = xs.Split(',').Select(int.Parse);
            var y = ys.Split(',').Select(int.Parse);
            var diff = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            var actual = diff?.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0, 0, 0, 0, null)]
        [TestCase(0, 1, 0, 1, null)]
        [TestCase(1, 1, 1, 1, null)]
        [TestCase(0, 2, 0, 1, "<RepeatIterator>d__112<int> [1] x: 0 y: missing item")]
        [TestCase(0, 1, 0, 2, "<RepeatIterator>d__112<int> [1] x: missing item y: 0")]
        [TestCase(1, 1, 0, 1, "<RepeatIterator>d__112<int> [0] x: 1 y: 0")]
        [TestCase(0, 1, 1, 1, "<RepeatIterator>d__112<int> [0] x: 0 y: 1")]
        public void EnumarebleRepeatStructural(int startX, int countX, int startY, int countY, string expected)
        {
            var x = Enumerable.Repeat(startX, countX);
            var y = Enumerable.Repeat(startY, countY);
            var diff = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            Assert.AreEqual(expected, diff?.ToString("", " "));
        }

        [Test]
        public void EnumarebleNullsStructural()
        {
            var x = new object[] { 1, null }.Select(z => z);
            var y = new object[] { 1, null }.Select(z => z);
            var diff = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            Assert.AreEqual(null, diff);

            x = new object[] { 1 }.Select(z => z);
            y = new object[] { 1, null }.Select(z => z);
            diff = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            var expected = "WhereSelectArrayIterator<Object, Object> [1] x: missing item y: null";
            var actual = diff?.ToString("", " ");
            Assert.AreEqual(expected, actual);

            x = new object[] { 1, null }.Select(z => z);
            y = new object[] { 1 }.Select(z => z);
            diff = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            expected = "WhereSelectArrayIterator<Object, Object> [1] x: null y: missing item";
            actual = diff?.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }
    }
}