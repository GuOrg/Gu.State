namespace Gu.ChangeTracking.Tests.PropertyValues
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    public class CopyPropertiesSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(ComplexType);
            var nameProperty = type.GetProperty(nameof(ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(ComplexType.Value));
            var settings = CopyPropertiesSettings.Create(type, new[] { nameProperty.Name }, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(valueProperty));
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = CopyPropertiesSettings.GetOrCreate(Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(null));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionProperties(Type type)
        {
            var settings = CopyPropertiesSettings.GetOrCreate(ReferenceHandling.Structural);
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            CollectionAssert.IsNotEmpty(properties);
            foreach (var propertyInfo in properties)
            {
                Assert.AreEqual(true, settings.IsIgnoringProperty(propertyInfo));
            }
        }

        [TestCase(BindingFlags.Public, ReferenceHandling.Throw)]
        [TestCase(BindingFlags.Public, ReferenceHandling.Structural)]
        public void Cache(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var settings = CopyPropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
            Assert.AreEqual(bindingFlags, settings.BindingFlags);
            Assert.AreEqual(referenceHandling, settings.ReferenceHandling);
            var second = CopyPropertiesSettings.GetOrCreate(BindingFlags.Public, referenceHandling);
            Assert.AreSame(settings, second);
        }

        public class ComplexType
        {
            public string Name { get; set; }

            public int Value { get; set; }
        }
    }
}
