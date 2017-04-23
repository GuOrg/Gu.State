namespace Gu.State.Tests.EqualByTests.Comparers
{
    using System.Linq;

    using NUnit.Framework;

    public class EnumerableEqualByComparerTests
    {
        [Test]
        public void GetOrCreate()
        {
            var x = Enumerable.Repeat(1, 1);
            Assert.IsTrue(EnumerableEqualByComparer.TryGetOrCreate(x, x, out EqualByComparer comparer));
            Assert.IsInstanceOf<EnumerableEqualByComparer<int>>(comparer);
        }

        [TestCase("1,2", "1,2", true)]
        [TestCase("1", "1,2", false)]
        [TestCase("1,2", "1", false)]
        public void Equals(string xs, string ys, bool expected)
        {
            var x = xs.Split(',').Select(int.Parse);
            var y = ys.Split(',').Select(int.Parse);
            var comparer = EnumerableEqualByComparer<int>.Default;
            var settings = PropertiesSettings.GetOrCreate();
            Assert.AreEqual(expected, comparer.Equals(x, y, settings, null));
        }
    }
}