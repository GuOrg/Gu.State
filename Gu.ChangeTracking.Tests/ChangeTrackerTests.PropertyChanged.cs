namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gu.ChangeTracking.Tests.Helpers;

    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        // ReSharper disable once TestClassNameDoesNotMatchFileNameWarning
        // ReSharper disable once InconsistentNaming
        public class PropertyChanged
        {
            private List<PropertyChangedEventArgs> changes;

            [SetUp]
            public void SetUp()
            {
                this.changes = new List<PropertyChangedEventArgs>();
            }

            [Test]
            public void NotifiesOnCurrentLevelAndStopsOnDisposed()
            {
                var root = new Level();
                using (var tracker = ChangeTracker.Track(root))
                {
                    tracker.PropertyChanged += TrackerOnPropertyChanged;

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);

                    tracker.Dispose();
                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);

                    tracker.PropertyChanged -= TrackerOnPropertyChanged;
                }
            }

            [Test]
            public void NotifiesNextLevel()
            {
                var level = new Level { Next = new Level() };
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += TrackerOnPropertyChanged;

                    level.Next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);

                    tracker.PropertyChanged -= TrackerOnPropertyChanged;
                }
            }

            [Test]
            public void NotifiesThreeLevels()
            {
                var level = new Level { Next = new Level { Next = new Level() } };
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += TrackerOnPropertyChanged;

                    level.Next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);

                    level.Next.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    Assert.AreEqual(2, this.changes.Count);

                    tracker.PropertyChanged -= TrackerOnPropertyChanged;
                }
            }

            [Test]
            public void TracksCollectionItem()
            {
                var root = new Level { Next = new Level { Levels = new ObservableCollection<Level>(new[] { new Level(), }) } };
                using (var tracker = ChangeTracker.Track(root))
                {
                    Assert.AreEqual(0, tracker.Changes);
                    root.Next.Levels[0].Value++;
                    Assert.AreEqual(1, tracker.Changes);
                }
            }

            [Test]
            public void StartSubscribingToNextLevel()
            {
                var level = new Level();
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += TrackerOnPropertyChanged;

                    level.Next = new Level();
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);

                    level.Next.Value++;
                    Assert.AreEqual(2, tracker.Changes);
                    Assert.AreEqual(2, this.changes.Count);

                    tracker.PropertyChanged -= TrackerOnPropertyChanged;
                }
            }

            [Test]
            public void StopsSubscribingNextLevel()
            {
                var level = new Level { Next = new Level() };
                using (var tracker = ChangeTracker.Track(level))
                {
                    tracker.PropertyChanged += TrackerOnPropertyChanged;

                    var next = level.Next;
                    level.Next = null;
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);

                    next.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    Assert.AreEqual(1, this.changes.Count);
                    tracker.PropertyChanged -= TrackerOnPropertyChanged;
                }
            }

            [Test]
            public void IgnoresProperty()
            {
                var withIllegalObject = new WithIllegalObject();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty(typeof(WithIllegalObject).GetProperty(nameof(WithIllegalObject.Illegal)));
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    Assert.AreEqual(0, tracker.Changes);
                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);

                    withIllegalObject.Illegal = new IllegalObject();
                    Assert.AreEqual(1, tracker.Changes);
                }
            }

            [Test]
            public void IgnoresPropertyLambda()
            {
                var withIllegalObject = new WithIllegalObject();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty<WithIllegalObject>(x => x.Illegal);
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    Assert.AreEqual(0, tracker.Changes);
                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);

                    withIllegalObject.Illegal = new IllegalObject();
                    Assert.AreEqual(1, tracker.Changes);
                }
            }

            [Test]
            public void IgnoresAttributedProperty()
            {
                var withIllegalObject = new WithIgnoredProperty();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitProperty(typeof(WithIgnoredProperty).GetProperty(nameof(WithIgnoredProperty.Ignored)));
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    Assert.AreEqual(0, tracker.Changes);
                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);

                    withIllegalObject.Ignored++;
                    Assert.AreEqual(1, tracker.Changes);
                }
            }

            [Test]
            public void IgnoresType()
            {
                var withIllegalObject = new WithIllegalObject();
                var settings = new ChangeTrackerSettings();
                settings.AddImmutableType<IllegalObject>();
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    Assert.AreEqual(0, tracker.Changes);
                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                }
            }

            [Test]
            public void IgnoresTypeProperty()
            {
                var root = new WithLevel();
                var settings = new ChangeTrackerSettings();
                settings.AddExplicitType<Level>();
                using (var tracker = ChangeTracker.Track(root, settings))
                {
                    Assert.AreEqual(0, tracker.Changes);
                    root.Level = new Level();
                    Assert.AreEqual(1, tracker.Changes);

                    root.Level.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                }
            }

            private void TrackerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                this.changes.Add(e);
            }
        }
    }
}
