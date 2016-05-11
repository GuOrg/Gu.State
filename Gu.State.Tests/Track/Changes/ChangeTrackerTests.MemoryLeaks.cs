namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

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
            public void CreateAndDisposeComplexType()
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
            public void CreateAndDisposeWithComplexType()
            {
                var x = new With<ComplexType> { Value = new ComplexType(1, 2) };

                using (var tracker = Track.Changes(x, ReferenceHandling.Structural))
                {
                }

                var wrx = new System.WeakReference(x);
                var wrxc = new System.WeakReference(x.Value);
                x = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wrxc.IsAlive);
            }

            [Test]
            public void DoesNotLeakTrackedProperty()
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