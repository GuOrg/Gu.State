namespace Gu.State.Tests.DiffTests
{
    using System.Collections.Generic;

    using NUnit.Framework;
    using static DiffTypes;

    public abstract class SetTests
    {
        public abstract Diff DiffMethod<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3 };
            var y = new HashSet<int> { 2, 3, 1 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 30 };
            var y = new HashSet<int> { 1, 2, 40 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("HashSet<int> [30] x: 30 y: missing item [40] x: missing item y: 40", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("HashSet<int> [40] x: 40 y: missing item [30] x: missing item y: 30", result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenLonger(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3, 40 };
            var y = new HashSet<int> { 1, 2, 3 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("HashSet<int> [40] x: 40 y: missing item", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("HashSet<int> [40] x: missing item y: 40", result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            var result = this.DiffMethod(x, y, referenceHandling);
            StringAssert.AreEqualIgnoringCase("HashSet<ComplexType> [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] Value x: 1 y: 2", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            StringAssert.AreEqualIgnoringCase("HashSet<ComplexType> [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] Value x: 2 y: 1", result.ToString("", " "));
        }

        [Test]
        public void HashSetOfComplexWhenNotEqualReferences()
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            var result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual("HashSet<ComplexType> [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType", result.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.References);
            Assert.AreEqual("HashSet<ComplexType> [Gu.State.Tests.DiffTests.DiffTypes+ComplexType] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType", result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfWithCollisionsWhenEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var e2 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, e2 };
            var y = new HashSet<HashCollisionType> { e2, e1 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfWithCollisionsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, new HashCollisionType { Value = 1 } };
            var y = new HashSet<HashCollisionType> { e1, new HashCollisionType { Value = 2 } };
            var result = this.DiffMethod(x, y, referenceHandling);
            var expected = "HashSet<HashCollisionType> [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] Value x: 1 y: 2";
            var actual = result.ToString("", " ");
            StringAssert.AreEqualIgnoringCase(expected, actual);

            expected = "HashSet<HashCollisionType> [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] Value x: 2 y: 1";
            result = this.DiffMethod(y, x, referenceHandling);
            actual = result.ToString("", " ");
            StringAssert.AreEqualIgnoringCase(expected, actual);
        }

        [Test]
        public void HashSetOfWithCollisionsWhenNotEqualReference()
        {
            var e1 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, new HashCollisionType() };
            var y = new HashSet<HashCollisionType> { e1, new HashCollisionType() };
            var result = this.DiffMethod(x, y, ReferenceHandling.References);
            var expected = "HashSet<HashCollisionType> [Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType] x: Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType y: Gu.State.Tests.DiffTests.DiffTypes+HashCollisionType";
            var actual = result.ToString("", " ");
            Assert.AreEqual(expected, actual);

            result = this.DiffMethod(y, x, ReferenceHandling.References);
            actual = result.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }
    }
}