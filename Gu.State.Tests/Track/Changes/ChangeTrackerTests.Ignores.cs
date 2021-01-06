// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public static partial class ChangeTrackerTests
    {
        public static class Ignores
        {
            [Test]
            public static void IgnoresProperty()
            {
                var withIllegalObject = new WithIllegal();
                var propertyInfo = typeof(WithIllegal).GetProperty(nameof(WithIllegal.Illegal), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                var settings = new PropertiesSettingsBuilder().IgnoreProperty(propertyInfo)
                                                              .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(withIllegalObject, settings);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                withIllegalObject.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var expected = new[]
                {
                    RootChangeEventArgs.Create(
                        ChangeTrackerNode.GetOrCreate(withIllegalObject, tracker.Settings, isRoot: false).Value,
                        new PropertyChangeEventArgs(withIllegalObject, withIllegalObject.GetType().GetProperty(nameof(withIllegalObject.Value), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))),
                };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                withIllegalObject.Illegal = new IllegalType();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void IgnoresPropertyLambda()
            {
                var source = new WithIllegal();
                var settings = new PropertiesSettingsBuilder().IgnoreProperty<WithIllegal>(x => x.Illegal)
                                                              .CreateSettings(ReferenceHandling.Structural);
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, settings);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value)))) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.Illegal = new IllegalType();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void IgnoresBaseClassPropertyLambda()
            {
                var source = new DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<ComplexType>(x => x.Excluded)
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, settings);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value)))) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.Excluded++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void IgnoresInterfacePropertyLambda()
            {
                var source = new DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<IBaseClass>(x => x.Excluded)
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, settings);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value)))) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.Excluded++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }

            [Test]
            public static void IgnoresType()
            {
                var source = new With<ComplexType>();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<ComplexType>()
                                                 .CreateSettings(ReferenceHandling.Structural);

                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();
                using var tracker = Track.Changes(source, settings);
                tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                tracker.Changed += (_, e) => changes.Add(e);
                Assert.AreEqual(0, tracker.Changes);
                CollectionAssert.IsEmpty(propertyChanges);
                CollectionAssert.IsEmpty(changes);

                source.Value = new ComplexType();
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, isRoot: false).Value, new PropertyChangeEventArgs(source, source.GetProperty("Value"))) };
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);

                source.Value.Value++;
                Assert.AreEqual(1, tracker.Changes);
                CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
            }
        }
    }
}
