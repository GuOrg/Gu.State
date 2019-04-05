// ReSharper disable RedundantArgumentDefaultValue
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
#if DEBUG // debug build keeps instances alive longer for nicer debugging experience
                Assert.Inconclusive();
#endif
            }

            [Test]
            public void CreateAndDisposeComplexType()
            {
                var source = new ComplexType();
                WeakReference weakReference = new WeakReference(source);
                var tracker = Track.Changes(source, ReferenceHandling.Structural);
#pragma warning disable IDISP016 // Don't use disposed instance.
                tracker.Dispose();
#pragma warning restore IDISP016 // Don't use disposed instance.
                source = null;
                GC.Collect();
                Assert.IsFalse(weakReference.IsAlive);
                Assert.NotNull(tracker); // touching it here so it is not optimized away.
            }

            [Test]
            public void CreateAndDisposeWithComplexType()
            {
                var source = new With<ComplexType> { Value = new ComplexType(1, 2) };

                using (Track.Changes(source, ReferenceHandling.Structural))
                {
                }

                var wrx = new System.WeakReference(source);
                var wrxc = new System.WeakReference(source.Value);
                source = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wrxc.IsAlive);
            }

            [Test]
            public void DoesNotLeakTrackedProperty()
            {
                var source = new With<ComplexType> { Value = new ComplexType() };
                using (Track.Changes(source, ReferenceHandling.Structural))
                {
                    var weakReference = new WeakReference(source.Value);
                    source.Value = null;
                    GC.Collect();
                    Assert.IsFalse(weakReference.IsAlive);
                }

                Assert.NotNull(source); // touching it here so it is not optimized away.
            }
        }
    }
}