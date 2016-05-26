// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class Levels
        {
            [Test]
            public void CreateAndDispose()
            {
                var source = new Level();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);
                }

                source.Value++;
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void NotifiesRootLevel()
            {
                var source = new Level();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.GetProperty(nameof(source.Value)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void NotifiesOneLevel()
            {
                var source = new Level { Next = new Level() };
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

                    source.Next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value;
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, source.GetProperty("Next"), RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.Next.GetProperty("Value")))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, source.GetProperty("Next"), RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.Next.GetProperty("Value")))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void StartsSubscribingOneLevel()
            {
                var source = new Level();
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

                    source.Next = new Level();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value;
                    expectedChanges.Add(RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.GetProperty("Next"))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, source.GetProperty("Next"), RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.Next.GetProperty("Value")))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void StopsSubscribingOneLevel()
            {
                var source = new Level { Next = new Level() };
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

                    var old = source.Next;
                    source.Next = new Level();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value;
                    expectedChanges.Add(RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.GetProperty("Next"))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    old.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, source.GetProperty("Next"), RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.Next.GetProperty("Value")))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void NotifiesTwoLevels()
            {
                var source = new Level { Next = new Level { Next = new Level() } };
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

                    source.Next.Next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value;
                    var rootChangeEventArgs = RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Next.Next, tracker.Settings, false).Value, new PropertyChangeEventArgs(source.Next.Next.GetProperty("Value")));
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, source.GetProperty("Next"), new PropertyGraphChangedEventArgs<ChangeTrackerNode>(ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, false).Value, source.Next.GetProperty("Next"), rootChangeEventArgs)));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Next.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(sourceNode, source.GetProperty("Next"), new PropertyGraphChangedEventArgs<ChangeTrackerNode>(ChangeTrackerNode.GetOrCreate(source.Next, tracker.Settings, false).Value, source.Next.GetProperty("Next"), rootChangeEventArgs)));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void NotifiesAddTwoLevels()
            {
                var source = new Level { Next = new Level { Next = new Level() } };
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

                    var level = new Level();
                    source.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var sourceNode = ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false)
                                                      .Value;
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Levels"),
                            RootChangeEventArgs.Create(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Levels,
                                    tracker.Settings,
                                    false)
                                                 .Value,
                                new AddEventArgs(0))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    level.Next = new Level();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Levels"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(
                                    (INotifyCollectionChanged)source.Levels,
                                    tracker.Settings,
                                    false)
                                                 .Value,
                                0,
                                RootChangeEventArgs.Create(
                                    ChangeTrackerNode.GetOrCreate(level, tracker.Settings, false)
                                                     .Value,
                                    new PropertyChangeEventArgs(level.GetProperty("Next"))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);


                    level.Next.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            source.GetProperty("Levels"),
                            new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source.Levels, tracker.Settings, false)
                                                 .Value,
                                0,
                                new PropertyGraphChangedEventArgs<ChangeTrackerNode>(
                                    ChangeTrackerNode.GetOrCreate(level, tracker.Settings, false)
                                                     .Value,
                                    level.GetProperty("Next"),
                                    RootChangeEventArgs.Create(
                                        ChangeTrackerNode.GetOrCreate(level.Next, tracker.Settings, false)
                                                         .Value,
                                        new PropertyChangeEventArgs(level.Next.GetProperty("Value")))))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void NotifiesOnAddSpecialCollection()
            {
                var source = new SpecialCollection();
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

                    var level = new Level();
                    source.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var sourceNode = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new AddEventArgs(0)));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(
                        new ItemGraphChangedEventArgs<ChangeTrackerNode>(
                            sourceNode,
                            0,
                            new RootChangeEventArgs<ChangeTrackerNode>(
                                ChangeTrackerNode.GetOrCreate(level, tracker.Settings, false)
                                                 .Value,
                                new PropertyChangeEventArgs(level.GetProperty("Value")))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Remove(level);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(RootChangeEventArgs.Create(sourceNode, new RemoveEventArgs(0)));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    level.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }
        }
    }
}