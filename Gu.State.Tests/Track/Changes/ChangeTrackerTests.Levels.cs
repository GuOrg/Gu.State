namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class Levels
        {
            [Test]
            public void NotifiesOnCurrentLevelAndStopsOnDisposed()
            {
                var changes = new List<object>();
                var root = new Level();
                using (var tracker = Track.Changes(root, ReferenceHandling.Structural))
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
            public void NotifiesNextLevel()
            {
                var changes = new List<object>();
                var level = new Level { Next = new Level() };
                using (var tracker = Track.Changes(level, ReferenceHandling.Structural))
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
            public void NotifiesOnAdd()
            {
                var changes = new List<object>();
                var root = new Level();
                using (var tracker = Track.Changes(root, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Levels.Add(level);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    root.Levels.Add(new Level());
                    Assert.AreEqual(4, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(4), changes);

                    root.Levels = new ObservableCollection<Level>();
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    tracker.Dispose();

                    level.Value++;
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);

                    root.Levels.Add(new Level());
                    Assert.AreEqual(5, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(5), changes);
                }
            }

            [Test]
            public void NotifiesOnAddSpecialCollection()
            {
                var changes = new List<object>();
                var root = new SpecialCollection();
                using (var tracker = Track.Changes(root, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Remove(level);
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    level.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);
                }
            }

            [Test]
            public void TracksAddedStopsOnRemoved()
            {
                var changes = new List<object>();
                var root = new Level();
                using (var tracker = Track.Changes(root, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    level.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    root.Levels.Clear();
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);

                    level.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);
                }
            }


            [Test]
            public void NotifiesThreeLevels()
            {
                var changes = new List<object>();
                var level = new Level { Next = new Level { Next = new Level() } };
                using (var tracker = Track.Changes(level, ReferenceHandling.Structural))
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
            public void NotifiesThreeLevels2()
            {
                var changes = new List<object>();
                var root = new Level { Next = new Level { Next = new Level() } };
                using (var tracker = Track.Changes(root, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    var level = new Level();
                    root.Levels.Add(level);
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    level.Next = new Level();
                    Assert.AreEqual(2, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(2), changes);

                    level.Next.Value++;
                    Assert.AreEqual(3, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(3), changes);
                }
            }
        }
    }
}