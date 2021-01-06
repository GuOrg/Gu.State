// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public static partial class ChangeTrackerTests
    {
        public static class ObservableCollectionOfComplexTypes
        {
            [Test]
            public static void CreateAndDispose()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

#pragma warning disable IDISP016, IDISP017 // Don't use disposed instance.
                tracker.Dispose();
#pragma warning restore IDISP016, IDISP017 // Don't use disposed instance.

                source.Add(new ComplexType());
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source[0].Value++;
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source[1].Value++;
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public static void Add()
            {
                var source = new ObservableCollection<ComplexType>();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Add(new ComplexType());
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new AddEventArgs(source, 0)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.Add(new ComplexType());
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expected = new[]
                {
                    RootChangeEventArgs.Create(node, new AddEventArgs(source, 0)),
                    RootChangeEventArgs.Create(node, new AddEventArgs(source, 1)),
                };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void AddThenItemsNotifies()
            {
                var source = new ObservableCollection<ComplexType>();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Add(new ComplexType());
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new AddEventArgs(source, 0)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source.Add(new ComplexType());
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);

                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new AddEventArgs(source, 1)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[0].Value++;
                Assert.AreEqual(3, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[0].Value++;
                Assert.AreEqual(4, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[1].Value++;
                Assert.AreEqual(5, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 1, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[1], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[1], source[1].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public static void Insert(int index)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Insert(index, new ComplexType());
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new AddEventArgs(source, index)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [TestCase(0)]
            [TestCase(1)]
            public static void InsertThenItemsNotifies(int index)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Insert(index, new ComplexType());
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new AddEventArgs(source, index)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[0].Value++;
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[1].Value++;
                Assert.AreEqual(3, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 1, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[1], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[1], source[1].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[2].Value++;
                Assert.AreEqual(4, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 2, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[2], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[2], source[2].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void Remove()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.RemoveAt(1);
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new RemoveEventArgs(source, 1)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.RemoveAt(0);
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expected = new[]
                {
                    RootChangeEventArgs.Create(node, new RemoveEventArgs(source, 1)),
                    RootChangeEventArgs.Create(node, new RemoveEventArgs(source, 0)),
                };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public static void RemoveThenItemNotifies(int index)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.RemoveAt(index);
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new RemoveEventArgs(source, index)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[0].Value++;
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[1].Value++;
                Assert.AreEqual(3, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 1, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[1], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[1], source[1].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void RemoveStopsTracking()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.RemoveAt(1);
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new RemoveEventArgs(source, 1)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                var complexType = source[0];
                source.RemoveAt(0);
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new RemoveEventArgs(source, 0)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                complexType.Value++;
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void Clear()
            {
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item, null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Clear();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new ResetEventArgs(source)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.Clear();
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expected = new[]
                {
                    RootChangeEventArgs.Create(node, new ResetEventArgs(source)),
                    RootChangeEventArgs.Create(node, new ResetEventArgs(source)),
                };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void ClearStopsTracking()
            {
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item, null };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Clear();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new ResetEventArgs(source)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                item.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void Replace()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source[0] = new ComplexType();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new ReplaceEventArgs(source, 0)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void ReplaceStopsListeningToOld()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                var old = source[0];
                source[0] = new ComplexType();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new ReplaceEventArgs(source, 0)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                old.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void ReplaceStartsListeningToNew()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expected = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source[0] = new ComplexType();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;

                expected.Add(RootChangeEventArgs.Create(node, new ReplaceEventArgs(source, 0)));
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source[0].Value++;
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false)
                                                  .Value;
                expected.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))));
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void Move()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Move(1, 0);
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                var expected = new[] { RootChangeEventArgs.Create(node, new MoveEventArgs(source, 1, 0)) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [TestCase(0, 2)]
            [TestCase(0, 1)]
            [TestCase(1, 2)]
            [TestCase(2, 0)]
            [TestCase(2, 1)]
            [TestCase(1, 0)]
            public static void MoveThenItemsNotifies(int from, int to)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Move(@from, to);
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false).Value;
                expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new MoveEventArgs(source, @from, to)));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[0].Value++;
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[1].Value++;
                Assert.AreEqual(3, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 1, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[1], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[1], source[1].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                source[2].Value++;
                Assert.AreEqual(4, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes" }, propertyChanges);
                expectedChanges.Add(new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 2, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[2], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[2], source[2].GetProperty("Value")))));
                CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void ItemNotifies()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source[0].Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false)
                                                  .Value;
                var expected = new[]
                {
                    new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))),
                };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void SameItemTwiceNotifies()
            {
                var item = new ComplexType();
                var source = new ObservableCollection<ComplexType> { item, item };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                item.Value++;
                Assert.AreEqual(2, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, isRoot: false)
                                                  .Value;
                var expected = new[]
                {
                    new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 0, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[0], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[0], source[0].GetProperty("Value")))),
                    new ItemGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, 1, RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source[1], tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source[1], source[1].GetProperty("Value")))),
                };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }
        }
    }
}
