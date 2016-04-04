namespace Gu.State.Tests.EqualByTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public abstract class DictionaryTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling) where T : class;

        [TestCase(1, "one", true)]
        [TestCase(2, "one", false)]
        [TestCase(1, "two", false)]
        public void DictionaryToSameLength(int key, string value, bool expected)
        {
            var x = new Dictionary<int, string> { { key, value } };
            var y = new Dictionary<int, string> { { 1, "one" } };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void DictionaryToEmpty()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string>();
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
        [TestCase(ReferenceHandling.References)]
        public void DictionaryToLonger(ReferenceHandling referenceHandling)
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void DictionaryWithCollisionsWhenEqual()
        {
            var k1 = new EqualByTypes.HashCollisionType();
            var k2 = new EqualByTypes.HashCollisionType();
            var x = new Dictionary<EqualByTypes.HashCollisionType, string> { { k1, "1" }, { k2, "2" } };
            var y = new Dictionary<EqualByTypes.HashCollisionType, string> { { k1, "1" }, { k2, "2" } };
            Assert.AreEqual(2, x.Count);
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            y = new Dictionary<EqualByTypes.HashCollisionType, string> { { k2, "2" }, { k1, "1" } };
            result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }
    }
}