namespace Gu.State.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        public class Simple
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void CreateAndDispose(ReferenceHandling referenceHandling)
            {
                var source = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var propertyChanges = new List<string>();
                var changes = new List<EventArgs>();

                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    Assert.AreEqual(0, tracker.Changes);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value1++;
                    Assert.AreEqual(1, tracker.Changes);
                    CollectionAssert.AreEqual(new[] { "Changes" }, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value1)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }

                source.Value1++;
                Assert.AreEqual(1, propertyChanges.Count);
                Assert.AreEqual(1, changes.Count);
#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                var wrx = new System.WeakReference(source);
                source = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
#endif
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void CreateAndDisposeExplicitSetting(ReferenceHandling referenceHandling)
            {
                var source = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var propertyChanges = new List<string>();
                var expectedPropertyChanges = new List<string>();
                var changes = new List<EventArgs>();

                using (var tracker = Track.Changes(source, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => propertyChanges.Add(e.PropertyName);
                    tracker.Changed += (_, e) => changes.Add(e);
                    CollectionAssert.IsEmpty(propertyChanges);
                    CollectionAssert.IsEmpty(changes);

                    source.Value1++;
                    Assert.AreEqual(1, tracker.Changes);
                    expectedPropertyChanges.AddRange(new[] { "Changes" });
                    CollectionAssert.AreEqual(expectedPropertyChanges, propertyChanges);
                    var expected = new[] { RootChangeEventArgs.Create(ChangeTrackerNode.GetOrCreate(source, tracker.Settings, false).Value, new PropertyChangeEventArgs(source, source.GetProperty(nameof(source.Value1)))) };
                    CollectionAssert.AreEqual(expected, changes, EventArgsComparer.Default);
                }

                source.Value1++;
                CollectionAssert.AreEqual(expectedPropertyChanges, propertyChanges);

#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                var wrx = new System.WeakReference(source);
                source = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
#endif
            }
        }
    }
}