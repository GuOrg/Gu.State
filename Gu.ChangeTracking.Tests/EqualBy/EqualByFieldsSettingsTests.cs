namespace Gu.ChangeTracking.Tests
{
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
            var settings = new EqualByFieldsSettings(type, new[] { nameField.Name }, Constants.DefaultFieldBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringField(nameField));
            Assert.AreEqual(false, settings.IsIgnoringField(valueField));
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