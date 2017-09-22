namespace Gu.State.Tests.CopyTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public abstract class DictionaryTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [Test]
        public void DictionaryToSameLength()
        {
            var source = new Dictionary<int, string> { { 1, "one" } };
            var target = new Dictionary<int, string> { { 1, "ett" } };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new KeyValuePair<int, string>(1, "one"), };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var source = new Dictionary<int, string> { { 1, "one" } };
            var target = new Dictionary<int, string>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new KeyValuePair<int, string>(1, "one"), };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }

        [Test]
        public void DictionaryToLonger()
        {
            var source = new Dictionary<int, string> { { 1, "one" } };
            var target = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new KeyValuePair<int, string>(1, "one") };
            CollectionAssert.AreEqual(expected, source);
            CollectionAssert.AreEqual(expected, target);
        }
    }
}