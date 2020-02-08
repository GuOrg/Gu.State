// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static SynchronizeTypes;

    public partial class SynchronizeTests
    {
        public class ObservableCollectionOfImmutableTypes
        {
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void CreateAndDisposeWhenTargetIsEmpty(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>>();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[1], target[1]);
                }

                source.Add(new WithGetReadOnlyPropertySealed<int>(3));
                CollectionAssert.AreEqual(new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2), new WithGetReadOnlyPropertySealed<int>(3) }, source);
                CollectionAssert.AreEqual(new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) }, target);
            }

            [Test]
            public void CreateAndDisposeWhenTargetIsNotEmptyStructural()
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.Structural))
                {
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                }

                source.Add(new WithGetReadOnlyPropertySealed<int>(3));
                CollectionAssert.AreEqual(new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2), new WithGetReadOnlyPropertySealed<int>(3) }, source);
                CollectionAssert.AreEqual(new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) }, target);
            }

            [Test]
            public void CreateAndDisposeWhenTargetIsNotEmptyReference()
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.References))
                {
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);

                    Assert.AreNotSame(source[0], target[0]);
                    Assert.AreNotSame(source[1], target[1]);
                }

                source.Add(new WithGetReadOnlyPropertySealed<int>(3));
                CollectionAssert.AreEqual(new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2), new WithGetReadOnlyPropertySealed<int>(3) }, source);
                CollectionAssert.AreEqual(new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) }, target);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Add(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>>();
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>>();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Add(new WithGetReadOnlyPropertySealed<int>(1));
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(1) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                    Assert.AreSame(source[0], target[0]);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Remove(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.RemoveAt(1);
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(1) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);

                    source.RemoveAt(0);
                    CollectionAssert.IsEmpty(source);
                    CollectionAssert.IsEmpty(target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Insert(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Insert(1, new WithGetReadOnlyPropertySealed<int>(3));
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(3), new WithGetReadOnlyPropertySealed<int>(2) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                    Assert.AreSame(source[0], target[0]);
                    Assert.AreSame(source[2], target[2]);
                    Assert.AreSame(source[1], target[1]);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Move(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Move(1, 0);
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(2), new WithGetReadOnlyPropertySealed<int>(1) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);

                    source.Move(0, 1);
                    expected = new[] { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Replace(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                var target = new ObservableCollection<WithGetReadOnlyPropertySealed<int>> { new WithGetReadOnlyPropertySealed<int>(1), new WithGetReadOnlyPropertySealed<int>(2) };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source[0] = new WithGetReadOnlyPropertySealed<int>(3);
                    var expected = new[] { new WithGetReadOnlyPropertySealed<int>(3), new WithGetReadOnlyPropertySealed<int>(2) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);

                    source[1] = new WithGetReadOnlyPropertySealed<int>(4);
                    expected = new[] { new WithGetReadOnlyPropertySealed<int>(3), new WithGetReadOnlyPropertySealed<int>(4) };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                }
            }
        }
    }
}
