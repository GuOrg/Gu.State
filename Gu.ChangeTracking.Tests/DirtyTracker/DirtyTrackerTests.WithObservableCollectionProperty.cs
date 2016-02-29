namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            private static readonly PropertyInfo ComplexesProperty = typeof(DirtyTrackerTypes.WithObservableCollectionProperties).GetProperty(nameof(DirtyTrackerTypes.WithObservableCollectionProperties.Complexes));

            [Test]
            public void AddSameToBoth()
            {
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties();
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties();

                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
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
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties();
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new DirtyTrackerTypes.ComplexType("b", 2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Name = "b";
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
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
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.RemoveAt(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveStillDirty()
            {
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("c", 3));
                var changes = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.RemoveAt(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
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
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenDirty()
            {
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("c", 2), new DirtyTrackerTypes.ComplexType("d", 4), new DirtyTrackerTypes.ComplexType("e", 5));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    y.Complexes.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveX()
            {
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
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
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
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
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
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
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
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
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties(new DirtyTrackerTypes.ComplexType("a", 1), new DirtyTrackerTypes.ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes[0] = new DirtyTrackerTypes.ComplexType("c", 3);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0] = new DirtyTrackerTypes.ComplexType("c", 3);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksItems()
            {
                var x = new DirtyTrackerTypes.WithObservableCollectionProperties();
                var y = new DirtyTrackerTypes.WithObservableCollectionProperties();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new DirtyTrackerTypes.ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    var complexType = y.Complexes[0];
                    y.Complexes[0] = new DirtyTrackerTypes.ComplexType(complexType.Name, complexType.Value);
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
