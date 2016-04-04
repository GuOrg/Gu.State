namespace Gu.State.Tests.EqualByTests
{
    using System.Linq;

    using NUnit.Framework;

    public abstract class ArrayTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

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
            Assert.AreEqual(expected, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [Test]
        public void ArrayOfInts()
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void JaggedArray2DOfInts()
        {
            Assert.Fail();
            //var x = new int[2][3];
            //var y = new[] { 1, 2, 3 };
            //var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            //Assert.AreEqual(true, result);

            //result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            //Assert.AreEqual(true, result);
        }

        [TestCase(3, true)]
        [TestCase(10, false)]
        public void Array2DOfInts(int yValue, bool expected)
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2 }, { yValue, 4 }, { 5, 6 } };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ArrayOfImmutable()
        {
            var source = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            var target = new[] { new EqualByTypes.Immutable(4), new EqualByTypes.Immutable(5), new EqualByTypes.Immutable(6) };
            this.EqualByMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new EqualByTypes.Immutable(1), new EqualByTypes.Immutable(2), new EqualByTypes.Immutable(3) };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }
    }
}