namespace Gu.State.Tests.CopyTests
{
    using NUnit.Framework;

    using static CopyTypes;

    public abstract class ArrayTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [Test]
        public void Ints()
        {
            var source = new[] { 1, 2, 3 };
            var target = new[] { 4, 5, 6 };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { 1, 2, 3 };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void Immutables(ReferenceHandling referenceHandling)
        {
            var source = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var target = new[] { new Immutable(4), new Immutable(5), new Immutable(6) };
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void Ints2D(ReferenceHandling referenceHandling)
        {
            var source = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var target = new int[3, 2];
            this.CopyMethod(source, target, referenceHandling);
            var expected = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }
    }
}