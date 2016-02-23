namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    using ComplexType = Gu.ChangeTracking.Tests.ChangeTrackerStubs.ComplexType;

    public partial class DirtyTrackerTests
    {
        public class Simple
        {
            [Test]
            public void TracksX()
            {
                var source = new SimpleDirtyTrackClass { Value = 1, Excluded = 2 };
                var target = new SimpleDirtyTrackClass { Value = 3, Excluded = 4 };
                var tracker = DirtyTracker.Track(source, target);
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                using (tracker)
                {
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.Count());
                    CollectionAssert.IsEmpty(changes);

                    source.Value = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.Count());
                    CollectionAssert.IsEmpty(changes);

                    source.Value = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.Count());
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    source.Excluded = 4;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.Count());
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    source.OnPropertyChanged(nameof(ComplexType.Excluded));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.Count());
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    source.Excluded = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.Count());
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                source.Value = 4;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual(1, tracker.Diff.Count());
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public void TracksY()
            {
                var source = new SimpleDirtyTrackClass { Value = 1, Excluded = 2 };
                var target = new SimpleDirtyTrackClass { Value = 3, Excluded = 4 };
                var tracker = DirtyTracker.Track(source, target);
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                using (tracker)
                {
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.Count());
                    CollectionAssert.IsEmpty(changes);

                    target.Value = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.Count());
                    CollectionAssert.IsEmpty(changes);

                    target.Value = 1;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.Count());
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.Excluded = 2;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.Count());
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.OnPropertyChanged(nameof(ComplexType.Excluded));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.Count());
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.Excluded = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.Count());
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                target.Value = 4;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual(1, tracker.Diff.Count());
                CollectionAssert.AreEqual(expectedChanges, changes);
            }
        }
    }
}