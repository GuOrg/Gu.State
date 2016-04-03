namespace Gu.State.Tests.DiffTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

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
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(null, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3 };
            var y = new HashSet<int> { 1, 2, 4 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("HashSet<int> [2] x: 3 y: 4", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("HashSet<int> [2] x: 4 y: 3", result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenLonger(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3, 4 };
            var y = new HashSet<int> { 1, 2, 3 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("HashSet<int> [3] x: 4 y: missing item", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("HashSet<int> [3] x: missing item y: 4", result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<DiffTypes.ComplexType>(DiffTypes.ComplexType.NameComparer) { new DiffTypes.ComplexType("a", 1) };
            var y = new HashSet<DiffTypes.ComplexType>(DiffTypes.ComplexType.NameComparer) { new DiffTypes.ComplexType("a", 1) };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(null, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<DiffTypes.ComplexType>(DiffTypes.ComplexType.NameComparer) { new DiffTypes.ComplexType("a", 1) };
            var y = new HashSet<DiffTypes.ComplexType>(DiffTypes.ComplexType.NameComparer) { new DiffTypes.ComplexType("a", 2) };
            var result = this.DiffMethod(x, y, referenceHandling);
            StringAssert.AreEqualIgnoringCase("HashSet<ComplexType> [0] Value x: 1 y: 2", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            StringAssert.AreEqualIgnoringCase("HashSet<ComplexType> [0] Value x: 2 y: 1", result.ToString("", " "));
        }

        [Test]
        public void HashSetOfComplexWhenNotEqualReferences()
        {
            var x = new HashSet<DiffTypes.ComplexType>(DiffTypes.ComplexType.NameComparer) { new DiffTypes.ComplexType("a", 1) };
            var y = new HashSet<DiffTypes.ComplexType>(DiffTypes.ComplexType.NameComparer) { new DiffTypes.ComplexType("a", 2) };
            var result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual("HashSet<ComplexType> [0] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType", result.ToString("", " "));

            result = this.DiffMethod(y, x, ReferenceHandling.References);
            Assert.AreEqual("HashSet<ComplexType> [0] x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType", result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfWithCollisionsWhenEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new DiffTypes.HashCollisionType();
            var e2 = new DiffTypes.HashCollisionType();
            var x = new HashSet<DiffTypes.HashCollisionType> { e1, e2 };
            var y = new HashSet<DiffTypes.HashCollisionType> { e2, e1 };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(null, result);

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual(null, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfWithCollisionsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new DiffTypes.HashCollisionType();
            var x = new HashSet<DiffTypes.HashCollisionType> { e1, new DiffTypes.HashCollisionType() };
            var y = new HashSet<DiffTypes.HashCollisionType> { e1, new DiffTypes.HashCollisionType() };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual("", result.ToString("", " "));

            result = this.DiffMethod(y, x, referenceHandling);
            Assert.AreEqual("", result.ToString("", " "));
        }
    }
}