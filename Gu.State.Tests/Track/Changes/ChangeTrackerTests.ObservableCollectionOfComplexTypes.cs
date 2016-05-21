// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class ObservableCollectionOfComplexTypes
        {
            [Test]
            public void CreateAndDispose()
            {
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                tracker.Dispose();

                source.Add(new ComplexType());
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                item.Value++;
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void Add()
            {
                var source = new ObservableCollection<ComplexType>();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Add(new ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new AddEventArgs(0)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    source.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expected = new[]
                                   {
                                       RootChangeEventArgs.Create(node, new AddEventArgs(0)),
                                       RootChangeEventArgs.Create(node, new AddEventArgs(1))
                                   };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void AddThenItemChange()
            {
                var source = new ObservableCollection<ComplexType>();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Add(new ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    expectedChanges.Add(RootChangeEventArgs.Create(node, new AddEventArgs(0)));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);

                    expectedChanges.Add(RootChangeEventArgs.Create(node, new AddEventArgs(1)));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(GraphChangeEventArgs.Create(source, 0));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source[0].Value++;
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    source[1].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }
            }

            [Test]
            public void Remove()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new RemoveEventArgs(1)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    source.RemoveAt(0);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expected = new[]
                                   {
                                       RootChangeEventArgs.Create(node, new RemoveEventArgs(1)),
                                       RootChangeEventArgs.Create(node, new RemoveEventArgs(0))
                                   };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void RemoveThenItemChangeHigherIndex()
            {
                Assert.Fail();
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), null };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    var complexType = root[0];
                    root.RemoveAt(0);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    complexType.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void RemoveStopsTracking()
            {
                Assert.Fail();
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), null };
                using (var tracker = Track.Changes(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    var complexType = root[0];
                    root.RemoveAt(0);
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
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item, null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new ResetEventArgs(null, null)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    source.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expected = new[]
                                   {
                                       RootChangeEventArgs.Create(node, new ResetEventArgs(null, null)),
                                       RootChangeEventArgs.Create(node, new ResetEventArgs(null, null))
                                   };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void ClearStopsTracking()
            {
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item, null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new ResetEventArgs(null, null)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    item.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void Replace()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new ReplaceEventArgs(0)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void ReplaceStopsListeningToOld()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    var old = source[0];
                    source[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new ReplaceEventArgs(0)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    old.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }


            [Test]
            public void ReplaceStartsListeningToNew()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new ReplaceEventArgs(0)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    source[0].Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default); // fix this
                }
            }

            [Test]
            public void Move()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Move(1, 0);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new MoveEventArgs(1, 0)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void MoveToHigherThenItemChange()
            {
                Assert.Fail();
                var changes = new List<object>();
                var root = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType(), new ComplexType() };
                using (var tracker = Track.Changes(root))
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


            [Test]
            public void MoveToLowerThenItemChange()
            {
                Assert.Fail();
                var changes = new List<object>();
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType(), new ComplexType() };
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Move(1, 0);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void ItemNotifies()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source[0].Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[]
                                       {
                                           GraphChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value, 0)
                                                               .With(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, false).Value, source[0].GetType().GetProperty(nameof(ComplexType.Value)))
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void SameItemTwiceNotifies()
            {
                var changes = new List<object>();
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item, item };
                using (var tracker = Track.Changes(source))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    item.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }
        }
    }
}