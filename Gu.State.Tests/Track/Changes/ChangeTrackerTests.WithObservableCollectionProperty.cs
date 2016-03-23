namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            [Test]
            public void CreateAndDispose()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>>();
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value = new ObservableCollection<ChangeTrackerTypes.ComplexType>();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Value.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Value[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    var observableCollection = root.Value;
                    root.Value = new ObservableCollection<ChangeTrackerTypes.ComplexType>();
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    root.Value.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    root.Value[0].Value++;
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);

                    observableCollection.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);
                    tracker.Dispose();

                    root.Value.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);
                }

                root.Value.Add(new ChangeTrackerTypes.ComplexType());
                CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);
            }

            [Test]
            public void ReplaceCollection()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>> { Value = new ObservableCollection<ChangeTrackerTypes.ComplexType>() };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    var observableCollection = root.Value;
                    root.Value = new ObservableCollection<ChangeTrackerTypes.ComplexType> { new ChangeTrackerTypes.ComplexType() };
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Value[0].Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    observableCollection.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void Add()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>> { Value = new ObservableCollection<ChangeTrackerTypes.ComplexType>() };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Value.Add(new ChangeTrackerTypes.ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Value[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    root.Value[0].Value++;
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    root.Value[1].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }
            }

            [Test]
            public void Remove()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>> { Value = new ObservableCollection<ChangeTrackerTypes.ComplexType> { new ChangeTrackerTypes.ComplexType(), null } };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    var complexType = root.Value[0];
                    root.Value.RemoveAt(0);
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
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>> { Value = new ObservableCollection<ChangeTrackerTypes.ComplexType> { new ChangeTrackerTypes.ComplexType(), null } };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Value.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void Replace()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>>
                {
                    Value = new ObservableCollection<ChangeTrackerTypes.ComplexType> { new ChangeTrackerTypes.ComplexType(), new ChangeTrackerTypes.ComplexType() }
                };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value[0] = new ChangeTrackerTypes.ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void Move()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.With<ObservableCollection<ChangeTrackerTypes.ComplexType>>
                {
                    Value = new ObservableCollection<ChangeTrackerTypes.ComplexType> { new ChangeTrackerTypes.ComplexType(), new ChangeTrackerTypes.ComplexType() }
                };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value.Move(1, 0);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}