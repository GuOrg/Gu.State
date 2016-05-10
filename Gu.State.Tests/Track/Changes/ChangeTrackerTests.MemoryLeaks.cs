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
                WeakReference weakReference = new WeakReference(item);
                var tracker = Track.Changes(item, ReferenceHandling.Structural);
                tracker.Dispose();
                item = null;
                GC.Collect();
                Assert.IsFalse(weakReference.IsAlive);
                Assert.NotNull(tracker); // touching it here so it is not optimized away.
            }

            [Test]
            public void DoesNotLeakLevel()
            {
                var item = new With<ComplexType> { Value = new ComplexType() };
                using (var tracker = Track.Changes(item, ReferenceHandling.Structural))
                {
                    var weakReference = new WeakReference(item.Value);
                    item.Value = null;
                    GC.Collect();
                    Assert.IsFalse(weakReference.IsAlive);
                }

                Assert.NotNull(item); // touching it here so it is not optimized away.
            }
        }
    }
}