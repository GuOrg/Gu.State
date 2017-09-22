namespace Gu.State.Tests.DiffTests
{
    using System.Collections.Generic;

    using NUnit.Framework;
    using static DiffTypes;

    public abstract class SetTests
    {
        public abstract Diff DiffMethod<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3 };
            var y = new HashSet<int> { 2, 3, 1 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 30 };
            var y = new HashSet<int> { 1, 2, 40 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual("HashSet<int> [30] x: 30 y: missing item [40] x: missing item y: 40", result.ToString(string.Empty, " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual("HashSet<int> [30] x: missing item y: 30 [40] x: 40 y: missing item", result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenLonger(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3, 40 };
            var y = new HashSet<int> { 1, 2, 3 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual("HashSet<int> [40] x: 40 y: missing item", result.ToString(string.Empty, " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual("HashSet<int> [40] x: missing item y: 40", result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        public void HashSetOfComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural)]
        public void HashSetOfComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            var expected = "HashSet<ComplexType> [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: missing item [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";

            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            var actual = result.ToString(string.Empty, " ");
            StringAssert.AreEqualIgnoringCase(expected, actual);

            result = this.DiffMethod(y, x, referenceHandling);
            actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(false, result.IsEmpty);
            StringAssert.AreEqualIgnoringCase(expected, actual);
        }

        [Test]
        public void HashSetOfComplexWhenNotEqualReferences()
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            var expected = "HashSet<ComplexType> [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: missing item [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";

            var result = this.DiffMethod(x, y, ReferenceHandling.References);
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual(expected, actual);

            result = this.DiffMethod(y, x, ReferenceHandling.References);
            Assert.AreEqual(false, result.IsEmpty);
            actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfWithCollisionsWhenEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var e2 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, e2 };
            var y = new HashSet<HashCollisionType> { e2, e1 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural)]
        public void HashSetOfWithCollisionsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, new HashCollisionType { Value = 1 } };
            var y = new HashSet<HashCollisionType> { e1, new HashCollisionType { Value = 2 } };
            var expected = "HashSet<HashCollisionType> [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] x: Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType y: missing item [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType";

            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            var actual = result.ToString(string.Empty, " ");
            StringAssert.AreEqualIgnoringCase(expected, actual);

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            actual = result.ToString(string.Empty, " ");
            StringAssert.AreEqualIgnoringCase(expected, actual);
        }

        [Test]
        public void HashSetOfWithCollisionsWhenNotEqualReference()
        {
            var e1 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, new HashCollisionType() };
            var y = new HashSet<HashCollisionType> { e1, new HashCollisionType() };
            var expected = "HashSet<HashCollisionType> [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] x: Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType y: missing item [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType";

            var result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result.IsEmpty);
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);

            result = this.DiffMethod(y, x, ReferenceHandling.References);
            Assert.AreEqual(false, result.IsEmpty);
            actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableHashSetOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3);
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }
    }
}