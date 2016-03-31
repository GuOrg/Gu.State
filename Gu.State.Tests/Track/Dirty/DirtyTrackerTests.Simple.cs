namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Simple
        {
            [Test]
            public void CreateAndDispose()
            {
                Assert.Fail("Verify it stops tracking on dispose");
                Assert.Fail("Check memory leaks");
            }

            [Test]
            public void TracksX()
            {
                var x = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value = 1, Excluded = 2 };
                var y = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value = 3, Excluded = 4 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("2", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Value = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Value = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Excluded = 4;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.OnPropertyChanged(nameof(DirtyTrackerTypes.SimpleDirtyTrackClass.Excluded));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.ToString("", " "));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Excluded = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksY()
            {
                var source = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value = 1, Excluded = 2 };
                var target = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value = 3, Excluded = 4 };
                var tracker = Track.IsDirty(source, target);
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                using (tracker)
                {
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    target.Value = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    target.Value = 1;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.Excluded = 2;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.OnPropertyChanged(nameof(DirtyTrackerTypes.SimpleDirtyTrackClass.Excluded));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.ToString("", " "));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.Excluded = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                target.Value = 4;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                CollectionAssert.AreEqual(expectedChanges, changes);
            }
        }
    }
}