// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static SynchronizeTypes;

    public static partial class SynchronizeTests
    {
        public static class ObservableCollectionOfComplexTypes
        {
            [Test]
            public static void CreateAndDisposeStructural()
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.Structural))
                {
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                }

                source.Add(new ComplexType("c", 3));
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 2), new ComplexType("b", 2) }, target, ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 2), new ComplexType("b", 2) }, target, ComplexType.Comparer);

                source[1].Value++;
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 3), new ComplexType("b", 3), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 2), new ComplexType("b", 2) }, target, ComplexType.Comparer);
            }

            [Test]
            public static void CreateAndDisposeStructural1()
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType>();

                using (Synchronize.PropertyValues(source, target, ReferenceHandling.Structural))
                {
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source.Add(source[0]);
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("a", 3) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                }

                source.Add(new ComplexType("c", 3));
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("a", 3), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                var expectedTargets = new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("a", 3) };
                CollectionAssert.AreEqual(expectedTargets, target, ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 4), new ComplexType("b", 2), new ComplexType("a", 4), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(expectedTargets, target, ComplexType.Comparer);

                source[1].Value++;
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 4), new ComplexType("b", 3), new ComplexType("a", 4), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(expectedTargets, target, ComplexType.Comparer);

                source[2].Value++;
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 5), new ComplexType("b", 3), new ComplexType("a", 5), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(expectedTargets, target, ComplexType.Comparer);

                target[0].Value++;
                target.Add(new ComplexType("c", 4));
                expectedTargets = new[] { new ComplexType("a", 4), new ComplexType("b", 2), new ComplexType("a", 3), new ComplexType("c", 4) };
                CollectionAssert.AreEqual(expectedTargets, target, ComplexType.Comparer);
            }

            [Test]
            public static void CreateAndDisposeReference()
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.References))
                {
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[1], target[1]);
                }

                source.Add(new ComplexType("c", 3));
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 2), new ComplexType("b", 2) }, target, ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("c", 3) }, source, ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new ComplexType("a", 3), new ComplexType("b", 2) }, target, ComplexType.Comparer);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Add(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType>();
                var target = new ObservableCollection<ComplexType>();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Add(new ComplexType("a", 1));
                    var expected = new[] { new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void AddThenUpdate1(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType>();
                var target = new ObservableCollection<ComplexType>();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Add(new ComplexType("a", 1));
                    var expected = new[] { new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void AddThenUpdate2(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Add(new ComplexType("c", 3));
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2), new ComplexType("c", 3) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("c", 3) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[1].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 3), new ComplexType("c", 3) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[2].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 3), new ComplexType("c", 4) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Remove(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.RemoveAt(1);
                    var expected = new[] { new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    source.RemoveAt(0);
                    CollectionAssert.IsEmpty(source);
                    CollectionAssert.IsEmpty(target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Insert(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Insert(1, new ComplexType("c", 3));
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("c", 3), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("c", 3), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[1].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("c", 4), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[2].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("c", 4), new ComplexType("b", 3) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Move(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Move(1, 0);
                    var expected = new[] { new ComplexType("b", 2), new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source.Move(0, 1);
                    expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void MoveThenUpdate(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2), new("c", 3) };
                var target = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2), new("c", 3) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Move(2, 0);
                    var expected = new[] { new ComplexType("c", 3), new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new ComplexType("c", 4), new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[1].Value++;
                    expected = new[] { new ComplexType("c", 4), new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[2].Value++;
                    expected = new[] { new ComplexType("c", 4), new ComplexType("a", 2), new ComplexType("b", 3) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Replace(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source[0] = new ComplexType("c", 3);
                    var expected = new[] { new ComplexType("c", 3), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[1] = new ComplexType("d", 4);
                    expected = new[] { new ComplexType("c", 3), new ComplexType("d", 4) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                }
            }

            [Test]
            public static void Synchronizes()
            {
                var source = new ObservableCollection<ComplexType> { new("a", 1), new("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.Structural))
                {
                    var expected = new List<ComplexType> { new("a", 1), new("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source.Add(new ComplexType("c", 3));
                    expected.Add(new ComplexType("c", 3));
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                    Assert.AreNotSame(source[2], target[2]);

                    source[2].Name = "changed";
                    expected[2].Name = "changed";
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                    Assert.AreNotSame(source[2], target[2]);

                    source.RemoveAt(1);
                    expected.RemoveAt(1);
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source.Clear();
                    Assert.AreEqual(0, source.Count);
                    Assert.AreEqual(0, target.Count);
                }
            }
        }
    }
}
