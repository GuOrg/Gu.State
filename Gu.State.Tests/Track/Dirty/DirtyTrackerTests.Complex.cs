namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Complex
        {
            [Test]
            public void WithComplexPropertyTracks()
            {
                var x = new DirtyTrackerTypes.WithComplexProperty();
                var y = new DirtyTrackerTypes.WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "newName1";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.Name)) }, tracker.Diff);

                    y.Name = "newName1";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ComplexType = new DirtyTrackerTypes.ComplexType("a", 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType = new DirtyTrackerTypes.ComplexType("a", 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ComplexType.Name = "newName2";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    x.ComplexType.Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType.Name = "newName2";
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType.Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);
                }
            }
        }
    }
}
