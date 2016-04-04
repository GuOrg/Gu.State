namespace Gu.State.Tests.EqualByTests
{
    using System.Linq;

    using NUnit.Framework;

    public abstract class EnumerableTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", true)]
        [TestCase("1, 2, 3", "1, 2", false)]
        [TestCase("1, 2", "1, 2, 3", false)]
        [TestCase("5, 2, 3", "1, 2, 3", false)]
        public void EnumarebleStructural(string xs, string ys, bool expected)
        {
            var x = xs.Split(',').Select(int.Parse);
            var y = ys.Split(',').Select(int.Parse);
            Assert.AreEqual(expected, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [TestCase(0, 0, 0, 0, true)]
        [TestCase(0, 1, 0, 1, true)]
        [TestCase(1, 1, 1, 1, true)]
        [TestCase(0, 2, 0, 1, false)]
        [TestCase(0, 1, 0, 2, false)]
        [TestCase(1, 1, 0, 1, false)]
        [TestCase(0, 1, 1, 1, false)]
        public void EnumarebleRepeatStructural(int startX, int countX, int startY, int countY, bool expected)
        {
            var x = Enumerable.Repeat(startX, countX);
            var y = Enumerable.Repeat(startY, countY);
            Assert.AreEqual(expected, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }

        [Test]
        public void EnumarebleNullsStructural()
        {
            var x = new object[] { 1, null }.Select(z => z);
            var y = new object[] { 1, null }.Select(z => z);
            Assert.AreEqual(true, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));

            x = new object[] { 1 }.Select(z => z);
            y = new object[] { 1, null }.Select(z => z);
            Assert.AreEqual(false, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));

            x = new object[] { 1, null }.Select(z => z);
            y = new object[] { 1 }.Select(z => z);
            Assert.AreEqual(false, this.EqualByMethod(x, y, referenceHandling: ReferenceHandling.Structural));
        }
    }
}