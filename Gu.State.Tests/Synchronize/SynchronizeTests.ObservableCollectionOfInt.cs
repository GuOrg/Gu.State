namespace Gu.State.Tests
{
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public static partial class SynchronizeTests
    {
        public static class ObservableCollectionOfInt
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void CreateAndDispose(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int>();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    var expected = new[] { 1, 2 };
                    CollectionAssert.AreEqual(expected, source);
                    CollectionAssert.AreEqual(expected, target);
                }

                source.Add(3);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, source);
                CollectionAssert.AreEqual(new[] { 1, 2 }, target);
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Add(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int>();
                var target = new ObservableCollection<int>();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Add(1);
                    CollectionAssert.AreEqual(new[] { 1 }, source);
                    CollectionAssert.AreEqual(new[] { 1 }, target);
                }

                source.Add(2);
                CollectionAssert.AreEqual(new[] { 1, 2 }, source);
                CollectionAssert.AreEqual(new[] { 1 }, target);
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Remove(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.RemoveAt(1);
                    CollectionAssert.AreEqual(new[] { 1 }, source);
                    CollectionAssert.AreEqual(new[] { 1 }, target);

                    source.RemoveAt(0);
                    CollectionAssert.IsEmpty(source);
                    CollectionAssert.IsEmpty(target);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Insert(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Insert(1, 3);
                    CollectionAssert.AreEqual(new[] { 1, 3, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 3, 2 }, target);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Move(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source.Move(1, 0);
                    CollectionAssert.AreEqual(new[] { 2, 1 }, source);
                    CollectionAssert.AreEqual(new[] { 2, 1 }, target);

                    source.Move(0, 1);
                    CollectionAssert.AreEqual(new[] { 1, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 2 }, target);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public static void Replace(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    source[0] = 3;
                    CollectionAssert.AreEqual(new[] { 3, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 3, 2 }, target);

                    source[1] = 4;
                    CollectionAssert.AreEqual(new[] { 3, 4 }, source);
                    CollectionAssert.AreEqual(new[] { 3, 4 }, target);
                }
            }
        }
    }
}
