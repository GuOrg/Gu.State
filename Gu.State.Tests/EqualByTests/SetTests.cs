namespace Gu.State.Tests.EqualByTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class SetTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void IntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3 };
            var y = new HashSet<int> { 2, 3, 1 };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void IntsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3 };
            var y = new HashSet<int> { 1, 2, 4 };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void IntsWhenLonger(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<int> { 1, 2, 3, 4 };
            var y = new HashSet<int> { 1, 2, 3 };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void ComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void ComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.NameComparer) { new ComplexType("a", 2) };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void WithCollisionsWhenEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var e2 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, e2 };
            var y = new HashSet<HashCollisionType> { e2, e1 };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void WithCollisionsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, new HashCollisionType { Value = 1 } };
            var y = new HashSet<HashCollisionType> { e1, new HashCollisionType { Value = 2 } };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        public void ImmutableHashSetOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3);
            Assert.AreEqual(true, this.EqualByMethod(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualByMethod(y, x, referenceHandling));
        }
    }
}