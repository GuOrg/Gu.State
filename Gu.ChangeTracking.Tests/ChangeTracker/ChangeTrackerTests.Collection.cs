namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class Collection
        {
            [Test]
            public void NotifiesOnAddInt()
            {
                var changes = new List<string>();
                var root = new Level();
                using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Ints.Add(1);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, changes);

                    root.Ints = new ObservableCollection<int>();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, changes);

                    root.Ints.Add(1);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, changes);

                    tracker.Dispose();
                    root.Ints.Add(2);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, changes);
                }
            }

            [Test]
            public void NotifiesOnAdd()
            {
                var changes = new List<string>();
                var root = new Level();
                using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, changes);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes" }, changes);

                    root.Levels.Add(level);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes" }, changes);

                    root.Levels.Add(new Level());
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes", "Changes", "Changes", "Changes" }, changes);

                    root.Levels = new ObservableCollection<Level>();
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 5), changes);

                    tracker.Dispose();

                    level.Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 5), changes);

                    root.Levels.Add(new Level());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 5), changes);
                }
            }

            [Test]
            public void NotifiesOnAddSpecialCollection()
            {
                var changes = new List<string>();
                var root = new SpecialCollection();
                using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 1), changes);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 2), changes);

                    root.Remove(level);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 3), changes);

                    level.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 3), changes);
                }
            }

            [Test]
            public void TracksAddedStopsOnRemoved()
            {
                var changes = new List<string>();
                var root = new Level();
                using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 1), changes);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 2), changes);

                    root.Levels.Clear();
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 3), changes);

                    level.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 3), changes);
                }
            }

            [Test]
            public void NotifiesThreeLevels()
            {
                var changes = new List<string>();
                var root = new Level { Next = new Level { Next = new Level() } };
                using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 1), changes);

                    level.Next = new Level();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 2), changes);

                    level.Next.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(Enumerable.Repeat("Changes", 3), changes);
                }
            }
        }
    }
}