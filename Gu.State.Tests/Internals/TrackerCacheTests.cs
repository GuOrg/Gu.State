// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.Internals
{
    using System;

    using NUnit.Framework;

    public class TrackerCacheTests
    {
        [Test]
        public void GetOrAddUsingReferencePair()
        {
            var x = new object();
            var y = new object();
            var pair = ReferencePair.GetOrCreate(x, y);
            Assert.AreEqual(pair.Count, 1);
            var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Structural);
            using (var created = TrackerCache.GetOrAdd(x, y, settings, p => p))
            {
                Assert.AreSame(pair, created.Value);
                Assert.AreEqual(pair.Count, 2);
            }

            Assert.AreEqual(1, pair.Count);

#pragma warning disable IDISP016 // Don't use disposed instance.
            pair.Dispose();
#pragma warning restore IDISP016 // Don't use disposed instance.
            Assert.AreEqual(0, pair.Count);
        }

        [Test]
        [Explicit("Just to confirm that it does not work")]
        public void GetRecursive()
        {
            var x = new object();
            var y = new object();
            var settings = PropertiesSettings.GetOrCreate();
            using (var rec = TrackerCache.GetOrAdd(x, y, settings, p => new Recursive(p, settings)))
            {
                Assert.AreSame(rec, rec.Value.Next);
            }
        }

        internal sealed class Recursive : IDisposable
        {
            public Recursive(IRefCounted<ReferencePair> pair, MemberSettings settings)
            {
                this.Next = TrackerCache.GetOrAdd(pair.Value.X, pair.Value.Y, settings, x => new Recursive(x, settings));
            }

            public IRefCounted<Recursive> Next { get; }

            public void Dispose()
            {
            }
        }
    }
}