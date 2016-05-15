namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class ObservableCollectionOfInts
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void CreateAndDispose(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int>();
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Add(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var node = ChangeTrackerNode.GetOrCreate((INotifyCollectionChanged)source, tracker.Settings, false).Value;
                    var expected = new[] { RootChangeEventArgs.Create(node, new AddEventArgs(0)) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    source.Add(2);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    expected = new[]
                                   {
                                       RootChangeEventArgs.Create(node, new AddEventArgs(0)),
                                       RootChangeEventArgs.Create(node, new AddEventArgs(1))
                                   };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    tracker.Dispose();

                    source.Add(3);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }

                source.Add(4);
                CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, propertyChanges);
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Add(ReferenceHandling referenceHandling)
            {
                var changes = new List<object>();
                var source = new ObservableCollection<int>();
                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Add(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.Add(2);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Remove(ReferenceHandling referenceHandling)
            {
                var changes = new List<object>();
                var source = new ObservableCollection<int> { 1, 2 };
                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.RemoveAt(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.RemoveAt(0);
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Clear(ReferenceHandling referenceHandling)
            {
                var changes = new List<object>();
                var source = new ObservableCollection<int> { 1, 2 };
                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source.Clear();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    source.Clear();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Replace(ReferenceHandling referenceHandling)
            {
                var changes = new List<object>();
                var source = new ObservableCollection<int> { 1, 2 };
                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    source[0] = 3;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Move(ReferenceHandling referenceHandling)
            {
                var changes = new List<object>();
                var source = new ObservableCollection<int> { 1, 2 };
                using (var tracker = Track.Changes(source, referenceHandling))
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
        }
    }
}