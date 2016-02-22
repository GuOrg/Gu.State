namespace Gu.ChangeTracking.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public class DirtyTrackerSettingsTests
    {
        [Test]
        public void Ignores()
        {
            var type = typeof(ComplexType);
            var nameProperty = type.GetProperty(nameof(ComplexType.Name));
            var valueProperty = type.GetProperty(nameof(ComplexType.Value));
            var settings = new DirtyTrackerSettings(type, new[] { nameProperty.Name }, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(nameProperty));
            Assert.AreEqual(false, settings.IsIgnoringProperty(valueProperty));
        }

        [Test]
        public void IgnoresNull()
        {
            var settings = new DirtyTrackerSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(true, settings.IsIgnoringProperty(null));
        }

        [Test]
        public void IgnoresIndexer()
        {
            var type = typeof(List<int>);
            var countProperty = type.GetProperty(nameof(IList.Count));
            var indexerProperty = type.GetProperties().Single(x => x.GetIndexParameters().Length > 0);
            var settings = new DirtyTrackerSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
            Assert.AreEqual(false, settings.IsIgnoringProperty(countProperty));
            Assert.AreEqual(true, settings.IsIgnoringProperty(indexerProperty));
        }
    }
}
