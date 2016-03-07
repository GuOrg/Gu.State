namespace Gu.State.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class ReferenceLoops
        {
            [Test]
            public void WithTwoComplexProperties()
            {
                var source = new SynchronizerTypes.WithTwoComplexProperties("a", 1) { ComplexValue1 = new SynchronizerTypes.ComplexType("a.1", 2), ComplexValue2 = new SynchronizerTypes.ComplexType("a.2", 3) };
                var target = new SynchronizerTypes.WithTwoComplexProperties("b", 3) { ComplexValue1 = new SynchronizerTypes.ComplexType("b.1", 4) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.StructuralWithReferenceLoops))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    Assert.AreNotSame(source.ComplexValue1, target.ComplexValue1);
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("a.1", source.ComplexValue1.Name);
                    Assert.AreEqual("a.1", target.ComplexValue1.Name);
                    Assert.AreEqual("a.2", source.ComplexValue2.Name);
                    Assert.AreEqual("a.2", target.ComplexValue2.Name);

                    source.Value++;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreNotSame(source.ComplexValue1, target.ComplexValue1);
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("a.1", source.ComplexValue1.Name);
                    Assert.AreEqual("a.1", target.ComplexValue1.Name);
                    Assert.AreEqual("a.2", source.ComplexValue2.Name);
                    Assert.AreEqual("a.2", target.ComplexValue2.Name);

                    source.ComplexValue1.Name += "_";
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("a.1_", source.ComplexValue1.Name);
                    Assert.AreEqual("a.1_", target.ComplexValue1.Name);
                    Assert.AreEqual("a.2", source.ComplexValue2.Name);
                    Assert.AreEqual("a.2", target.ComplexValue2.Name);

                    source.ComplexValue1 = source.ComplexValue2;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("a.2", source.ComplexValue1.Name);
                    Assert.AreEqual("a.2", target.ComplexValue1.Name);
                    Assert.AreEqual("a.2", source.ComplexValue2.Name);
                    Assert.AreEqual("a.2", target.ComplexValue2.Name);

                    source.ComplexValue2 = null;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("a.2", source.ComplexValue1.Name);
                    Assert.AreEqual("a.2", target.ComplexValue1.Name);
                    Assert.AreEqual(null, source.ComplexValue2);
                    Assert.AreEqual(null, target.ComplexValue2);
                }

                source.Name += "_";
                Assert.AreEqual("a_", source.Name);
                Assert.AreEqual("a", target.Name);
                Assert.AreEqual(2, source.Value);
                Assert.AreEqual(2, target.Value);
                Assert.AreEqual("a.2", source.ComplexValue1.Name);
                Assert.AreEqual("a.2", target.ComplexValue1.Name);
                Assert.AreEqual(null, source.ComplexValue2);
                Assert.AreEqual(null, target.ComplexValue2);
            }

            [Test]
            public void CreateAndDisposeParentChild()
            {
                var source = new SynchronizerTypes.Parent("a", new SynchronizerTypes.Child("b"));
                var target = new SynchronizerTypes.Parent("b", new SynchronizerTypes.Child());
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.StructuralWithReferenceLoops))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("b", source.Child.Name);
                    Assert.AreEqual("b", target.Child.Name);

                    source.Name = "a1";
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual("b", source.Child.Name);
                    Assert.AreEqual("b", target.Child.Name);

                    source.Child.Name = "b1";
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual("b1", source.Child.Name);
                    Assert.AreEqual("b1", target.Child.Name);
                    var sc = source.Child;
                    var tc = target.Child;

                    source.Child = null;
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual(null, source.Child);
                    Assert.AreEqual(null, target.Child);

                    sc.Name = "new";
                    Assert.AreEqual("b1", tc.Name);
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual(null, source.Child);
                    Assert.AreEqual(null, target.Child);
                }

                source.Name = "_";
                Assert.AreEqual("_", source.Name);
                Assert.AreEqual("a1", target.Name);
                Assert.AreEqual(null, source.Child);
                Assert.AreEqual(null, target.Child);
            }

            [Test]
            public void CollectionWithSelf()
            {
                Assert.Inconclusive("Probably not possible without references");
                var source = new ObservableCollection<INotifyCollectionChanged>();
                source.Add(source);
                var target = new ObservableCollection<INotifyCollectionChanged>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.StructuralWithReferenceLoops))
                {
                    Assert.AreEqual(1, source.Count);
                    Assert.AreEqual(1, target.Count);
                    Assert.AreNotSame(source[0], target[0]);
                }
            }

            [Test]
            public void CollectionWithSame()
            {
                var source = new ObservableCollection<SynchronizerTypes.ComplexType>();
                var complexType = new SynchronizerTypes.ComplexType("a", 1);
                source.Add(complexType);
                source.Add(complexType);
                var target = new ObservableCollection<SynchronizerTypes.ComplexType>();
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.StructuralWithReferenceLoops))
                {
                    var expected = new[] { new SynchronizerTypes.ComplexType("a", 1), new SynchronizerTypes.ComplexType("a", 1) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    complexType.Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2), new SynchronizerTypes.ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    source.RemoveAt(1);
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 2) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);

                    complexType.Value++;
                    expected = new[] { new SynchronizerTypes.ComplexType("a", 3) };
                    CollectionAssert.AreEqual(expected, source, SynchronizerTypes.ComplexType.Comparer);
                    CollectionAssert.AreEqual(expected, target, SynchronizerTypes.ComplexType.Comparer);
                }
            }
        }
    }
}
