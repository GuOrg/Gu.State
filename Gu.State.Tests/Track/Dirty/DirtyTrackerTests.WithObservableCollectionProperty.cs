namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class WithObservableCollectionProperty
        {
            private static readonly PropertyInfo ComplexesProperty = typeof(WithObservableCollectionProperties).GetProperty(nameof(WithObservableCollectionProperties.Complexes));

            [Test]
            public void AddSameToBoth()
            {
                var x = new WithObservableCollectionProperties();
                var y = new WithObservableCollectionProperties();

                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithObservableCollectionProperties Complexes [0] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithObservableCollectionProperties Complexes [0] Value x: 2 y: 1", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void AddDifferent()
            {
                var x = new WithObservableCollectionProperties();
                var y = new WithObservableCollectionProperties();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithObservableCollectionProperties Complexes [0] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: null", tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new ComplexType("b", 2));
                    Assert.AreEqual(true, tracker.IsDirty);
                    Assert.AreEqual("WithObservableCollectionProperties Complexes [0] Name x: a y: b Value x: 1 y: 2", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Name = "b";
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual("WithObservableCollectionProperties Complexes [0] Value x: 1 y: 2", tracker.Diff.ToString("", " "));
                    expectedChanges.Add("Diff");
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value = 1;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveTheDifference()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.RemoveAt(1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void RemoveStillDirty()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("c", 3));
                var changes = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.RemoveAt(1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);
                }
            }

            [Test]
            public void ClearBothWhenNotDirty()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void ClearBothWhenDirty()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("c", 2), new ComplexType("d", 4), new ComplexType("e", 5));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Clear();
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    CollectionAssert.IsEmpty(changes);

                    y.Complexes.Clear();
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveX()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Move(0, 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes.Move(0, 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void MoveXThenY()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Move(0, 1);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Move(0, 1);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void Replace()
            {
                var x = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var y = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes[0] = new ComplexType("c", 3);
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0] = new ComplexType("c", 3);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }

            [Test]
            public void TracksItems()
            {
                var x = new WithObservableCollectionProperties();
                var y = new WithObservableCollectionProperties();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling: ReferenceHandling.Structural))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.IsEmpty(changes);

                    x.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes.Add(new ComplexType("a", 1));
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    x.Complexes[0].Value++;
                    Assert.AreEqual(true, tracker.IsDirty);
                    CollectionAssert.AreEqual(new[] { ComplexesProperty }, tracker.Diff.ToString("", " "));
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    y.Complexes[0].Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    expectedChanges.AddRange(new[] {  "Diff", "IsDirty" });
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    var complexType = y.Complexes[0];
                    y.Complexes[0] = new ComplexType(complexType.Name, complexType.Value);
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);

                    complexType.Value++;
                    Assert.AreEqual(false, tracker.IsDirty);
                    Assert.AreEqual(null, tracker.Diff);
                    CollectionAssert.AreEqual(expectedChanges, changes);
                }
            }
        }
    }
}
