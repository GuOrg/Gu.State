namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static DiffTypes;

    public abstract class CollectionTests
    {
        public abstract Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling)
            where T : class;

        [Test]
        public void ListOfIntToEmpty()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int>();
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var expected = "List<int> [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            expected = "List<int> [0] x: missing item y: 1 [1] x: missing item y: 2 [2] x: missing item y: 3";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void ListOfIntEmptyToEmpty()
        {
            var x = new List<int>();
            var y = new List<int>();
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ListOfIntToLonger()
        {
            var x = new List<int> { 1, 2, 3 };
            var y = new List<int> { 1, 2, 3, 4 };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var expected = "List<int> [3] x: missing item y: 4";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            expected = "List<int> [3] x: 4 y: missing item";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void ListOfWithSimpleProperties()
        {
            var x = new List<WithSimpleProperties>
            {
                new WithSimpleProperties(
                    1,
                    2,
                    "a",
                    StringSplitOptions.RemoveEmptyEntries),
            };
            var y = new List<WithSimpleProperties>
            {
                new WithSimpleProperties(
                    1,
                    2,
                    "a",
                    StringSplitOptions.RemoveEmptyEntries),
            };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ListOfComplex()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(x, y, ReferenceHandling.References);
            var expected = "List<ComplexType> [0] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType [1] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ListOfComplexSameItems()
        {
            var x = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new List<ComplexType>(x);
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void ObservableCollectionOfIntToEmpty()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int>();
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var expected =
                "ObservableCollection<int> [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            expected =
                "ObservableCollection<int> [0] x: missing item y: 1 [1] x: missing item y: 2 [2] x: missing item y: 3";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void ObservableCollectionOfIntToLonger()
        {
            var x = new ObservableCollection<int> { 1, 2, 3 };
            var y = new ObservableCollection<int> { 1, 2, 3, 4 };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            var expected = "ObservableCollection<int> [3] x: missing item y: 4";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            expected = "ObservableCollection<int> [3] x: 4 y: missing item";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void ObservableCollectionOfComplexType()
        {
            var x = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var y = new ObservableCollection<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableListOfIntWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableList.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableList.Create(1, 2, 3);
            var result = this.DiffBy(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }
    }
}
