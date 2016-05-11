// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;

    using NUnit.Framework;

    public class PropertiesSettingsTests
    {
        [Test]
        public void IgnoresPropertyCtor()
        {
            var type = typeof(SettingsTypes.ComplexType);
            var nameProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Value));
            var settings = new PropertiesSettings(new[] { nameProperty }, null, null, null, ReferenceHandling.Throw, Constants.DefaultPropertyBindingFlags);
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(valueProperty));
        }

        [Test]
        public void IgnoresPropertyBuilder()
        {
            var type = typeof(SettingsTypes.ComplexType);
            var nameProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Value));
            var settings = PropertiesSettings.Build()
                                             .IgnoreProperty(nameProperty)
                                             .CreateSettings();
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(valueProperty));
        }

        [Test]
        public void IgnoresIndexer()
        {
            var type = typeof(SettingsTypes.WithIndexerType);
            var nameProperty = type.GetProperty(nameof(SettingsTypes.WithIndexerType.Name));
            var indexerProperty = type.GetProperties().Single(x => x.GetIndexParameters().Length > 0);
            var settings = PropertiesSettings.Build()
                                             .IgnoreIndexersFor<SettingsTypes.WithIndexerType>()
                                             .CreateSettings();
            Assert.AreEqual(false, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(true, settings.IsIgnoringProperty(indexerProperty));
        }

        [Test]
        public void IgnoresBaseClassPropertyLambda()
        {
            var settings = PropertiesSettings.Build()
                                             .IgnoreProperty<SettingsTypes.ComplexType>(x => x.Name)
                                             .CreateSettings();
            var nameProperty = typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name));
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));

            nameProperty = typeof(SettingsTypes.Derived).GetProperty(nameof(SettingsTypes.Derived.Name));
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(typeof(SettingsTypes.Derived).GetProperty(nameof(SettingsTypes.Derived.Value))));
        }

        [Test]
        public void IgnoresInterfaceProperty()
        {
            var settings = PropertiesSettings.Build()
                                 .IgnoreProperty<SettingsTypes.IComplexType>(x => x.Name)
                                 .CreateSettings();
            Assert.AreEqual(true, settings.IsIgnoringProperty(typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name))));
            Assert.AreEqual(true, settings.IsIgnoringProperty(typeof(SettingsTypes.IComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name))));
        }

        [Test]
        public void IgnoresTypeCtor()
        {
            var type = typeof(SettingsTypes.ComplexType);
            var nameProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Value));
            var settings = new PropertiesSettings(null, new[] { type }, null, null, ReferenceHandling.Throw, Constants.DefaultPropertyBindingFlags);
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(true, settings.IsIgnoringProperty(valueProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(typeof(SettingsTypes.Immutable).GetProperty(nameof(SettingsTypes.Immutable.Value))));
        }

        [Test]
        public void IgnoresTypeBuilder()
        {
            var type = typeof(SettingsTypes.ComplexType);
            var nameProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(SettingsTypes.ComplexType.Value));
            var settings = PropertiesSettings.Build()
                                             .IgnoreType(type)
                                             .CreateSettings();
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(true, settings.IsIgnoringProperty(valueProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(typeof(SettingsTypes.Immutable).GetProperty(nameof(SettingsTypes.Immutable.Value))));
        }

        [Test]
        public void IgnoresBaseTypeBuilder()
        {
            var settings = PropertiesSettings.Build()
                                             .IgnoreType<SettingsTypes.ComplexType>()
                                             .CreateSettings();
            Assert.AreEqual(true, settings.IsIgnoringProperty(typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name))));
            Assert.AreEqual(true, settings.IsIgnoringProperty(typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Value))));
            Assert.AreEqual(true, settings.IsIgnoringProperty(typeof(SettingsTypes.Derived).GetProperty(nameof(SettingsTypes.Derived.Name))));
            Assert.AreEqual(true, settings.IsIgnoringProperty(typeof(SettingsTypes.Derived).GetProperty(nameof(SettingsTypes.Derived.Value))));
            Assert.AreEqual(false, settings.IsIgnoringProperty(typeof(SettingsTypes.Derived).GetProperty(nameof(SettingsTypes.Derived.DoubleValue))));
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = PropertiesSettings.GetOrCreate();
            Assert.AreEqual(true, settings.IsIgnoringProperty(null));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(ImmutableList<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(ImmutableArray<int>))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        [TestCase(typeof(ImmutableDictionary<int, int>))]
        [TestCase(typeof(HashSet<int>))]
        [TestCase(typeof(ImmutableHashSet<int>))]
        public void IgnoresCollectionProperties(Type type)
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

        [TestCase(BindingFlags.Public, ReferenceHandling.Throw)]
        [TestCase(BindingFlags.Public, ReferenceHandling.Structural)]
        public void Cache(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var settings = PropertiesSettings.GetOrCreate(referenceHandling, bindingFlags);
            Assert.AreEqual(bindingFlags, settings.BindingFlags);
            Assert.AreEqual(referenceHandling, settings.ReferenceHandling);
            var second = PropertiesSettings.GetOrCreate(referenceHandling, BindingFlags.Public);
            Assert.AreSame(settings, second);
        }
    }
}
