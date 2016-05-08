namespace Gu.State.Tests
{
    using System;
    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class MemoryLeaks
        {
            [SetUp]
            public void SetUp()
            {
#if (DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                Assert.Inconclusive();
#endif
            }

            [Test]
            public void ComplexType()
            {
                var item = new ComplexType();
                var tracker = Track.Changes(item, ReferenceHandling.Structural);
                var weakReference = new WeakReference(item);

                item = null;
                GC.Collect();
                Assert.IsTrue(weakReference.IsAlive);

                tracker.Dispose();
                GC.Collect();
                Assert.IsFalse(weakReference.IsAlive);
            }

            [Test]
            public void DoesNotLeakLevel()
            {
                Assert.Fail("Track levels and remove.Level. Then check that it is not kept alive.");
            }
        }
    }
}