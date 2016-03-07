namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class ObservableCollectionOfInt
        {
            [Test]
            public void AddSameToBoth()
            {
                var x = new ObservableCollection<int>();
                var y = new ObservableCollection<int>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void AddDifferent()
            {
                var x = new ObservableCollection<int>();
                var y = new ObservableCollection<int>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(2);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveTheDifference()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 1 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.RemoveAt(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveStillDirty()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 3 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.RemoveAt(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 1, 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenDirty()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 3, 4, 5 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    y.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveX()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 1, 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Move(0, 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Move(0, 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveXThenY()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 1, 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Move(0, 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Move(0, 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void Replace()
            {
                var x = new ObservableCollection<int> { 1, 2 };
                var y = new ObservableCollection<int> { 1, 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x[0] = 3;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y[0] = 3;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }
        }
    }
}
