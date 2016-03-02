namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public class DirtyTrackerSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(DirtyTrackerTypes.ComplexType);
            var nameProperty = type.GetProperty(nameof(DirtyTrackerTypes.ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(DirtyTrackerTypes.ComplexType.Value));
            var settings = DirtyTrackerSettings.Create(type, new[] { nameProperty.Name }, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(valueProperty));
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = new DirtyTrackerSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(null));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionFields(Type type)
        {
            var settings = DirtyTrackerSettings.GetOrCreate(ReferenceHandling.Structural);
            var propertyInfos = type.GetProperties(Constants.DefaultFieldBindingFlags);
            if (type != typeof(int[]))
            {
                CollectionAssert.IsNotEmpty(propertyInfos);
            }

            foreach (var propertyInfo in propertyInfos)
            {
                Assert.AreEqual(true, settings.IsIgnoringProperty(propertyInfo));
            }
        }
    }
}
