namespace Gu.State.Tests.EqualByTests.Comparers
{
    using System.Collections.Generic;
    using NUnit.Framework;

    public class DictionaryEqualByComparerTests
    {
        [Test]
        public void GetOrCreate()
        {
            var dictionary = new Dictionary<string, int>();
            Assert.IsTrue(DictionaryEqualByComparer.TryGetOrCreate(dictionary, dictionary, out EqualByComparer comparer));
            Assert.IsInstanceOf<DictionaryEqualByComparer<string, int>>(comparer);
        }
    }
}
