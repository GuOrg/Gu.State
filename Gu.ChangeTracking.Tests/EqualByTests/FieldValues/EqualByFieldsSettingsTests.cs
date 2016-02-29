namespace Gu.ChangeTracking.Tests.EqualByTests.FieldValues
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    using NUnit.Framework;

    public class EqualByFieldsSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(ComplexType);
            var nameField = type.GetField(nameof(ComplexType.Name));
            var valueField = type.GetField(nameof(ComplexType.Value));
            var settings = new EqualByFieldsSettings(new[] { nameField }, Constants.DefaultFieldBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringField(nameField));
            Assert.AreEqual(false, settings.IsIgnoringField(valueField));
        }

        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionFields(Type type)
        {
            var settings = EqualByFieldsSettings.GetOrCreate(ReferenceHandling.Structural);
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
            var settings = new EqualByFieldsSettings(null, Constants.DefaultFieldBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringField(null));
        }

        [TestCase(BindingFlags.Public, ReferenceHandling.Throw)]
        [TestCase(BindingFlags.Public, ReferenceHandling.Structural)]
        public void Cache(BindingFlags bindingFlags, ReferenceHandling referenceHandling)
        {
            var settings = EqualByFieldsSettings.GetOrCreate(bindingFlags, referenceHandling);
            Assert.AreEqual(bindingFlags, settings.BindingFlags);
            Assert.AreEqual(referenceHandling, settings.ReferenceHandling);
            var second = EqualByFieldsSettings.GetOrCreate(BindingFlags.Public, referenceHandling);
            Assert.AreSame(settings, second);
        }

        public class ComplexType
        {
            public string Name;

            public int Value;
        }
    }
}