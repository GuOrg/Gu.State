namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class ObservableCollectionOfComplexType
        {
            [Test]
            public void AddSameToBoth()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void AddDifferent()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new ComplexType("b", 2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x[0].Name = "b";
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y[0].Value = 1;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveTheDifference()
            {
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("a", 1) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
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
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("c", 3) };
                var changes = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.RemoveAt(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
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
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("c", 2), new ComplexType("d", 4), new ComplexType("e", 5) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
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
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
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
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
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
                var x = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var y = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x[0] = new ComplexType("c", 3);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y[0] = new ComplexType("c", 3);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksItems()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    var complexType = y[0];
                    y[0] = new ComplexType(complexType.Name, complexType.Value);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    complexType.Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }
        }
    }
}
