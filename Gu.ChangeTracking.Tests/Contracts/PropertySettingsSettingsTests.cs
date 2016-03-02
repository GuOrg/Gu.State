namespace Gu.ChangeTracking.Tests.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public class PropertySettingsSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(DirtyTrackerTypes.ComplexType);
            var nameProperty = type.GetProperty(nameof(DirtyTrackerTypes.ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(DirtyTrackerTypes.ComplexType.Value));
            var settings = PropertiesSettings.Create(type, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw, new[] { nameProperty.Name });
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(valueProperty));
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = new PropertiesSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(null));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionFields(Type type)
        {
            var settings = PropertiesSettings.GetOrCreate();
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
