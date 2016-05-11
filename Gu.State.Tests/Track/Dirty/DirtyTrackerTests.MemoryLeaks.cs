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
                Assert.Inconclusive("debug build keeps instances alive longer for nicer debugging experience");
#endif
            }

            [Test]
            public void ComplexType()
            {
                var x = new ComplexType("a", 1);
                var y = new ComplexType("a", 1);
                var changes = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                }

                x.Value++;
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
            public void WithComplexProperty()
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var changes = new List<string>();
                var settings = PropertiesSettings.GetOrCreate();
                using (var tracker = Track.IsDirty(x, y, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                }

                var node = DirtyTrackerNode.GetOrCreate(x.Value, y.Value, settings, true);
                Assert.AreEqual(1, node.Count);

                x.ComplexType = new ComplexType("b", 2);
                CollectionAssert.IsEmpty(changes);

                var wrx = new System.WeakReference(x);
                var wrxc = new System.WeakReference(x.ComplexType);
                var wry = new System.WeakReference(y);
                var wryc = new System.WeakReference(y.ComplexType);
                x = null;
                y = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wrxc.IsAlive);
                Assert.AreEqual(false, wry.IsAlive);
                Assert.AreEqual(false, wryc.IsAlive);
            }

            [Test]
            public void DoesNotLeakTrackedProperty()
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };

                using (var tracker = Track.IsDirty(x, y))
                {
                    Assert.AreEqual(false, tracker.IsDirty);

                    var weakReference = new System.WeakReference(x.ComplexType);
                    Assert.AreEqual(true, weakReference.IsAlive);
                    x.ComplexType = null;
                    System.GC.Collect();
                    Assert.AreEqual(false, weakReference.IsAlive);

                    weakReference.Target = y.ComplexType;
                    y.ComplexType = null;
                    System.GC.Collect();
                    Assert.AreEqual(false, weakReference.IsAlive);
                }
            }
        }
    }
}