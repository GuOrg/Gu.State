namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Complex
        {
            [Test]
            public void ThrowsWithoutReferenceHandling()
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var exception = Assert.Throws<NotSupportedException>(() => DirtyTracker.Track(x, y));
                var expectedMessage = "Only equatable properties are supported without specifying ReferenceHandling\r\n" +
                                      "Property WithComplexProperty.ComplexType is not IEquatable<ComplexType>.\r\n" +
                                      "Use the overload DirtyTracker.Track(x, y, ReferenceHandling) if you want to track a graph";
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            [Test]
            public void WithComplexPropertyTracks()
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "newName";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(WithComplexProperty).GetProperty(nameof(x.Name)) }, tracker.Diff);

                    y.Name = "newName";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ComplexType = new ComplexType("a", 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType = new ComplexType("a", 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ComplexType.Name = "newName";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType.Name = "newName";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);
                }
            }
        }
    }
}
