// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class Ignores
        {
            [Test]
            public void IgnoresProperty()
            {
                var withIllegalObject = new WithIllegal();
                var propertyInfo = typeof(WithIllegal).GetProperty(nameof(WithIllegal.Illegal));
                var settings = new PropertiesSettingsBuilder().IgnoreProperty(propertyInfo)
                                                              .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] {"Changes"}, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(withIllegalObject, tracker.Settings, false).Value, new PropertyChangeEventArgs(withIllegalObject.GetType().GetProperty(nameof(withIllegalObject.Value)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    withIllegalObject.Illegal = new IllegalType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void IgnoresPropertyLambda()
            {
                var withIllegalObject = new WithIllegal();
                var settings = new PropertiesSettingsBuilder().IgnoreProperty<WithIllegal>(x => x.Illegal)
                                                              .CreateSettings(ReferenceHandling.Structural);
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(withIllegalObject, tracker.Settings, false).Value, new PropertyChangeEventArgs(withIllegalObject.GetType().GetProperty(nameof(withIllegalObject.Value)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    withIllegalObject.Illegal = new IllegalType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void IgnoresBaseClassPropertyLambda()
            {
                var root = new DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<ComplexType>(x => x.Excluded)
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(root, tracker.Settings, false).Value, new PropertyChangeEventArgs(root.GetType().GetProperty(nameof(root.Value)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    root.Excluded++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void IgnoresInterfacePropertyLambda()
            {
                var root = new DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<IBaseClass>(x => x.Excluded)
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(root, tracker.Settings, false).Value, new PropertyChangeEventArgs(root.GetType().GetProperty(nameof(root.Value)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                    root.Excluded++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }
            }

            [Test]
            public void IgnoresType()
            {
                var root = new WithIllegal();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<IllegalType>()
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void IgnoresTypeProperty()
            {
                var root = new WithIllegal();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<IllegalType>()
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using (var tracker = Track.Changes(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);
                }
            }
        }
    }
}
