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
        public abstract Diff DiffMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", "Empty")]
        [TestCase("1, 2, 3", "1, 2", "int[] [2] x: 3 y: missing item")]
        [TestCase("1, 2", "1, 2, 3", "int[] [2] x: missing item y: 3")]
        [TestCase("5, 2, 3", "1, 2, 3", "int[] [0] x: 5 y: 1")]
        public void ArrayOfIntsStructural(string xs, string ys, string expected)
        {
            var x = xs.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var y = ys.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var result = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString("", " "));
        }

        [Test]
        public void ListOfIntsToEmpty()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = "List<int> [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";
            Assert.AreEqual(expected, result.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected = "List<int> [0] x: missing item y: 1 [1] x: missing item y: 2 [2] x: missing item y: 3";
            Assert.AreEqual(expected, result.ToString("", " "));
        }

        [Test]
        public void ListOfIntsEmptyToEmpty()
        {
            var x = new List<int>();
            var y = new List<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
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
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            var expected = "List<ComplexType> [0] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType [1] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ListOfComplexSameItems()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType>(x);
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ObservableCollectionOfIntsToEmpty()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int>();
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected =
                "ObservableCollection<int> [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            expected =
                "ObservableCollection<int> [0] x: missing item y: 1 [1] x: missing item y: 2 [2] x: missing item y: 3";
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
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ArrayOfInts()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfEqualImmutable(ReferenceHandling referenceHandling)
        {
            var source = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var target = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var diff = this.DiffMethod(source, target, referenceHandling);
            Assert.AreEqual("Empty", diff.ToString());
        }
    }
}