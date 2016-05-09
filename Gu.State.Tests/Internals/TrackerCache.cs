namespace Gu.State.Tests.Internals
{
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
            var settings = PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.Structural);
            using (var created = TrackerCache.GetOrAdd(x, y, settings, p => p))
            {
                Assert.AreSame(pair, created.Value);
                Assert.AreEqual(pair.Count, 2);
            }

            Assert.AreEqual(1, pair.Count);

            pair.Dispose();
            Assert.AreEqual(0, pair.Count);
        }
    }
}