namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            [Test]
            public void AddSameToBoth()
            {
                var x = new WithObservableCollectionProperties();
                var y = new WithObservableCollectionProperties();
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Complexes");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void AddDifferent()
            {
                var x = new WithObservableCollectionProperties();
                var y = new WithObservableCollectionProperties();
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new ComplexType("b", 2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Name = "b";
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value = 1;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveTheDifference()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1));
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.RemoveAt(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveStillDirty()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("c", 3));
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.RemoveAt(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenDirty()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("c", 2), new ComplexType("d", 4), new ComplexType("e", 5));
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    y.Complexes.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveX()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Move(0, 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes.Move(0, 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveXThenY()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Move(0, 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Move(0, 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void Replace()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes[0] = new ComplexType("c", 3);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0] = new ComplexType("c", 3);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksItems()
            {
                var x = new WithObservableCollectionProperties();
                var y = new WithObservableCollectionProperties();
                var countProperty = typeof(WithObservableCollectionProperties).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    var complexType = y.Complexes[0];
                    y.Complexes[0] = new ComplexType(complexType.Name, complexType.Value);
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
