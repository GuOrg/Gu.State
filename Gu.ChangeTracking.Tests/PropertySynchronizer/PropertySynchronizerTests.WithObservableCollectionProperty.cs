namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;

    using Gu.ChangeTracking.Tests.PropertySynchronizerStubs;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class WithObservableCollectionProperty
        {
            [Test]
            public void CreateAndDisposeStructural()
            {
                var source = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var target = new WithObservableCollectionProperties();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);

                    source.Complexes[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                    Assert.AreNotSame(source.Complexes[0], target.Complexes[0]);
                    Assert.AreNotSame(source.Complexes[1], target.Complexes[1]);
                }

                source.Complexes.Add(new ComplexType("c", 3));
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2), new ComplexType("c", 3) },
                    source.Complexes,
                    ComplexType.Comparer);
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2) },
                    target.Complexes,
                    ComplexType.Comparer);

                source.Complexes[0].Value++;
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 3), new ComplexType("b", 2), new ComplexType("c", 3) },
                    source.Complexes,
                    ComplexType.Comparer);
                CollectionAssert.AreEqual(
                    new[] { new ComplexType("a", 2), new ComplexType("b", 2) },
                    target.Complexes,
                    ComplexType.Comparer);
            }

            [TestCase(ReferenceHandling.Structural)]
            //[TestCase(ReferenceHandling.Reference)]
            public void Add(ReferenceHandling referenceHandling)
            {
                var source = new WithObservableCollectionProperties();
                var target = new WithObservableCollectionProperties();
                using (PropertySynchronizer.Create(source, target, referenceHandling))
                {
                    source.Complexes.Add(new ComplexType("a", 1));
                    var expected = new[] { new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);

                    source.Complexes[0].Value++;
                    expected = new[] { new ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            //[TestCase(ReferenceHandling.Reference)]
            public void Remove(ReferenceHandling referenceHandling)
            {
                var source = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var target = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                using (PropertySynchronizer.Create(source, target, referenceHandling))
                {
                    source.Complexes.RemoveAt(1);
                    var expected = new[] { new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                    source.Complexes.RemoveAt(0);
                    CollectionAssert.IsEmpty(source.Complexes);
                    CollectionAssert.IsEmpty(target.Complexes);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            //[TestCase(ReferenceHandling.Reference)]
            public void Insert(ReferenceHandling referenceHandling)
            {
                var source = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var target = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                using (PropertySynchronizer.Create(source, target, referenceHandling))
                {
                    source.Complexes.Insert(1, new ComplexType("c", 3));
                    var expected = new[] { new ComplexType("a", 1), new ComplexType("c", 3), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);

                    source.Complexes[0].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("c", 3), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);

                    source.Complexes[1].Value++;
                    expected = new[] { new ComplexType("a", 2), new ComplexType("c", 4), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            //[TestCase(ReferenceHandling.Reference)]
            public void Move(ReferenceHandling referenceHandling)
            {
                var source = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var target = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                using (PropertySynchronizer.Create(source, target, referenceHandling))
                {
                    source.Complexes.Move(1, 0);
                    var expected = new[] { new ComplexType("b", 2), new ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);

                    source.Complexes.Move(0, 1);
                    expected = new[] { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            //[TestCase(ReferenceHandling.Reference)]
            public void Replace(ReferenceHandling referenceHandling)
            {
                var source = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var target = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                using (PropertySynchronizer.Create(source, target, referenceHandling))
                {
                    source.Complexes[0] = new ComplexType("c", 3);
                    var expected = new[] { new ComplexType("c", 3), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);

                    source.Complexes[1] = new ComplexType("d", 4);
                    expected = new[] { new ComplexType("c", 3), new ComplexType("d", 4) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                }
            }

            [Test]
            public void Synchronizes()
            {
                var source = new WithObservableCollectionProperties(new ComplexType("a", 1), new ComplexType("b", 2));
                var target = new WithObservableCollectionProperties();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    var expected = new List<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                    Assert.AreNotSame(source.Complexes[0], target.Complexes[0]);
                    Assert.AreNotSame(source.Complexes[1], target.Complexes[1]);

                    source.Complexes.Add(new ComplexType("c", 3));
                    expected.Add(new ComplexType("c", 3));
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                    Assert.AreNotSame(source.Complexes[0], target.Complexes[0]);
                    Assert.AreNotSame(source.Complexes[1], target.Complexes[1]);
                    Assert.AreNotSame(source.Complexes[2], target.Complexes[2]);

                    source.Complexes[2].Name = "changed";
                    expected[2].Name = "changed";
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                    Assert.AreNotSame(source.Complexes[0], target.Complexes[0]);
                    Assert.AreNotSame(source.Complexes[1], target.Complexes[1]);
                    Assert.AreNotSame(source.Complexes[2], target.Complexes[2]);

                    source.Complexes.RemoveAt(1);
                    expected.RemoveAt(1);
                    CollectionAssert.AreEqual(expected, source.Complexes, ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target.Complexes, ComplexType.Comparer);
                    Assert.AreNotSame(source.Complexes[0], target.Complexes[0]);
                    Assert.AreNotSame(source.Complexes[1], target.Complexes[1]);

                    source.Complexes.Clear();
                    Assert.AreEqual(0, source.Complexes.Count);
                    Assert.AreEqual(0, target.Complexes.Count);
                }
            }
        }
    }
}
