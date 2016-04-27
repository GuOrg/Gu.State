namespace Gu.State.Tests.DiffTests
{
    using System.Linq;

    using NUnit.Framework;

    public abstract class ArrayTests
    {
        public abstract Diff DiffMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", "Empty")]
        [TestCase("1, 2, 3", "1, 2", "int[] [2] x: 3 y: missing item")]
        [TestCase("1, 2", "1, 2, 3", "int[] [2] x: missing item y: 3")]
        [TestCase("5, 2, 3", "1, 2, 3", "int[] [0] x: 5 y: 1")]
        public void ArrayOfIntsStructural(string xs, string ys, string expected)
        {
            var x = xs.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var y = ys.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var result = this.DiffMethod(x, y, referenceHandling: ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString("", " "));
        }

        [Test]
        public void ArrayOfInts()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(0, 0, 1, "Empty")]
        [TestCase(0, 0, 10, "")]
        [TestCase(2, 1, 10, "")]
        public void Array2DOfInts(int i1, int i2, int yValue, string expected)
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            y[i1, i2] = yValue;
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString(""," "));

            result = this.DiffMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString("", " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfEqualImmutable(ReferenceHandling referenceHandling)
        {
            var source = new[] { new DiffTypes.Immutable(1), new DiffTypes.Immutable(2), new DiffTypes.Immutable(3) };
            var target = new[] { new DiffTypes.Immutable(1), new DiffTypes.Immutable(2), new DiffTypes.Immutable(3) };
            var diff = this.DiffMethod(source, target, referenceHandling);
            Assert.AreEqual("Empty", diff.ToString());
        }
    }
}