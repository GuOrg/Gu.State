namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;

    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Immutable
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            public void Tracks(ReferenceHandling referenceHandling)
            {
                var x = new DirtyTrackerTypes.WithImmutableProperty();
                var y = new DirtyTrackerTypes.WithImmutableProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "newName1";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithImmutableProperty).GetProperty(nameof(x.Name)) }, tracker.Diff);

                    y.Name = "newName1";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ImmutableValue = new DirtyTrackerTypes.WithGetReadOnlyPropertySealed<int>(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithImmutableProperty).GetProperty(nameof(x.ImmutableValue)) }, tracker.Diff);

                    y.ImmutableValue = new DirtyTrackerTypes.WithGetReadOnlyPropertySealed<int>(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ImmutableValue = new DirtyTrackerTypes.WithGetReadOnlyPropertySealed<int>(2);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithImmutableProperty).GetProperty(nameof(x.ImmutableValue)) }, tracker.Diff);
                }
            }
        }
    }
}
