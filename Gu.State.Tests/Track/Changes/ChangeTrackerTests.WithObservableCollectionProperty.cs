namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            [Test]
            public void CreateAndDispose()
            {
                var source = new With<ObservableCollection<ComplexType>>();

                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Value = new ObservableCollection<ComplexType>();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    source.Value[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    var observableCollection = source.Value;
                    source.Value = new ObservableCollection<ComplexType>();
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    source.Value[0].Value++;
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);

                    observableCollection.Add(new ComplexType());
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);
                    tracker.Dispose();

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(6, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);
                }

                source.Value.Add(new ComplexType());
                CollectionAssert.AreEqual(CreateExpectedChangeArgs(6), changes);
            }

            [Test]
            public void ReplaceCollection()
            {
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType>() };
                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    var observableCollection = source.Value;
                    source.Value = new ObservableCollection<ComplexType> { new ComplexType() };
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.Value[0].Value++;
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
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType>() };
                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    source.Value[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    source.Value[0].Value++;
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    source.Value[1].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }
            }

            [Test]
            public void Remove()
            {
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType> { new ComplexType(), null } };
                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    var complexType = source.Value[0];
                    source.Value.RemoveAt(0);
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
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType> { new ComplexType(), null } };
                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.Value.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void Replace()
            {
                var source = new With<ObservableCollection<ComplexType>>
                {
                    Value = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() }
                };

                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Value[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void Move()
            {
                var source = new With<ObservableCollection<ComplexType>>
                {
                    Value = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() }
                };

                var changes = new List<object>();
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.Move(1, 0);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}