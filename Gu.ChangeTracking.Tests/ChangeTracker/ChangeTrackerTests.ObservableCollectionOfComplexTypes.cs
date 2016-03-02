namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;

    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class ObservableCollectionOfComplexTypes
        {
            [Test]
            public void CreateAndDispose()
            {
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType>();
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    root.Add(new ComplexType());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    tracker.Dispose();

                    root.Add(new ComplexType());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }

                root.Add(new ComplexType());
                CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
            }

            [Test]
            public void Add()
            {
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType>();
                using (var tracker = ChangeTracker.Track(root, PropertiesSettings.GetOrCreate()))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Add(new ComplexType());
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    root[0].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    root[0].Value++;
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);

                    root[1].Value++;
                    Assert.AreEqual(7, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(7), changes);
                }
            }

            [Test]
            public void Remove()
            {
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), null };
                using (var tracker = ChangeTracker.Track(root, PropertiesSettings.GetOrCreate()))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.RemoveAt(1);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    var complexType = root[0];
                    root.RemoveAt(0);
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    complexType.Value++;
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);
                }
            }

            [Test]
            public void Clear()
            {
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), null };
                using (var tracker = ChangeTracker.Track(root, PropertiesSettings.GetOrCreate()))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Clear();
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);
                }
            }

            [Test]
            public void Replace()
            {
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                using (var tracker = ChangeTracker.Track(root, PropertiesSettings.GetOrCreate()))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void Move()
            {
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                using (var tracker = ChangeTracker.Track(root, PropertiesSettings.GetOrCreate()))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Move(1, 0);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}