namespace Gu.State.Tests.EqualByTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class SetTests
    {
        public abstract bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase(ReferenceHandling.Structural)]
        public void ComplexWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.ByNameComparer) { new("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.ByNameComparer) { new("a", 1) };
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(true, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ComplexWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new HashSet<ComplexType>(ComplexType.ByNameComparer) { new("a", 1) };
            var y = new HashSet<ComplexType>(ComplexType.ByNameComparer) { new("a", 2) };
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithCollisionsWhenEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType { Value = 1 };
            var e2 = new HashCollisionType { Value = 2 };
            var x = new HashSet<HashCollisionType> { e1, e2 };
            var y = new HashSet<HashCollisionType> { e2, e1 };
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(true, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithCollisionsWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var e1 = new HashCollisionType();
            var x = new HashSet<HashCollisionType> { e1, new() { Value = 1 } };
            var y = new HashSet<HashCollisionType> { e1, new() { Value = 2 } };
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableHashSetOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableHashSet.Create(1, 2, 3);
            Assert.AreEqual(true, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualBy(y, x, referenceHandling));
        }
    }
}
