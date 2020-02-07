namespace Gu.State.Tests.DiffTests
{
    using System.Linq;

    using NUnit.Framework;

    public abstract class EnumerableTests
    {
        public abstract Diff DiffBy<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase("1, 2, 3", "1, 2, 3", "Empty")]
        [TestCase("1, 2, 3", "1, 2", "WhereSelectArrayIterator<string, int> [Skip(2)] x: 3 y: missing item")]
        [TestCase("1, 2", "1, 2, 3", "WhereSelectArrayIterator<string, int> [Skip(2)] x: missing item y: 3")]
        [TestCase("5, 2, 3", "1, 2, 3", "WhereSelectArrayIterator<string, int> [Skip(0)] x: 5 y: 1")]
        public void EnumerableStructural(string xs, string ys, string expected)
        {
            var x = xs.Split(',').Select(int.Parse);
            var y = ys.Split(',').Select(int.Parse);
            var diff = this.DiffBy(x, y, ReferenceHandling.Structural);
            var actual = diff.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(0, 0, 0, 0, "Empty")]
        [TestCase(0, 1, 0, 1, "Empty")]
        [TestCase(1, 1, 1, 1, "Empty")]
        [TestCase(0, 2, 0, 1, "<RepeatIterator>d__11\\d<int> \\[Skip\\(1\\)\\] x: 0 y: missing item")]
        [TestCase(0, 1, 0, 2, "<RepeatIterator>d__11\\d<int> \\[Skip\\(1\\)\\] x: missing item y: 0")]
        [TestCase(1, 1, 0, 1, "<RepeatIterator>d__11\\d<int> \\[Skip\\(0\\)\\] x: 1 y: 0")]
        [TestCase(0, 1, 1, 1, "<RepeatIterator>d__11\\d<int> \\[Skip\\(0\\)\\] x: 0 y: 1")]
        public void EnumerableRepeatStructural(int startX, int countX, int startY, int countY, string expected)
        {
            var x = Enumerable.Repeat(startX, countX);
            var y = Enumerable.Repeat(startY, countY);
            var diff = this.DiffBy(x, y, ReferenceHandling.Structural);
            StringAssert.IsMatch(expected, diff.ToString(string.Empty, " "));
        }

        [Test]
        public void EnumerableNullsStructural()
        {
            var x = new object[] { 1, null }.Select(z => z);
            var y = new object[] { 1, null }.Select(z => z);
            var diff = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual("Empty", diff.ToString());

            x = new object[] { 1 }.Select(z => z);
            y = new object[] { 1, null }.Select(z => z);
            diff = this.DiffBy(x, y, ReferenceHandling.Structural);
            var expected = "WhereSelectArrayIterator<Object, Object> [Skip(1)] x: missing item y: null";
            var actual = diff.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);

            x = new object[] { 1, null }.Select(z => z);
            y = new object[] { 1 }.Select(z => z);
            diff = this.DiffBy(x, y, ReferenceHandling.Structural);
            expected = "WhereSelectArrayIterator<Object, Object> [Skip(1)] x: null y: missing item";
            actual = diff.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }
    }
}