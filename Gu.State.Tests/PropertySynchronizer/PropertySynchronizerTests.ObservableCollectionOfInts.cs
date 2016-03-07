namespace Gu.State.Tests
{
    using System;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class ObservableCollectionOfInts
        {
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void CreateAndDispose(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int>();
                using (var synchronizer = Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    var itemsSynchronizerField = synchronizer.GetType().GetField("itemsSynchronizer", Constants.DefaultFieldBindingFlags);
                    Assert.NotNull(itemsSynchronizerField);
                    var itemsSynchronizer = itemsSynchronizerField.GetValue(synchronizer);
                    Assert.NotNull(itemsSynchronizer);
                    var itemSynchronizersField = itemsSynchronizer.GetType().GetField("itemSynchronizers", Constants.DefaultFieldBindingFlags);
                    Assert.NotNull(itemSynchronizersField);
                    Assert.IsNull(itemSynchronizersField.GetValue(itemsSynchronizer));

                    CollectionAssert.AreEqual(new[] { 1, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 2 }, target);
                }

                source.Add(3);
                CollectionAssert.AreEqual(new[] { 1, 2, 3 }, source);
                CollectionAssert.AreEqual(new[] { 1, 2 }, target);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Add(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int>();
                var target = new ObservableCollection<int>();
                using (Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.Add(1);
                    CollectionAssert.AreEqual(new[] { 1 }, source);
                    CollectionAssert.AreEqual(new[] { 1 }, target);
                }

                source.Add(2);
                CollectionAssert.AreEqual(new[] { 1, 2 }, source);
                CollectionAssert.AreEqual(new[] { 1 }, target);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Remove(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.RemoveAt(1);
                    CollectionAssert.AreEqual(new[] { 1 }, source);
                    CollectionAssert.AreEqual(new[] { 1 }, target);

                    source.RemoveAt(0);
                    CollectionAssert.IsEmpty(source);
                    CollectionAssert.IsEmpty(target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Insert(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.Insert(1, 3);
                    CollectionAssert.AreEqual(new[] { 1, 3, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 3, 2 }, target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Move(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source.Move(1, 0);
                    CollectionAssert.AreEqual(new[] { 2, 1 }, source);
                    CollectionAssert.AreEqual(new[] { 2, 1 }, target);

                    source.Move(0, 1);
                    CollectionAssert.AreEqual(new[] { 1, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 1, 2 }, target);
                }
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void Replace(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: referenceHandling))
                {
                    source[0] = 3;
                    CollectionAssert.AreEqual(new[] { 3, 2 }, source);
                    CollectionAssert.AreEqual(new[] { 3, 2 }, target);

                    source[1] = 4;
                    CollectionAssert.AreEqual(new[] { 3, 4 }, source);
                    CollectionAssert.AreEqual(new[] { 3, 4 }, target);
                }
            }

            [Test]
            public void ThrowsIfTargetCollectionChanges()
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronizer.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => target.Add(3));
                    var expected = "You cannot modify the target collection when you have applied a PropertySynchronizer on it";
                    Assert.AreEqual(expected, exception.Message);
                }
            }
        }
    }
}
