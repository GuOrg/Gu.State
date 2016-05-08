namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class WithComplex
        {
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
            public void WithComplexPropertyHandlesNull(ReferenceHandling referenceHandling)
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

                    x.ComplexType = new ComplexType("a", 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    var expected = "WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null";
                    var actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.ComplexType = new ComplexType("a", 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);

                    x.ComplexType = null;
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    expected = "WithComplexProperty ComplexType x: null y: Gu.State.Tests.DirtyTrackerTypes+ComplexType";
                    actual = tracker.Diff.ToString("", " ");
                    Assert.AreEqual(expected, actual);

                    y.ComplexType = null;
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    Assert.AreEqual(null, tracker.Diff);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.StructuralWithReferenceLoops)]
            public void WithComplexPropertyTracksNested(ReferenceHandling referenceHandling)
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
