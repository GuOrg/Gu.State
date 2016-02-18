namespace Gu.ChangeTracking.Tests
{
    using System.Collections.ObjectModel;

    using Gu.ChangeTracking.Tests.PropertySynchronizerStubs;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class Collection
        {
            [Test]
            public void ObservableCollectionOfInts()
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int>();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    CollectionAssert.AreEqual(new[] { 1, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 2 }, target);

                    source.Add(3);
                    CollectionAssert.AreEqual(new[] { 1, 2, 3 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 2, 3 }, target);

                    source.RemoveAt(1);
                    CollectionAssert.AreEqual(new[] { 1, 3 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 3 }, target);

                    target.RemoveAt(1);
                    CollectionAssert.AreEqual(new[] { 1, 3 }, source);
                    CollectionAssert.AreEqual(new[] { 1 }, target);

                    source.Clear();
                    CollectionAssert.IsEmpty(source);
                    CollectionAssert.IsEmpty(target);
                }
            }

            [Test]
            public void ObservableCollectionOfComplexTypes()
            {
                var source = new ObservableCollection<ComplexType> { new ComplexType("a", 1), new ComplexType("b", 2) };
                var target = new ObservableCollection<ComplexType>();
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    Assert.AreEqual(2, source.Count);
                    Assert.AreEqual(2, target.Count);
                    Assert.AreEqual("a", source[0].Name);
                    Assert.AreEqual("a", target[0].Name);
                    Assert.AreEqual(1, source[0].Value);
                    Assert.AreEqual(1, target[0].Value);

                    Assert.AreEqual("b", source[1].Name);
                    Assert.AreEqual("b", target[1].Name);
                    Assert.AreEqual(2, source[1].Value);
                    Assert.AreEqual(2, target[1].Value);

                    source.Add(new ComplexType("c", 3));
                    Assert.AreEqual(3, source.Count);
                    Assert.AreEqual(3, target.Count);
                    Assert.AreEqual("a", source[0].Name);
                    Assert.AreEqual("a", target[0].Name);
                    Assert.AreEqual(1, source[0].Value);
                    Assert.AreEqual(1, target[0].Value);

                    Assert.AreEqual("b", source[1].Name);
                    Assert.AreEqual("b", target[1].Name);
                    Assert.AreEqual(2, source[1].Value);
                    Assert.AreEqual(2, target[1].Value);

                    Assert.AreEqual("c", source[2].Name);
                    Assert.AreEqual("c", target[2].Name);
                    Assert.AreEqual(3, source[2].Value);
                    Assert.AreEqual(3, target[2].Value);

                    source[2].Name = "changed";
                    Assert.AreEqual(3, source.Count);
                    Assert.AreEqual(3, target.Count);
                    Assert.AreEqual("a", source[0].Name);
                    Assert.AreEqual("a", target[0].Name);
                    Assert.AreEqual(1, source[0].Value);
                    Assert.AreEqual(1, target[0].Value);

                    Assert.AreEqual("b", source[1].Name);
                    Assert.AreEqual("b", target[1].Name);
                    Assert.AreEqual(2, source[1].Value);
                    Assert.AreEqual(2, target[1].Value);

                    Assert.AreEqual("changed", source[2].Name);
                    Assert.AreEqual("changed", target[2].Name);
                    Assert.AreEqual(3, source[2].Value);
                    Assert.AreEqual(3, target[2].Value);

                    source.RemoveAt(1);
                    Assert.AreEqual(2, source.Count);
                    Assert.AreEqual(2, target.Count);
                    Assert.AreEqual("a", source[0].Name);
                    Assert.AreEqual("a", target[0].Name);
                    Assert.AreEqual(1, source[0].Value);
                    Assert.AreEqual(1, target[0].Value);

                    Assert.AreEqual("changed", source[1].Name);
                    Assert.AreEqual("changed", target[1].Name);
                    Assert.AreEqual(3, source[1].Value);
                    Assert.AreEqual(3, target[1].Value);

                    target.RemoveAt(0);
                    Assert.AreEqual(2, source.Count);
                    Assert.AreEqual(1, target.Count);
                    Assert.AreEqual("a", source[0].Name);
                    Assert.AreEqual("changed", target[0].Name);
                    Assert.AreEqual(1, source[0].Value);
                    Assert.AreEqual(2, target[0].Value);

                    Assert.AreEqual("changed", source[1].Name);
                    Assert.AreEqual(3, source[1].Value);

                    Assert.Inconclusive("Not sure how to handle the situation where target changes. Maybe throw but not very elegant");
                }
            }

            [Test]
            public void WithObservableCollectionProperty()
            {
                var source = new WithObservableCollectionProperty("a", 1);
                source.Complexes.Add(new ComplexType("a.1", 11));
                var target = new WithObservableCollectionProperty("b", 2);
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    Assert.Fail();
                    //Assert.AreEqual(2, source.Count);
                    //Assert.AreEqual(2, target.Count);
                    //Assert.AreEqual("a", source[0].Name);
                    //Assert.AreEqual("a", target[0].Name);
                    //Assert.AreEqual(1, source[0].Value);
                    //Assert.AreEqual(1, target[0].Value);

                    //Assert.AreEqual("b", source[1].Name);
                    //Assert.AreEqual("b", target[1].Name);
                    //Assert.AreEqual(2, source[1].Value);
                    //Assert.AreEqual(2, target[1].Value);

                    //source.Add(new ComplexType("c", 3));
                    //Assert.AreEqual(3, source.Count);
                    //Assert.AreEqual(3, target.Count);
                    //Assert.AreEqual("a", source[0].Name);
                    //Assert.AreEqual("a", target[0].Name);
                    //Assert.AreEqual(1, source[0].Value);
                    //Assert.AreEqual(1, target[0].Value);

                    //Assert.AreEqual("b", source[1].Name);
                    //Assert.AreEqual("b", target[1].Name);
                    //Assert.AreEqual(2, source[1].Value);
                    //Assert.AreEqual(2, target[1].Value);

                    //Assert.AreEqual("c", source[2].Name);
                    //Assert.AreEqual("c", target[2].Name);
                    //Assert.AreEqual(3, source[2].Value);
                    //Assert.AreEqual(3, target[2].Value);

                    //source[2].Name = "changed";
                    //Assert.AreEqual(3, source.Count);
                    //Assert.AreEqual(3, target.Count);
                    //Assert.AreEqual("a", source[0].Name);
                    //Assert.AreEqual("a", target[0].Name);
                    //Assert.AreEqual(1, source[0].Value);
                    //Assert.AreEqual(1, target[0].Value);

                    //Assert.AreEqual("b", source[1].Name);
                    //Assert.AreEqual("b", target[1].Name);
                    //Assert.AreEqual(2, source[1].Value);
                    //Assert.AreEqual(2, target[1].Value);

                    //Assert.AreEqual("changed", source[2].Name);
                    //Assert.AreEqual("changed", target[2].Name);
                    //Assert.AreEqual(3, source[2].Value);
                    //Assert.AreEqual(3, target[2].Value);

                    //source.RemoveAt(1);
                    //Assert.AreEqual(2, source.Count);
                    //Assert.AreEqual(2, target.Count);
                    //Assert.AreEqual("a", source[0].Name);
                    //Assert.AreEqual("a", target[0].Name);
                    //Assert.AreEqual(1, source[0].Value);
                    //Assert.AreEqual(1, target[0].Value);

                    //Assert.AreEqual("changed", source[1].Name);
                    //Assert.AreEqual("changed", target[1].Name);
                    //Assert.AreEqual(3, source[1].Value);
                    //Assert.AreEqual(3, target[1].Value);

                    //target.RemoveAt(0);
                    //Assert.AreEqual(2, source.Count);
                    //Assert.AreEqual(1, target.Count);
                    //Assert.AreEqual("a", source[0].Name);
                    //Assert.AreEqual("changed", target[0].Name);
                    //Assert.AreEqual(1, source[0].Value);
                    //Assert.AreEqual(2, target[0].Value);

                    //Assert.AreEqual("changed", source[1].Name);
                    //Assert.AreEqual(3, source[1].Value);

                    //Assert.Inconclusive("Not sure how to handle the situation where target changes. Maybe throw but not very elegant");
                }
            }
        }
    }
}
