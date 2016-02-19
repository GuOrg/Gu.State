namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Collection
        {
            [Test]
            public void TracksObservableCollectionOfInt()
            {
                var x = new ObservableCollection<int>();
                var y = new ObservableCollection<int>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
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

                    Assert.Inconclusive();
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
