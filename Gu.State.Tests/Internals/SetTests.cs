namespace Gu.State.Tests.Internals
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    public class SetTests
    {
        [Test]
        public void SortSet()
        {
            var set = new HashSet<int> { 3, 4, 1, 2 };
            var sorted = new Gu.State.Set.SortedByHashCode<int>(set, set.Comparer);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, sorted);
            Assert.AreEqual(false, sorted.HasCollision);
        }

        [TestCase("1", "1", false)]
        [TestCase("1,2", "1,2", false)]
        [TestCase("2,1", "1,2", false)]
        [TestCase("1,1", "1,1", true)]
        [TestCase("2,1,1", "1,1,2", true)]
        public void SortedByHashCode(string raw, string expectedInts, bool expectedCollision)
        {
            var ints = raw.Split(',').Select(int.Parse);
            var expected = expectedInts.Split(',').Select(int.Parse);
            var sorted = new Gu.State.Set.SortedByHashCode<int>(ints, EqualityComparer<int>.Default);
            CollectionAssert.AreEqual(expected, sorted);
            Assert.AreEqual(expectedCollision, sorted.HasCollision);
        }
    }
}
