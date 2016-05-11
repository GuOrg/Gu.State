﻿namespace Gu.State.Tests
{
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
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.Changes(x, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    CollectionAssert.IsEmpty(changes);

                    x.Value1++;
                    Assert.AreEqual(1, tracker.Changes);
                    expectedChanges.AddRange(new[] { "Changes" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                x.Value1++;
                CollectionAssert.AreEqual(expectedChanges, changes);

#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                var wrx = new System.WeakReference(x);
                x = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
#endif
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void CreateAndDisposeExplicitSetting(ReferenceHandling referenceHandling)
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var changes = new List<string>();
                var expectedChanges = new List<string>();

                using (var tracker = Track.Changes(x, PropertiesSettings.GetOrCreate(referenceHandling)))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    CollectionAssert.IsEmpty(changes);

                    x.Value1++;
                    Assert.AreEqual(1, tracker.Changes);
                    expectedChanges.AddRange(new[] { "Changes" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }

                x.Value1++;
                CollectionAssert.AreEqual(expectedChanges, changes);

#if (!DEBUG) // debug build keeps instances alive longer for nicer debugging experience
                var wrx = new System.WeakReference(x);
                x = null;
                System.GC.Collect();
                Assert.AreEqual(false, wrx.IsAlive);
#endif
            }
        }
    }
}