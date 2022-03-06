namespace Gu.State.Tests.Settings
{
    using NUnit.Framework;

    using static SettingsTypes;

    public class FieldsSettingsBuilderTests
    {
        [Test]
        public void AddImmutableType()
        {
            var settings = new FieldsSettingsBuilder().AddImmutableType<ComplexType>().CreateSettings();
            Assert.AreEqual(true, settings.IsImmutable(typeof(ComplexType)));

            settings = new FieldsSettingsBuilder().AddImmutableType(typeof(ComplexType)).CreateSettings();
            Assert.AreEqual(true, settings.IsImmutable(typeof(ComplexType)));
        }
    }
}
