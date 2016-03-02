namespace Gu.ChangeTracking.Tests.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    using NUnit.Framework;

    public class CopyFieldsSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(ComplexType);
            var settings = FieldsSettings.Create(type, excludedFields: new[] { nameof(ComplexType.Name) });
            var nameField = type.GetField(nameof(ComplexType.Name), Constants.DefaultFieldBindingFlags);
            Assert.AreEqual(true, settings.IsIgnoringField(nameField));
            var valueField = type.GetField(nameof(ComplexType.Value), Constants.DefaultFieldBindingFlags);
            Assert.AreEqual(false, settings.IsIgnoringField(valueField));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionFields(Type type)
        {
            var settings = FieldsSettings.GetOrCreate();
            var fieldInfos = type.GetFields(Constants.DefaultFieldBindingFlags);
            if (type != typeof(int[]))
            {
                CollectionAssert.IsNotEmpty(fieldInfos);
            }

            foreach (var fieldInfo in fieldInfos)
            {
                Assert.AreEqual(true, settings.IsIgnoringField(fieldInfo));
            }
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = FieldsSettings.GetOrCreate();
            Assert.AreEqual(true, settings.IsIgnoringField(null));
        }

        [TestCase(BindingFlags.Public, ReferenceHandling.Throw)]
        [TestCase(BindingFlags.Public, ReferenceHandling.Structural)]
        public void Cache(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var settings = FieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            Assert.AreEqual(bindingFlags, settings.BindingFlags);
            Assert.AreEqual(referenceHandling, settings.ReferenceHandling);
            var second = FieldsSettings.GetOrCreate(BindingFlags.Public, referenceHandling);
            Assert.AreSame(settings, second);
        }

        public class ComplexType
        {
            public string Name;

            public int Value;
        }
    }
}
