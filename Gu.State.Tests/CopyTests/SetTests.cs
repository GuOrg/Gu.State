namespace Gu.State.Tests.CopyTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public abstract class SetTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var expected = new[] { 1, 2, 3 };
            var source = new HashSet<int> { 1, 2, 3 };
            CollectionAssert.AreEqual(expected, source);
            var target = new HashSet<int> { 2, 3, 1 };
            this.CopyMethod(source, target, referenceHandling);
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEquivalent(expected, target);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var source = new HashSet<int> { 1, 2, 3 };
            var target = new HashSet<int> { 1, 2, 4 };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { 1, 2, 3 };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void HashSetOfIntsWhenLonger(ReferenceHandling referenceHandling)
        {
            var source = new HashSet<int> { 1, 2, 3, 4 };
            var target = new HashSet<int> { 1, 2, 3 };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { 1, 2, 3, 4 };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var source = new HashSet<CopyTypes.ComplexType>(CopyTypes.ComplexType.NameComparer) { new CopyTypes.ComplexType("a", 1) };
            var target = new HashSet<CopyTypes.ComplexType>(CopyTypes.ComplexType.NameComparer) { new CopyTypes.ComplexType("a", 1) };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, CopyTypes.ComplexType.Comparer);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void HashSetOfComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var source = new HashSet<CopyTypes.ComplexType>(CopyTypes.ComplexType.NameComparer) { new CopyTypes.ComplexType("a", 1) };
            var target = new HashSet<CopyTypes.ComplexType>(CopyTypes.ComplexType.NameComparer) { new CopyTypes.ComplexType("a", 2) };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, CopyTypes.ComplexType.Comparer);
        }

        [Test]
        public void HashSetOfComplexWhenNotEqualReferences()
        {
            var sv = new CopyTypes.ComplexType("a", 1);
            var source = new HashSet<CopyTypes.ComplexType>(CopyTypes.ComplexType.NameComparer) { sv };
            var target = new HashSet<CopyTypes.ComplexType>(CopyTypes.ComplexType.NameComparer) { new CopyTypes.ComplexType("a", 2) };
            this.CopyMethod(source, target, ReferenceHandling.References);
            var expected = new[] { sv };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }
    }
}