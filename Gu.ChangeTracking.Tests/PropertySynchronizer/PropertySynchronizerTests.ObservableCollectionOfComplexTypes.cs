namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gu.ChangeTracking.Tests.PropertySynchronizerStubs;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class ObservableCollectionOfComplexTypes
        {
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
