namespace Gu.State.Tests.EqualByTests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class DictionaryTests
    {
        public abstract bool EqualByMethod<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

        [TestCase(1, "one", true)]
        [TestCase(2, "one", false)]
        [TestCase(1, "two", false)]
        public void SameLength(int key, string value, bool expected)
        {
            var x = new Dictionary<int, string> { { key, value } };
            var y = new Dictionary<int, string> { { 1, "one" } };
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void OneEmpty()
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string>();
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void OneLonger(ReferenceHandling referenceHandling)
        {
            var x = new Dictionary<int, string> { { 1, "one" } };
            var y = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            var result = this.EqualByMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualByMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithCollisionsWhenEqual()
        {
            var k1 = new HashCollisionType();
            var k2 = new HashCollisionType();
            var x = new Dictionary<HashCollisionType, string> { { k1, "1" }, { k2, "2" } };
            var y = new Dictionary<HashCollisionType, string> { { k1, "1" }, { k2, "2" } };
            Assert.AreEqual(2, x.Count);
            var result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            y = new Dictionary<HashCollisionType, string> { { k2, "2" }, { k1, "1" } };
            result = this.EqualByMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableDictionaryOfIntsAndStringsWhenEqual(ReferenceHandling referenceHandling)
        {
            var builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, string>();
            builder.Add(1, "one");
            builder.Add(2, "two");
            var x = builder.ToImmutable();
            var y = builder.ToImmutable();
            Assert.AreEqual(true, this.EqualByMethod(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualByMethod(y, x, referenceHandling));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableDictionaryOfIntsAndStringsWhenNotEqualKeys(ReferenceHandling referenceHandling)
        {
            var builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, string>();
            builder.Add(1, "one");
            builder.Add(2, "two");
            var x = builder.ToImmutable();

            builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, string>();
            builder.Add(1, "one");
            builder.Add(3, "two");
            var y = builder.ToImmutable();
            Assert.AreEqual(false, this.EqualByMethod(x, y, referenceHandling));
            Assert.AreEqual(false, this.EqualByMethod(y, x, referenceHandling));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableDictionaryOfIntsAndStringsWhenNotEqualValues(ReferenceHandling referenceHandling)
        {
            var builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, string>();
            builder.Add(1, "one");
            builder.Add(2, "two");
            var x = builder.ToImmutable();

            builder = System.Collections.Immutable.ImmutableDictionary.CreateBuilder<int, string>();
            builder.Add(1, "one");
            builder.Add(2, "två");
            var y = builder.ToImmutable();
            Assert.AreEqual(false, this.EqualByMethod(x, y, referenceHandling));
            Assert.AreEqual(false, this.EqualByMethod(y, x, referenceHandling));
        }
    }
}