namespace Gu.ChangeTracking.Tests
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;
    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using NUnit.Framework;

    public class CollectionItemTrackerTests
    {
        private static readonly PropertyInfo DummyPropertyInfo = typeof(List<int>).GetProperty("Count");
        private List<PropertyChangedEventArgs> changes;

        [SetUp]
        public void SetUp()
        {
            this.changes = new List<PropertyChangedEventArgs>();
        }

        [Test]
        public void NotifiesOnCollectionChanged()
        {
            var ints = new ObservableCollection<int>();
            using (var tracker = new CollectionTracker(typeof(CollectionItemTrackerTests), DummyPropertyInfo, ints, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += this.TrackerOnPropertyChanged;

                ints.Add(1);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, this.changes.Count);
                tracker.PropertyChanged -= this.TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesOnCollectionItemChanged()
        {
            var items = new ObservableCollection<Level>();
            using (var tracker = new CollectionTracker(typeof(CollectionItemTrackerTests), DummyPropertyInfo, items, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += this.TrackerOnPropertyChanged;

                var item = new Level();
                items.Add(item);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, this.changes.Count);

                item.Value++;
                Assert.AreEqual(2, tracker.Changes);
                Assert.AreEqual(2, this.changes.Count);


                items.Add(new Level());
                Assert.AreEqual(3, tracker.Changes);
                Assert.AreEqual(3, this.changes.Count);

                tracker.PropertyChanged -= this.TrackerOnPropertyChanged;
            }
        }

        private void TrackerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.changes.Add(e);
        }
    }
}
