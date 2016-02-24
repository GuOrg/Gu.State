namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class Ignores
        {
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
            public void IgnoresImmutableType()
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
