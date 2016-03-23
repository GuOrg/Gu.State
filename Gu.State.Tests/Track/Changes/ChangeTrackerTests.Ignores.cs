namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class Ignores
        {
            [Test]
            public void IgnoresProperty()
            {
                var changes = new List<object>();
                var withIllegalObject = new ChangeTrackerTypes.WithIllegal();
                var propertyInfo = typeof(ChangeTrackerTypes.WithIllegal).GetProperty(nameof(ChangeTrackerTypes.WithIllegal.Illegal));
                var settings = new PropertiesSettingsBuilder().IgnoreProperty(propertyInfo)
                                                              .CreateSettings(ReferenceHandling.Structural);

                using (var tracker = Track.Changes(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Illegal = new ChangeTrackerTypes.IllegalType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresPropertyLambda()
            {
                var changes = new List<object>();
                var withIllegalObject = new ChangeTrackerTypes.WithIllegal();
                var settings = new PropertiesSettingsBuilder().IgnoreProperty<ChangeTrackerTypes.WithIllegal>(x => x.Illegal)
                                                              .CreateSettings(ReferenceHandling.Structural);
                using (var tracker = Track.Changes(withIllegalObject, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(changes);

                    withIllegalObject.Value++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);

                    withIllegalObject.Illegal = new ChangeTrackerTypes.IllegalType();
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(CreateExpectedChangeArgs(1), changes);
                }
            }

            [Test]
            public void IgnoresBaseClassPropertyLambda()
            {
                var changes = new List<object>();
                var root = new ChangeTrackerTypes.DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<ChangeTrackerTypes.ComplexType>(x => x.Excluded)
                                                 .CreateSettings(ReferenceHandling.Structural);
                using (var tracker = Track.Changes(root, settings))
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
                var root = new ChangeTrackerTypes.DerivedClass();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<ChangeTrackerTypes.IBaseClass>(x => x.Excluded)
                                                 .CreateSettings(ReferenceHandling.Structural);
                //settings.IgnoreProperty<IBaseClass>(x => x.Excluded);
                using (var tracker = Track.Changes(root, settings))
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
                var withIllegalObject = new ChangeTrackerTypes.WithIllegal();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<ChangeTrackerTypes.IllegalType>()
                                                 .CreateSettings(ReferenceHandling.Structural);
                using (var tracker = Track.Changes(withIllegalObject, settings))
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
                var root = new ChangeTrackerTypes.WithIllegal();
                var settings = PropertiesSettings.Build()
                                                 .IgnoreType<ChangeTrackerTypes.IllegalType>()
                                                 .CreateSettings(ReferenceHandling.Structural);
                using (var tracker = Track.Changes(root, settings))
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
