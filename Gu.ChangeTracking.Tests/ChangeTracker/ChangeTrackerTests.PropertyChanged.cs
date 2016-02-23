namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

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

            [Test]
            public void IgnoresProperty()
            {
                var changes = new List<object>();
                var withIllegalObject = new WithIllegalObject();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty(typeof(WithIllegalObject).GetProperty(nameof(WithIllegalObject.Illegal)));
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Illegal = new IllegalObject();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresPropertyLambda()
            {
                var changes = new List<object>();
                var withIllegalObject = new WithIllegalObject();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty<WithIllegalObject>(x => x.Illegal);
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Illegal = new IllegalObject();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresBaseClassPropertyLambda()
            {
                var changes = new List<object>();
                var root = new DerivedClass();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty<ComplexType>(x => x.Excluded);
                using (var tracker = ChangeTracker.Track(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Excluded++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresInterfacePropertyLambda()
            {
                var changes = new List<object>();
                var root = new DerivedClass();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty<IBaseClass>(x => x.Excluded);
                using (var tracker = ChangeTracker.Track(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Excluded++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresAttributedProperty()
            {
                var changes = new List<object>();
                var withIllegalObject = new WithIgnoredProperty();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty(typeof(WithIgnoredProperty).GetProperty(nameof(WithIgnoredProperty.Ignored)));
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Ignored++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresType()
            {
                var changes = new List<object>();
                var withIllegalObject = new WithIllegalObject();
                var settings = new ChangeTrackerSettings();
                settings.AddImmutableType<IllegalObject>();
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresTypeProperty()
            {
                var changes = new List<object>();
                var root = new WithLevel();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitType<Level>();
                using (var tracker = ChangeTracker.Track(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Level = new Level();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    root.Level.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}
