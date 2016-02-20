namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.ChangeTracking.Tests.PropertySynchronizerStubs;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class ObservableCollectionOfComplexTypes
        {
            [Test]
            public void CreateAndDisposeStructural()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                }

                source.Add(new ComplexType("c", 3));
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("c", 3) },
                    source,
                    ComplexType.Comparer);
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2) },
                    target,
                    ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("c", 3) },
                    source,
                    ComplexType.Comparer);
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2) },
                    target,
                    ComplexType.Comparer);
            }

            [Test]
            public void CreateAndDisposeReference()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Reference))
                {
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[1], target[1]);
                }

                source.Add(new ComplexType("c", 3));
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("c", 3) },
                    source,
                    ComplexType.Comparer);
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2) },
                    target,
                    ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("c", 3) },
                    source,
                    ComplexType.Comparer);
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 3), new ComplexType("b", 2) },
                    target,
                    ComplexType.Comparer);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.Reference)]
            public void Add(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType>();
                var target = new ObservableCollection<ComplexType>();
                using (PropertySynchronizer.Create(source, target, referenceHandling))
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
            [TestCase(ReferenceHandling.Reference)]
            public void Remove(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                using (PropertySynchronizer.Create(source, target, referenceHandling))
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
            [TestCase(ReferenceHandling.Reference)]
            public void Insert(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                using (PropertySynchronizer.Create(source, target, referenceHandling))
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
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.Reference)]
            public void Move(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                using (PropertySynchronizer.Create(source, target, referenceHandling))
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
            [TestCase(ReferenceHandling.Reference)]
            public void Replace(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                using (PropertySynchronizer.Create(source, target, referenceHandling))
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
            public void Synchronizes()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    var expected = new List<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
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

                    //target.RemoveAt(0);
                    //Assert.AreEqual(2, source.Count);
                    //Assert.AreEqual(1, target.Count);
                    //Assert.AreEqual("a", source[0].Name);
                    //Assert.AreEqual("changed", target[0].Name);
                    //Assert.AreEqual(1, source[0].Value);
                    //Assert.AreEqual(3, target[0].Value);

                    //Assert.AreEqual("changed", source[1].Name);
                    //Assert.AreEqual(3, source[1].Value);

                    //Assert.Inconclusive("Not sure how to handle the situation where target changes. Maybe throw but not very elegant");
                }
            }

            [Test]
            public void WithObservableCollectionProperty()
            {
                var source = new WithObservableCollectionProperty("a", 1);
                source.Complexes.Add(new ComplexType("a.1", 11));
                source.Ints.Add(1);
                var target = new WithObservableCollectionProperty("b", 2);
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    CollectionAssert.AreEqual(new[] { 1 }, source.Ints);
                    CollectionAssert.AreEqual(new[] { 1 }, target.Ints);
                    var expectedComplexes = new List<ComplexType> { new ComplexType("a.1", 11) };

                    CollectionAssert.AreEqual(expectedComplexes, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expectedComplexes, target.Complexes, ComplexType.Comparer);

                    source.Complexes.Add(new ComplexType("c", 3));
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    expectedComplexes.Add(new ComplexType("c", 3));
                    CollectionAssert.AreEqual(expectedComplexes, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expectedComplexes, target.Complexes, ComplexType.Comparer);

                    source.Complexes[1].Name = "changed";
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    expectedComplexes[1].Name = "changed";
                    CollectionAssert.AreEqual(expectedComplexes, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expectedComplexes, target.Complexes, ComplexType.Comparer);

                    source.Complexes.RemoveAt(1);
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    expectedComplexes.RemoveAt(1);
                    CollectionAssert.AreEqual(expectedComplexes, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expectedComplexes, target.Complexes, ComplexType.Comparer);

                    source.Complexes.Clear();
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    CollectionAssert.IsEmpty(source.Complexes);
                    CollectionAssert.IsEmpty(target.Complexes);
                }
            }
        }
    }
}
