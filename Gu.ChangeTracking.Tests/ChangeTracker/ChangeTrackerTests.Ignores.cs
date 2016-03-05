﻿namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;

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
                var withIllegalObject = new WithIllegal();
                var propertyInfo = typeof(WithIllegal).GetProperty(nameof(WithIllegal.Illegal));
                var settings = new PropertiesSettingsBuilder().IgnoreProperty(propertyInfo)
                                                              .CreateSettings();

                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Illegal = new IllegalType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresPropertyLambda()
            {
                var changes = new List<object>();
                var withIllegalObject = new WithIllegal();
                var settings = new PropertiesSettingsBuilder().IgnoreProperty<WithIllegal>(x => x.Illegal)
                                                              .CreateSettings();
                using (var tracker = ChangeTracker.Track(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Illegal = new IllegalType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresBaseClassPropertyLambda()
            {
                var changes = new List<object>();
                var root = new DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<ComplexType>(x => x.Excluded)
                                                 .CreateSettings();
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
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<IBaseClass>(x => x.Excluded)
                                                 .CreateSettings();
                //settings.IgnoreProperty<IBaseClass>(x => x.Excluded);
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
            public void IgnoresType()
            {
                var changes = new List<object>();
                var withIllegalObject = new WithIllegal();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<IllegalType>()
                                                 .CreateSettings();
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
                var root = new WithIllegal();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<IllegalType>()
                                                 .CreateSettings();
                using (var tracker = ChangeTracker.Track(root, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    root.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }
        }
    }
}
