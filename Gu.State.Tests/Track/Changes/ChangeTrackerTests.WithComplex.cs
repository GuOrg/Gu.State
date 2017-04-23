// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class WithComplex
        {
            [Test]
            public void Subscribes()
            {
                var source = new With<ComplexType> { Value = new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();

                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var rootChangeEventArgs = RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Value, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source.Value, source.Value.GetProperty(nameof(source.Value.Value))));
                    var expected = new[]
                                       {
                                            new PropertyGraphChangedEventArgs<ChangeTrackerNode>(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, source.GetProperty(nameof(source.Value)), rootChangeEventArgs)
                                       };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void StartsSubscribing()
            {
                var source = new With<ComplexType>();
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

                    source.Value = new ComplexType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value)))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Value.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    var rootChangeEventArgs = RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source.Value, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source.Value, source.Value.GetProperty(nameof(source.Value.Value))));
                    expectedChanges.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, source.GetProperty(nameof(source.Value)), rootChangeEventArgs));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void StopsSubscribing()
            {
                var source = new With<ComplexType> { Value = new ComplexType() };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();

                using (var tracker = Track.Changes(source, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    var oldValue = source.Value;
                    source.Value = null;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    oldValue.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }
    }
}