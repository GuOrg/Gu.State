namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.State.Tests.ChangeTrackerStubs;

    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            [Test]
            public void CreateAndDispose()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType>();
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Values.Add(new ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Values[0].Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    var observableCollection = root.Values;
                    root.Values = new ObservableCollection<ComplexType>();
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    root.Values.Add(new ComplexType());
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    root.Values[0].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    observableCollection.Add(new ComplexType());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                    tracker.Dispose();

                    root.Values.Add(new ComplexType());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }

                root.Values.Add(new ComplexType());
                CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
            }

            [Test]
            public void ReplaceCollection()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType>();
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    var observableCollection = root.Values;
                    root.Values = new ObservableCollection<ComplexType> { new ComplexType() };
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Values[0].Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    observableCollection.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void Add()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType>();
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Values.Add(new ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Values.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Values[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    root.Values[0].Value++;
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    root.Values[1].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }
            }

            [Test]
            public void Remove()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType> { Values = new ObservableCollection<ComplexType> { new ComplexType(), null } };
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Values.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    var complexType = root.Values[0];
                    root.Values.RemoveAt(0);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    complexType.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void Clear()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType> { Values = new ObservableCollection<ComplexType> { new ComplexType(), null } };
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Values.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Values.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void Replace()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType> { Values = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() } };
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Values[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void Move()
            {
                var changes = new List<object>();
                var root = new WithObservableCollectionProperty<ComplexType> { Values = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() } };
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Values.Move(1, 0);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}