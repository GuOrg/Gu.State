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
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var countProperty = typeof(ObservableCollection<ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new ComplexType("a", 1));
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
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var countProperty = typeof(ObservableCollection<ComplexType>).GetProperty("Count");
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Add(new ComplexType("b", 2));
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
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 1 };
                //var countProperty = typeof(ObservableCollection<ComplexType>).GetProperty("Count");
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x.RemoveAt(1);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void RemoveStillDirty()
            {
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 3 };
                //var countProperty = typeof(ObservableCollection<ComplexType>).GetProperty("Count");
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x.RemoveAt(1);
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    expectedChanges.AddRange(new[] { "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 1, 2 };
                //var countProperty = typeof(ObservableCollection<ComplexType>).GetProperty("Count");
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x.Clear();
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);

                //    y.Clear();
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void ClearBothWhenDirty()
            {
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 3, 4, 5 };
                //var countProperty = typeof(ObservableCollection<ComplexType>).GetProperty("Count");
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x.Clear();
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { countProperty, ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    y.Clear();
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void MoveX()
            {
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 1, 2 };
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x.Move(0, 1);
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);

                //    x.Move(0, 1);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void MoveXThenY()
            {
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 1, 2 };
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x.Move(0, 1);
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);

                //    y.Move(0, 1);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void Replace()
            {
                Assert.Fail("Change to complex type");
                //var x = new ObservableCollection<ComplexType> { 1, 2 };
                //var y = new ObservableCollection<ComplexType> { 1, 2 };
                //var changes = new List<string>();
                //var expectedChanges = new List<string>();
                //using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                //{
                //    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    CollectionAssert.IsEmpty(changes);

                //    x[0] = 3;
                //    Assert.AreEqual(true, tracker.IsDirty);
                //    CollectionAssert.AreEqual(new[] { ItemDirtyTracker.IndexerProperty }, tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);

                //    y[0] = 3;
                //    Assert.AreEqual(false, tracker.IsDirty);
                //    CollectionAssert.IsEmpty(tracker.Diff);
                //    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                //    CollectionAssert.AreEqual(expectedChanges, changes);
                //}
            }

            [Test]
            public void TracksObservableCollectionOfComplexType()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    Assert.Fail();
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    //x.Name = "newName";
                    //Assert.AreEqual(true, tracker.IsDirty);
                    //expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    //CollectionAssert.AreEqual(expectedChanges, changes);
                    //CollectionAssert.AreEqual(new[] { typeof(WithComplexProperty).GetProperty(nameof(x.Name)) }, tracker.Diff);

                    //y.Name = "newName";
                    //Assert.AreEqual(false, tracker.IsDirty);
                    //expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    //CollectionAssert.AreEqual(expectedChanges, changes);
                    //CollectionAssert.IsEmpty(tracker.Diff);

                    //x.ComplexType = new ComplexType("a", 1);
                    //Assert.AreEqual(true, tracker.IsDirty);
                    //expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    //CollectionAssert.AreEqual(expectedChanges, changes);
                    //CollectionAssert.AreEqual(new[] { typeof(WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    //y.ComplexType = new ComplexType("a", 1);
                    //Assert.AreEqual(false, tracker.IsDirty);
                    //expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    //CollectionAssert.AreEqual(expectedChanges, changes);
                    //CollectionAssert.IsEmpty(tracker.Diff);

                    //x.ComplexType.Name = "newName";
                    //Assert.AreEqual(true, tracker.IsDirty);
                    //expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    //CollectionAssert.AreEqual(expectedChanges, changes);
                    //CollectionAssert.AreEqual(new[] { typeof(WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    //y.ComplexType.Name = "newName";
                    //Assert.AreEqual(false, tracker.IsDirty);
                    //expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    //CollectionAssert.AreEqual(expectedChanges, changes);
                    //CollectionAssert.IsEmpty(tracker.Diff);
                }
            }

            [Test]
            public void TracksWithObservableCollectionProperty()
            {
                var x = new WithObservableCollectionProperty();
                var y = new WithObservableCollectionProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    Assert.Fail();
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }
    }
}
