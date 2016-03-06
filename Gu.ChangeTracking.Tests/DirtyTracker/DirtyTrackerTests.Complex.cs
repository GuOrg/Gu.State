namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Complex
        {
            [Test]
            public void ThrowsWithoutReferenceHandling()
            {
                var expected = "EqualBy.PropertyValues(x, y) failed.\r\n" +
                               "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                               "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                               "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                               "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                               "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property WithComplexProperty.ComplexType.\r\n" +
                               "    - The type ComplexType.\r\n";
                var x = new DirtyTrackerTypes.WithComplexProperty();
                var y = new DirtyTrackerTypes.WithComplexProperty();

                var exception = Assert.Throws<NotSupportedException>(() => DirtyTracker.Track(x, y));

                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void WithComplexPropertyTracks()
            {
                var x = new DirtyTrackerTypes.WithComplexProperty();
                var y = new DirtyTrackerTypes.WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = DirtyTracker.Track(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    CollectionAssert.IsEmpty(tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Name = "newName1";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.Name)) }, tracker.Diff);

                    y.Name = "newName1";
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ComplexType = new DirtyTrackerTypes.ComplexType("a", 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType = new DirtyTrackerTypes.ComplexType("a", 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);

                    x.ComplexType.Name = "newName2";
                    Assert.AreEqual(true, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    x.ComplexType.Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType.Name = "newName2";
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.AreEqual(new[] { typeof(DirtyTrackerTypes.WithComplexProperty).GetProperty(nameof(x.ComplexType)) }, tracker.Diff);

                    y.ComplexType.Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    expectedChanges.AddRange(new[] { "IsDirty", "Diff" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                    CollectionAssert.IsEmpty(tracker.Diff);
                }
            }
        }
    }
}
