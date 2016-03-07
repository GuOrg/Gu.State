namespace Gu.State.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class ObservableCollectionOfComplexTypes
        {
            [Test]
            public void CreateAndDisposeStructural()
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
                {
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                }

                source.Add(new SynchronizerTypes.ComplexType("c", 3));
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("c", 3) }, source, SynchronizerTypes.ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2) }, target, SynchronizerTypes.ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("c", 3) }, source, SynchronizerTypes.ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2) }, target, SynchronizerTypes.ComplexType.Comparer);
            }

            [Test]
            public void CreateAndDisposeStructuralWithReferenceLoops()
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.StructuralWithReferenceLoops))
                {
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source.Add(source[0]);
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 3) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                }

                source.Add(new SynchronizerTypes.ComplexType("c", 3));
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("c", 3) }, source, SynchronizerTypes.ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 3) }, target, SynchronizerTypes.ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 4), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 4), new SynchronizerTypes.ComplexType("c", 3) }, source, SynchronizerTypes.ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 3) }, target, SynchronizerTypes.ComplexType.Comparer);
            }

            [Test]
            public void CreateAndDisposeReference()
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.References))
                {
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[1], target[1]);

                    source[0].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[1], target[1]);
                }

                source.Add(new SynchronizerTypes.ComplexType("c", 3));
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("c", 3) }, source, SynchronizerTypes.ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("b", 2) }, target, SynchronizerTypes.ComplexType.Comparer);

                source[0].Value++;
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("c", 3) }, source, SynchronizerTypes.ComplexType.Comparer);
                CollectionAssert.AreEqual(new[] { new SynchronizerTypes.ComplexType("a", 3), new SynchronizerTypes.ComplexType("b", 2) }, target, SynchronizerTypes.ComplexType.Comparer);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Add(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType>();
                var target = new ObservableCollection<SynchronizerTypes.ComplexType>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.Add(new SynchronizerTypes.ComplexType("a", 1));
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Remove(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.RemoveAt(1);
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    source.RemoveAt(0);
                    CollectionAssert.IsEmpty(source);
                    CollectionAssert.IsEmpty(target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Insert(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.Insert(1, new SynchronizerTypes.ComplexType("c", 3));
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("c", 3), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    source[0].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("c", 3), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    source[1].Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("c", 4), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Move(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.Move(1, 0);
                    var expected = new[] { new SynchronizerTypes.ComplexType("b", 2), new SynchronizerTypes.ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    source.Move(0, 1);
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Replace(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source[0] = new SynchronizerTypes.ComplexType("c", 3);
                    var expected = new[] { new SynchronizerTypes.ComplexType("c", 3), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    source[1] = new SynchronizerTypes.ComplexType("d", 4);
                    expected = new[] { new SynchronizerTypes.ComplexType("c", 3), new SynchronizerTypes.ComplexType("d", 4) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                }
            }

            [Test]
            public void Synchronizes()
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                var target = new ObservableCollection<SynchronizerTypes.ComplexType>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
                {
                    var expected = new List<SynchronizerTypes.ComplexType> { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("b", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);

                    source.Add(new SynchronizerTypes.ComplexType("c", 3));
                    expected.Add(new SynchronizerTypes.ComplexType("c", 3));
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                    Assert.AreNotSame(source[2], target[2]);

                    source[2].Name = "changed";
                    expected[2].Name = "changed";
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                    Assert.AreNotSame(source[2], target[2]);

                    source.RemoveAt(1);
                    expected.RemoveAt(1);
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
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
        }
    }
}
