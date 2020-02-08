namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class WithComplex
        {
            [Test]
            public void CreateAndDisposeStopsListeningToSubProperties()
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var changes = new List<string>();
                var settings = PropertiesSettings.GetOrCreate();
                using (var tracker = Track.IsDirty(x, y, settings))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                }

                x.ComplexType.Value++;
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public void HandlesNullStructural()
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                var expected = "WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null";
                var actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);

                y.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);

                x.ComplexType = null;
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                expected = "WithComplexProperty ComplexType x: null y: Gu.State.Tests.DirtyTrackerTypes+ComplexType";
                actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);

                y.ComplexType = null;
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);
            }

            [Test]
            public void HandlesNullReferences()
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.References);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                var expected = "WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null";
                var actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);

                y.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: Gu.State.Tests.DirtyTrackerTypes+ComplexType", tracker.Diff.ToString(string.Empty, " "));

                x.ComplexType = null;
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                expected = "WithComplexProperty ComplexType x: null y: Gu.State.Tests.DirtyTrackerTypes+ComplexType";
                actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);

                y.ComplexType = null;
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);
            }

            [TestCase(ReferenceHandling.Structural)]
            public void TracksNested(ReferenceHandling referenceHandling)
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, referenceHandling);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Name = "newName1";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty Name x: newName1 y: null", tracker.Diff.ToString(string.Empty, " "));

                y.Name = "newName1";
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);

                x.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);

                x.ComplexType.Name = "newName2";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Name x: newName2 y: a", tracker.Diff.ToString(string.Empty, " "));

                x.ComplexType.Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Name x: newName2 y: a Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType.Name = "newName2";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType.Value++;
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);
            }

            [Test]
            public void TracksNestedWithExplicitSetting()
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, PropertiesSettings.GetOrCreate());
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Name = "newName1";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty Name x: newName1 y: null", tracker.Diff.ToString(string.Empty, " "));

                y.Name = "newName1";
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);

                x.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType = new ComplexType("a", 1);
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);

                x.ComplexType.Name = "newName2";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Name x: newName2 y: a", tracker.Diff.ToString(string.Empty, " "));

                x.ComplexType.Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Name x: newName2 y: a Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType.Name = "newName2";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType.Value++;
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);
            }

            [TestCase(ReferenceHandling.Structural)]
            public void WhenNestedNameChanged(ReferenceHandling referenceHandling)
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, referenceHandling);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.ComplexType.Name = "changed";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty ComplexType Name x: changed y: a", tracker.Diff.ToString(string.Empty, " "));

                y.ComplexType.Name = x.ComplexType.Name;
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);
            }

            [TestCase(ReferenceHandling.Structural)]
            public void WhenRootNameChanged(ReferenceHandling referenceHandling)
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, referenceHandling);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Name = "changed";
                Assert.AreEqual(true, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual("WithComplexProperty Name x: changed y: null", tracker.Diff.ToString(string.Empty, " "));

                y.Name = x.Name;
                Assert.AreEqual(false, tracker.IsDirty);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
                Assert.AreEqual(null, tracker.Diff);
            }
        }
    }
}
