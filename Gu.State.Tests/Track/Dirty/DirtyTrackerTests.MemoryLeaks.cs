namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class MemoryLeaks
        {
            [SetUp]
            public void SetUp()
            {
#if (DEBUG)
            {
                Assert.Inconclusive("debug build keeps instances alive longer for nicer debugging experience");
            }
#endif
            }

            [Test]
            public void CreateAndDisposeWithComplexProperty()
            {
                var x = new WithComplexProperty {ComplexType = new ComplexType("a", 1)};
                var y = new WithComplexProperty {ComplexType = new ComplexType("a", 1)};
                var changes = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                }

                x.ComplexType = new ComplexType("b", 2);
                CollectionAssert.IsEmpty(changes);

                var wrx = new System.WeakReference(x);
                var wry = new System.WeakReference(y);
                x = null;
                y = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wry.IsAlive);
            }

            [Test]
            public void DoesNotLeakTrackedProperty()
            {
                var x = new WithComplexProperty {ComplexType = new ComplexType("a", 1)};
                var y = new WithComplexProperty {ComplexType = new ComplexType("a", 1)};

                using (var tracker = Track.IsDirty(x, y))
                {
                    Assert.AreEqual(false, tracker.IsDirty);

                    var wrxc = new System.WeakReference(x.ComplexType);
                    Assert.AreEqual(true, wrxc.IsAlive);
                    x.ComplexType = null;
                    System.GC.Collect();
                    Assert.AreEqual(false, wrxc.IsAlive);

                    var wryc = new System.WeakReference(y.ComplexType);
                    y.ComplexType = null;
                    System.GC.Collect();
                    Assert.AreEqual(false, wryc.IsAlive);
                }
            }
        }
    }
}