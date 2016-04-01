namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Complex
        {
            [Test]
            public void CreateAndDispose()
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    var xc = x.ComplexType;
                    x.ComplexType = null;
                    var yc = y.ComplexType;
                    y.ComplexType = null;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                    var wrxc = new System.WeakReference(xc);
                    var wryc = new System.WeakReference(yc);
                    xc = null;
                    yc = null;
                    System.GC.Collect();
                    Assert.AreEqual(false, wrxc.IsAlive);
                    Assert.AreEqual(false, wryc.IsAlive);
#endif
                }

                x.ComplexType = new ComplexType("b", 2);
                CollectionAssert.AreEqual(expectedChanges, changes);

#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                var wrx = new System.WeakReference(x);
                var wry = new System.WeakReference(y);
                x = null;
                y = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wry.IsAlive);
#endif
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
            public void WithComplexPropertyTracks(ReferenceHandling referenceHandling)
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "newName1";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("WithComplexProperty Name x: newName1 y: null", tracker.Diff.ToString("", " "));

                    y.Name = "newName1";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.ComplexType = new ComplexType("a", 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null", tracker.Diff.ToString("", " "));

                    y.ComplexType = new ComplexType("a", 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.ComplexType.Name = "newName2";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("WithComplexProperty ComplexType Name x: newName2 y: a", tracker.Diff.ToString("", " "));

                    x.ComplexType.Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("WithComplexProperty ComplexType Name x: newName2 y: a Value x: 2 y: 1", tracker.Diff.ToString("", " "));

                    y.ComplexType.Name = "newName2";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual("WithComplexProperty ComplexType Value x: 2 y: 1", tracker.Diff.ToString("", " "));

                    y.ComplexType.Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }
        }
    }
}
