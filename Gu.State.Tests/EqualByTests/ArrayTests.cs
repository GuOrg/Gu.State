namespace Gu.State.Tests.EqualByTests
{
    using NUnit.Framework;

    public abstract class ArrayTests
    {
        public abstract bool EqualBy<T>(T source, T target, ReferenceHandling referenceHandling);

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableListOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableList.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableList.Create(1, 2, 3);
            Assert.AreEqual(true, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualBy(y, x, referenceHandling));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableArrayOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
            Assert.AreEqual(true, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualBy(y, x, referenceHandling));
        }
    }
}