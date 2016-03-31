namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Simple
        {
            [Test]
            public void CreateAndDispose()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Value1++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("SimpleDirtyTrackClass Value x: 2 y: 1", tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                x.Value1++;
                CollectionAssert.AreEqual(expectedChanges, changes);

                var wrx = new WeakReference(x);
                var wry = new WeakReference(y);
                x = null;
                y = null;

#if (!DEBUG)
                Assert.AreEqual(false, wrx.IsAlive);
                Assert.AreEqual(false, wry.IsAlive);
#endif
            }

            [Test]
            public void TracksX()
            {
                var x = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new SimpleDirtyTrackClass { Value1 = 3, Value2 = 4 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.IsDirty(x, y))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("2", tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Value1 = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Value1 = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Value2 = 4;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value2));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.ToString("", " "));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Value2 = 3;
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
                var source = new SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var target = new SimpleDirtyTrackClass { Value1 = 3, Value2 = 4 };
                var tracker = Track.IsDirty(source, target);
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                using (tracker)
                {
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    target.Value1 = 5;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(2, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    target.Value1 = 1;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.Value2 = 2;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.OnPropertyChanged(nameof(SimpleDirtyTrackClass.Value2));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(0, tracker.Diff.ToString("", " "));
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    target.Value2 = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.IsDirty));
                    expectedChanges.Add(nameof(DirtyTracker<INotifyPropertyChanged>.Diff));
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                target.Value1 = 4;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual(1, tracker.Diff.ToString("", " "));
                CollectionAssert.AreEqual(expectedChanges, changes);
            }
        }
    }
}