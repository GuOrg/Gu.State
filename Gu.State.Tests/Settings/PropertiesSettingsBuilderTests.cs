namespace Gu.State.Tests.Settings
{
    using System;

    using NUnit.Framework;

    using static SettingsTypes;

    public class PropertiesSettingsBuilderTests
    {
        [Test]
        public void AddPropertyThrowsOnNested()
        {
            var builder = new PropertiesSettingsBuilder();
            var exception = Assert.Throws<ArgumentException>(() => builder.IgnoreProperty<ComplexType>(x => x.Name.Length));
            var expected = "property must be a property expression like foo => foo.Bar\r\n" +
                           "Nested properties are not allowed";
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void AddImmutableType()
        {
            var settings = new PropertiesSettingsBuilder().AddImmutableType<ComplexType>()
                                                          .CreateSettings();
            Assert.AreEqual(true, settings.IsImmutable(typeof(ComplexType)));

            settings = new PropertiesSettingsBuilder().AddImmutableType(typeof(ComplexType))
                                                      .CreateSettings();
            Assert.AreEqual(true, settings.IsImmutable(typeof(ComplexType)));
        }
    }
}
