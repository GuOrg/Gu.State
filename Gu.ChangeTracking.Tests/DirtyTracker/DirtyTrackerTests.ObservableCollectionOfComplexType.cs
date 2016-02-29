namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class ObservableCollectionOfComplexType
        {
            [Test]
            public void AddSameToBoth()
            {
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType>();
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType>();
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
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
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType>();
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType>();
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new DirtyTrackerTypes.ComplexType("b", 2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.Add("Diff");
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
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1) };
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.RemoveAt(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveStillDirty()
            {
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("c", 3) };
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.RemoveAt(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenDirty()
            {
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("c", 2), new DirtyTrackerTypes.ComplexType("d", 4), new DirtyTrackerTypes.ComplexType("e", 5) };
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    y.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveX()
            {
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
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
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
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
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType> { new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x[0] = new DirtyTrackerTypes.ComplexType("c", 3);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y[0] = new DirtyTrackerTypes.ComplexType("c", 3);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksItems()
            {
                var x = new ObservableCollection<DirtyTrackerTypes.ComplexType>();
                var y = new ObservableCollection<DirtyTrackerTypes.ComplexType>();
                var countProperty = typeof(ObservableCollection<DirtyTrackerTypes.ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
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
                    y[0]= new DirtyTrackerTypes.ComplexType(complexType.Name, complexType.Value);
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
