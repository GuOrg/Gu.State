// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class ReferenceLoops
        {
            [Test]
            public void WithSelf()
            {
                var source = new WithSelf();
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

                    source.Value = source;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    expectedChanges.Add(RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value)))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);

                    source.Name += "abc";
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expectedChanges.Add(RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Name)))));
                    CollectionAssert.AreEqual(expectedChanges, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void CreateAndDisposeParentChildLoop()
            {
                var parent = new Parent { Child = new Child("c") };
                parent.Child.Parent = parent;
                Assert.AreSame(parent, parent.Child.Parent);
                Assert.AreSame(parent.Child, parent.Child.Parent.Child);
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var tracker = Track.Changes(parent, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                tracker.Dispose();
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                parent.Name += "abc";
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                parent.Child.Name += "abc";
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void SequenceOfChanges()
            {
                var parent = new Parent { Child = new Child("c") };

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                var expected = new List<EventArgs>();
                using (var tracker = Track.Changes(parent, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    parent.Name = "Poppa";
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var parentNode = ChangeTrackerNode.GetOrCreate(parent, tracker.Settings, isRoot: false).Value;
                    expected.Add(RootChangeEventArgs.Create(parentNode, new PropertyChangeEventArgs(parent, parent.GetProperty("Name"))));
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    parent.Child = new Child("Child");
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expected.Add(RootChangeEventArgs.Create(parentNode, new PropertyChangeEventArgs(parent, parent.GetProperty("Child"))));
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    parent.Child.Parent = parent;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, propertyChanges);
                    expected.Add(new PropertyGraphChangedEventArgs<ChangeTrackerNode>(parentNode, parent.GetProperty("Child"), RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(parent.Child, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(parent.Child, parent.Child.GetProperty("Parent")))));
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    parent.Name += "meh";
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes" }, propertyChanges);
                    expected.Add(RootChangeEventArgs.Create(parentNode, new PropertyChangeEventArgs(parent, parent.GetProperty("Name"))));
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void ParentChanges()
            {
                var parent = new Parent { Child = new Child("c") };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(parent, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    parent.Child.Parent = parent;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var rootChangeEventArgs = RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(parent.Child, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(parent.Child, parent.Child.GetProperty("Parent")));
                    var expected = new[] { new PropertyGraphChangedEventArgs<ChangeTrackerNode>(ChangeTrackerNode.GetOrCreate(parent, tracker.Settings, isRoot: false).Value, parent.GetProperty("Child"), rootChangeEventArgs), };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void ParentNameChanges()
            {
                var parent = new Parent { Child = new Child("") };
                parent.Child.Parent = parent;
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(parent, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    parent.Name += "abc";
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(parent, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(parent, parent.GetProperty(nameof(parent.Name)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void ChildNameChanges()
            {
                var parent = new Parent { Child = new Child("") };
                parent.Child.Parent = parent;
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(parent, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    parent.Child.Name += "abc";
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var rootChangeEventArgs = RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(parent.Child, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(parent.Child, parent.Child.GetProperty(nameof(Child.Name))));
                    var expected = new[] { new PropertyGraphChangedEventArgs<ChangeTrackerNode>(ChangeTrackerNode.GetOrCreate(parent, tracker.Settings, isRoot: false).Value, parent.GetType().GetProperty("Child"), rootChangeEventArgs) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }
        }
    }
}