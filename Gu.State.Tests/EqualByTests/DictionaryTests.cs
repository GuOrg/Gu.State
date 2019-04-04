namespace Gu.State.Tests.EqualByTests
{
    using NUnit.Framework;

    public abstract class DictionaryTests
    {
        public abstract bool EqualBy<T>(T source, T target, ReferenceHandling referenceHandling)
            where T : class;

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
            Assert.AreEqual(true, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(true, this.EqualBy(y, x, referenceHandling));
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
            Assert.AreEqual(false, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(false, this.EqualBy(y, x, referenceHandling));
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
            Assert.AreEqual(false, this.EqualBy(x, y, referenceHandling));
            Assert.AreEqual(false, this.EqualBy(y, x, referenceHandling));
        }
    }
}