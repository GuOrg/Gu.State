namespace Gu.State.Tests.EqualByTests
{
    using System.Linq;

    using NUnit.Framework;

    public abstract class ArrayTests
    {
        public abstract bool EqualBy<T>(T source, T target, ReferenceHandling referenceHandling);

        [TestCase("1, 2, 3", "1, 2, 3", true)]
        [TestCase("1, 2, 3", "1, 2", false)]
        [TestCase("1, 2", "1, 2, 3", false)]
        [TestCase("5, 2, 3", "1, 2, 3", false)]
        public void ArrayOfIntsStructural(string xs, string ys, bool expected)
        {
            var x = xs.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var y = ys.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            Assert.AreEqual(expected, this.EqualBy(x, y, ReferenceHandling.Structural));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void IntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(true, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(5, true)]
        [TestCase(10, false)]
        public void JaggedArray2DOfInts(int value, bool expected)
        {
            var x = new[] { new[] { 1, 2, 3 }, new[] { 4, 5 } };
            var y = new[] { new[] { 1, 2, 3 }, new[] { 4, value } };
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [TestCase(0, 0, 1, true)]
        [TestCase(0, 0, 10, false)]
        [TestCase(2, 1, 10, false)]
        public void Array2DOfInts(int i1, int i2, int yValue, bool expected)
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            y[i1, i2] = yValue;
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfImmutableWhenNotEqual(ReferenceHandling referenceHandling)
        {
            var x = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            var y = new[] { new EqualByTypes.Immutable(4), new EqualByTypes.Immutable(5), new EqualByTypes.Immutable(6) };
            Assert.AreEqual(false, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(false, this.EqualBy(y, x, referenceHandling));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfImmutableWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            var y = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            Assert.AreEqual(true, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualBy(y, x, referenceHandling));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableListOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
            Assert.AreEqual(true, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualBy(y, x, referenceHandling));
        }
    }
}