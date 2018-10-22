// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            [Test]
            public void CreateAndDispose()
            {
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType> { new ComplexType() } };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var tracker = Track.Changes(source, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);
                tracker.Dispose();

                source.Value[0].Value++;
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Value = new ObservableCollection<ComplexType>();
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void ReplaceCollection()
            {
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType>() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false)
                                                      .Value;
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    var observableCollection = source.Value;
                    source.Value = new ObservableCollection<ComplexType> { new ComplexType() };
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new PropertyChangeEventArgs(source, source.GetProperty("Value"))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value[0].Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                0,
                                RootChangeEventArgs.Create(
                                    ChangeTrackerNode.GetOrCreate(source.Value[0], tracker.Settings, isRoot: false)
                                                     .Value,
                                    new PropertyChangeEventArgs(source.Value[0], source.Value[0].GetProperty("Value"))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    observableCollection.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void TracksCollectionProperty()
            {
                var source = new Level
                {
                    Next = new Level
                    {
                        Levels = new ObservableCollection<Level>(new[] { new Level() }),
                    },
                };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false)
                                                      .Value;
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Next.Levels[0].Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Next"),
                            new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, isRoot: false)
                                                 .Value,
                                source.GetProperty("Levels"),
                                new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                    ChangeTrackerNode.GetOrCreate(
                                        (INotifyCollectionChanged)source.Next.Levels,
                                        tracker.Settings,
                                        isRoot: false)
                                                     .Value,
                                    0,
                                    RootChangeEventArgs.Create(
                                        ChangeTrackerNode.GetOrCreate(source.Next.Levels[0], tracker.Settings, isRoot: false)
                                                         .Value,
                                        new PropertyChangeEventArgs(source.Next.Levels[0], source.Next.Levels[0].GetProperty("Value")))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void Add()
            {
                var source = new With<ObservableCollection<ComplexType>>
                {
                    Value = new ObservableCollection<ComplexType>(),
                };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false)
                                                      .Value;
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new AddEventArgs(source.Value, 0))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value.Add(new ComplexType());
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new AddEventArgs(source.Value, 1))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value[0].Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                0,
                                RootChangeEventArgs.Create(
                                    ChangeTrackerNode.GetOrCreate(source.Value[0], tracker.Settings, isRoot: false)
                                                     .Value,
                                    new PropertyChangeEventArgs(source.Value[0], source.Value[0].GetProperty("Value"))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value[0].Value++;
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                0,
                                RootChangeEventArgs.Create(
                                    ChangeTrackerNode.GetOrCreate(source.Value[0], tracker.Settings, isRoot: false)
                                                     .Value,
                                    new PropertyChangeEventArgs(source.Value[0], source.Value[0].GetProperty("Value"))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value[1].Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(
                        new[] { "Changes", "Changes", "Changes", "Changes", "Changes" },
                        propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                1,
                                RootChangeEventArgs.Create(
                                    ChangeTrackerNode.GetOrCreate(source.Value[1], tracker.Settings, isRoot: false)
                                                     .Value,
                                    new PropertyChangeEventArgs(source.Value[1], source.Value[1].GetProperty("Value"))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void Remove()
            {
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType> { new ComplexType(), null } };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false)
                                                      .Value;
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new RemoveEventArgs(source.Value, 1))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    var complexType = source.Value[0];
                    source.Value.RemoveAt(0);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new RemoveEventArgs(source.Value, 0))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    complexType.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void Clear()
            {
                var source = new With<ObservableCollection<ComplexType>> { Value = new ObservableCollection<ComplexType> { new ComplexType(), null } };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false)
                                                      .Value;
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new ResetEventArgs(source.Value))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new ResetEventArgs(source.Value))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void Replace()
            {
                var source = new With<ObservableCollection<ComplexType>>
                {
                    Value = new ObservableCollection<ComplexType> { new ComplexType(), new ComplexType() },
                };

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expectedChanges = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false)
                                                      .Value;
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value[0] = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                new ReplaceEventArgs(source.Value, 0))));

                    source.Value[0].Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Value"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Value,
                                    tracker.Settings,
                                    isRoot: false)
                                                 .Value,
                                0,
                                RootChangeEventArgs.Create(
                                    ChangeTrackerNode.GetOrCreate(source.Value[0], tracker.Settings, isRoot: false)
                                                     .Value,
                                    new PropertyChangeEventArgs(source.Value[0], source.Value[0].GetProperty("Value"))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }
        }
    }
}