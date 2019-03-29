namespace Gu.State.Tests.DiffTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public abstract class DictionaryTests
    {
        public abstract Diff DiffBy<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase(1, "one", "Empty")]
        [TestCase(2, "one", "Dictionary<int, string> [1] x: missing item y: one [2] x: one y: missing item")]
        [TestCase(1, "two", "Dictionary<int, string> [1] x: two y: one")]
        public void DictionaryToSameLength(int key, string value, string expected)
        {
            var x = new Dictionary<int, string> { { key, value } };
            var y = new Dictionary<int, string> { { 1, "one" } };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DictionaryWithCollisionsWhenEqual()
        {
            var k1 = new DiffTypes.HashCollisionType();
            var k2 = new DiffTypes.HashCollisionType();
            var x = new Dictionary<DiffTypes.HashCollisionType, string> { { k1, "1" }, { k2, "2" } };
            var y = new Dictionary<DiffTypes.HashCollisionType, string> { { k1, "1" }, { k2, "2" } };
            Assert.AreEqual(2, x.Count);
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual("Empty", actual);

            y = new Dictionary<DiffTypes.HashCollisionType, string> { { k2, "2" }, { k1, "1" } };
            result = this.DiffBy(x, y, ReferenceHandling.Structural);
            actual = result.ToString(string.Empty, " ");
            Assert.AreEqual("Empty", actual);
        }

        [TestCase(1, "one", "Empty")]
        [TestCase(2, "one", "Dictionary<int, ComplexType> [1] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType [2] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: missing item")]
        [TestCase(1, "two", "Dictionary<int, ComplexType> [1] <member> x: two y: one")]
        public void DictionaryIntComplex(int key, string value, string expected)
        {
            expected = this is FieldValues.Dictionary
                           ? expected?.Replace("<member>", "name")
                           : expected?.Replace("<member>", "Name");
            var x = new Dictionary<int, DiffTypes.ComplexType> { { key, new DiffTypes.ComplexType(value, 1) } };
            var y = new Dictionary<int, DiffTypes.ComplexType> { { 1, new DiffTypes.ComplexType("one", 1) } };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string>();
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var expected = "Dictionary<int, string> [1] x: one y: missing item";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            expected = "Dictionary<int, string> [1] x: missing item y: one";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void DictionaryToLonger()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            var expected = "Dictionary<int, string> [2] x: missing item y: two";
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);

            result = this.DiffBy(y, x, ReferenceHandling.Structural);
            expected = "Dictionary<int, string> [2] x: two y: missing item";
            actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableDictionaryOfIntsAndStringsWhenEqual(ReferenceHandling referenceHandling)
        {
            var builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, string>();
            builder.Add(1, "one");
            builder.Add(2, "two");
            var x = builder.ToImmutable();
            var y = builder.ToImmutable();
            Assert.AreEqual("Empty", this.DiffBy(x, y, referenceHandling).ToString(string.Empty, " "));
            Assert.AreEqual("Empty", this.DiffBy(y, x, referenceHandling).ToString(string.Empty, " "));
        }
    }
}