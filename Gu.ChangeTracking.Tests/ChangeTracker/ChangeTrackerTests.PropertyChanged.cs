namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class PropertyChanged
        {
            [Test]
            public void NotifiesOnCurrentLevelAndStopsOnDisposed()
            {
                var changes = new List<object>();
                var root = new Level();
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    tracker.Dispose();
                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void WithImmutable()
            {
                var changes = new List<object>();
                var root = new WithImmutable();

                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Immutable = new Immutable();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void NotifiesNextLevel()
            {
                var changes = new List<object>();
                var level = new Level { Next = new Level() };
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    level.Next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void NotifiesThreeLevels()
            {
                var changes = new List<object>();
                var level = new Level { Next = new Level { Next = new Level() } };
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    level.Next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    level.Next.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void TracksCollectionItem()
            {
                var changes = new List<object>();
                var root = new Level { Next = new Level { Levels = new ObservableCollection<Level>(new[] { new Level(), }) } };
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Next.Levels[0].Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void StartSubscribingToNextLevel()
            {
                var changes = new List<object>();
                var level = new Level();
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    level.Next = new Level();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    level.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);
                }
            }

            [Test]
            public void StopsSubscribingNextLevel()
            {
                var changes = new List<object>();
                var level = new Level { Next = new Level() };
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var next = level.Next;
                    level.Next = null;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}
