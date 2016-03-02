namespace Gu.ChangeTracking.Tests.Contracts
{
    using System;

    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;

    using NUnit.Framework;

    public class ChangeTrackerSettingsTests
    {
        [Test]
        public void AddPropertyLambda()
        {
            var settings = new ChangeTrackerSettings();
            settings.AddIgnoredProperty<Level>(x => x.Next);
            Assert.IsTrue(settings.IsIgnoringProperty(typeof(Level).GetProperty(nameof(Level.Next))));
        }

        [Test]
        public void AddPropertyThrowsOnNested()
        {
            var settings = new ChangeTrackerSettings();
            var exception = Assert.Throws<ArgumentException>(() => settings.AddIgnoredProperty<Level>(x => x.Next.Value));
            var expected = "property must be a property expression like foo => foo.Bar\r\n" +
                           "Nested properties are not allowed";
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
