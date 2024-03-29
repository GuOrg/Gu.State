// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public static partial class DirtyTrackerTests
    {
        public static class ObservableCollectionOfComplexType
        {
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void CreateAndDispose(ReferenceHandling referenceHandling)
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                using (var tracker = Track.IsDirty(x, y, referenceHandling))
                {
                    tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                    Assert.AreEqual(true, tracker.IsDirty);
                    var expected = "ObservableCollection<ComplexType> [0] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item [1] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item";
                    var actual = tracker.Diff.ToString(string.Empty, " ");
                    Assert.AreEqual(expected, actual);
                    CollectionAssert.IsEmpty(changes);
                }

                x.Add(new ComplexType("c", 3));
                CollectionAssert.IsEmpty(changes);

                x[0].Value++;
                CollectionAssert.IsEmpty(changes);

                x[1].Value++;
                CollectionAssert.IsEmpty(changes);

                x[2].Value++;
                CollectionAssert.IsEmpty(changes);
            }

            [Test]
            public static void AddSameToBoth()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Add(new ComplexType("a", 1));
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y.Add(new ComplexType("a", 1));
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                x[0].Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[0].Value++;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void AddThenUpdate()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                x.Add(new ComplexType("a", 1));
                y.Add(new ComplexType("a", 1));
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);

                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x[0].Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[0].Value++;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void AddDifferent()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Add(new ComplexType("a", 1));
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y.Add(new ComplexType("b", 2));
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: a y: b Value x: 1 y: 2", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);

                x[0].Name = "b";
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Value x: 1 y: 2", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[0].Value = 1;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public static void InsertX(int index)
            {
                var x = new ObservableCollection<ComplexType> { new(), new(), new() };
                var y = new ObservableCollection<ComplexType> { new(), new(), new() };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Insert(index, new ComplexType());
                Assert.AreEqual(true, tracker.IsDirty);
                var expected = "ObservableCollection<ComplexType> [3] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item";
                var actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public static void InsertY(int index)
            {
                var x = new ObservableCollection<ComplexType> { new(), new(), new() };
                var y = new ObservableCollection<ComplexType> { new(), new(), new() };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                y.Insert(index, new ComplexType());
                Assert.AreEqual(true, tracker.IsDirty);
                var expected = "ObservableCollection<ComplexType> [3] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType";
                var actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [TestCase(0)]
            [TestCase(1)]
            [TestCase(2)]
            public static void InsertXThenYThenUpdate(int index)
            {
                var x = new ObservableCollection<ComplexType> { new(), new(), new() };
                var y = new ObservableCollection<ComplexType> { new(), new(), new() };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                x.Insert(index, new ComplexType());
                y.Insert(index, new ComplexType());
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x[index].Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual($"ObservableCollection<ComplexType> [{index}] Value x: 1 y: 0", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[index].Value = x[index].Value;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void RemoveTheDifference()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [1] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item", tracker.Diff.ToString(string.Empty, " "));
                CollectionAssert.IsEmpty(changes);

                x.RemoveAt(1);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void RemoveStillDirty1()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("c", 3) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: a y: c Value x: 1 y: 3 [1] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item", tracker.Diff.ToString(string.Empty, " "));
                CollectionAssert.IsEmpty(changes);

                x.RemoveAt(1);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: a y: c Value x: 1 y: 3", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void RemoveStillDirty2()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 0), new("b", 0), new("c", 0) };
                var y = new ObservableCollection<ComplexType> { new("d", 0), new("e", 0) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: a y: d [1] Name x: b y: e [2] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item", tracker.Diff.ToString(string.Empty, " "));
                CollectionAssert.IsEmpty(changes);

                x.RemoveAt(0);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: b y: d [1] Name x: c y: e", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void ClearBothWhenNotDirty()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Clear();
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType [1] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y.Clear();
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void ClearBothWhenDirty()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("c", 2), new("d", 4), new("e", 5) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(true, tracker.IsDirty);
                var expected = "ObservableCollection<ComplexType> [0] Name x: a y: c Value x: 1 y: 2 [1] Name x: b y: d Value x: 2 y: 4 [2] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType";
                var actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);
                CollectionAssert.IsEmpty(changes);

                x.Clear();
                Assert.AreEqual(true, tracker.IsDirty);
                expected = "ObservableCollection<ComplexType> [0] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType [1] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType [2] x: missing item y: Gu.State.Tests.DirtyTrackerTypes+ComplexType";
                Assert.AreEqual(expected, tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.Add("Diff");
                CollectionAssert.AreEqual(expectedChanges, changes);

                y.Clear();
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void MoveX()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Move(0, 1);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: b y: a Value x: 2 y: 1 [1] Name x: a y: b Value x: 1 y: 2", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                x.Move(0, 1);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void MoveXThenY()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Move(0, 1);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: b y: a Value x: 2 y: 1 [1] Name x: a y: b Value x: 1 y: 2", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y.Move(0, 1);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void MoveXThenYThenUpdate()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                x.Move(0, 1);
                y.Move(0, 1);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x[0].Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Value x: 3 y: 2", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[0].Value = x[0].Value;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void Replace()
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x[0] = new ComplexType("c", 3);
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Name x: c y: a Value x: 3 y: 1", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[0] = new ComplexType("c", 3);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [TestCase(0)]
            [TestCase(1)]
            public static void ReplaceThenUpdate(int index)
            {
                var x = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                x[index] = new ComplexType("c", 3);
                y[index] = new ComplexType("c", 3);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x[index].Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual($"ObservableCollection<ComplexType> [{index}] Value x: 4 y: 3", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[index].Value = x[index].Value;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void TracksItems()
            {
                var x = new ObservableCollection<ComplexType>();
                var y = new ObservableCollection<ComplexType>();
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                x.Add(new ComplexType("a", 1));
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] x: Gu.State.Tests.DirtyTrackerTypes+ComplexType y: missing item", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y.Add(new ComplexType("a", 1));
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                x[0].Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                Assert.AreEqual("ObservableCollection<ComplexType> [0] Value x: 2 y: 1", tracker.Diff.ToString(string.Empty, " "));
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                y[0].Value++;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty" });
                CollectionAssert.AreEqual(expectedChanges, changes);

                var complexType = y[0];
                y[0] = new ComplexType(complexType.Name, complexType.Value);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.AreEqual(expectedChanges, changes);

                complexType.Value++;
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.AreEqual(expectedChanges, changes);
            }

            [Test]
            public static void DuplicateItemOneNotification()
            {
                var item = new ComplexType("a", 1);
                var x = new ObservableCollection<ComplexType> { item, item };
                var y = new ObservableCollection<ComplexType> { new("a", 1), new("a", 1) };
                var changes = new List<string>();
                var expectedChanges = new List<string>();
                using var tracker = Track.IsDirty(x, y, ReferenceHandling.Structural);
                tracker.PropertyChanged += (_, e) => changes.Add(e.PropertyName);
                Assert.AreEqual(false, tracker.IsDirty);
                Assert.AreEqual(null, tracker.Diff);
                CollectionAssert.IsEmpty(changes);

                item.Value++;
                Assert.AreEqual(true, tracker.IsDirty);
                var expected = "ObservableCollection<ComplexType> [0] Value x: 2 y: 1 [1] Value x: 2 y: 1";
                var actual = tracker.Diff.ToString(string.Empty, " ");
                Assert.AreEqual(expected, actual);
                expectedChanges.AddRange(new[] { "Diff", "IsDirty", "Diff" }); // not sure how we want this
                CollectionAssert.AreEqual(expectedChanges, changes);
            }
        }
    }
}
