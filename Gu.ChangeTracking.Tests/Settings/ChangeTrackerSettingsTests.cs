namespace Gu.ChangeTracking.Tests.Settings
{
    using System;
    using Gu.ChangeTracking.Tests.Helpers;
    using NUnit.Framework;

    public class ChangeTrackerSettingsTests
    {
        [Test]
        public void AddPropertyLambda()
        {
            var settings = new ChangeTrackerSettings();
            settings.AddExplicitProperty<Level>(x => x.Next);
            Assert.IsTrue(settings.IsIgnored(typeof(Level).GetProperty(nameof(Level.Next))));
        }

        [Test]
        public void AddPropertyThrowsOnNested()
        {
            var settings = new ChangeTrackerSettings();
            var exception = Assert.Throws<ArgumentException>(() => settings.AddExplicitProperty<Level>(x => x.Next.Value));
            var expected = "property must be a property expression like foo => foo.Bar\r\n" +
                           "Nested properties are not allowed";
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
