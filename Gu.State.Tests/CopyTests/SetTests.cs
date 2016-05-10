namespace Gu.State.Tests.CopyTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static CopyTypes;

    public abstract class SetTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
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

        [TestCase(ReferenceHandling.Throw)]
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
        public void HashSetOfComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var source = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var target = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { new ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
        }

        [TestCase(ReferenceHandling.Structural)]
        public void HashSetOfComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var source = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var target = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { new ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
        }

        [Test]
        public void HashSetOfComplexWhenNotEqualReferences()
        {
            var sv = new ComplexType("a", 1);
            var source = new HashSet<ComplexType>(ComplexType.NameComparer) { sv };
            var target = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            this.CopyMethod(source, target, ReferenceHandling.References);
            var expected = new[] { sv };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void HashSetOfComplexWhenEqualReferences()
        {
            var sv = new ComplexType("a", 1);
            var source = new HashSet<ComplexType>(ComplexType.NameComparer) { sv };
            var target = new HashSet<ComplexType>(ComplexType.NameComparer) { sv };
            this.CopyMethod(source, target, ReferenceHandling.References);
            var expected = new[] { sv };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }
    }
}