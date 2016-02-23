namespace Gu.ChangeTracking.Tests
{
    using System;
    using Gu.ChangeTracking.Tests.ChangeTrackerStubs;
    using NUnit.Framework;

    public partial class ChangeTrackerTests
    {
        public class Throws
        {
            [Test]
            public void OnPropertyOfTypeThatIsNotINotifyPropertyChanged()
            {
                var item = new WithIllegalObject();
                var exception = Assert.Throws<ArgumentException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
                Console.WriteLine(exception.Message);
                var expected = "Create tracker failed for Gu.ChangeTracking.Tests.ChangeTrackerStubs.WithIllegalObject.Illegal.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Add a specialcase to tracker setting example:\r\n" +
                               "    settings.AddSpecialType<Gu.ChangeTracking.Tests.ChangeTrackerStubs.IllegalObject>(...)\r\n" +
                               "    or:" +
                               "    settings.AddSpecialProperty(typeof(WithIllegalObject).GetProperty(nameof(WithIllegalObject.Illegal))" +
                               "    Note that this requires you to track changes.\r\n" +
                               "* Implement INotifyPropertyChanged for Gu.ChangeTracking.Tests.ChangeTrackerStubs.WithIllegalObject\r\n" +
                               "* Implement INotifyCollectionChanged for Gu.ChangeTracking.Tests.ChangeTrackerStubs.WithIllegalObject\r\n";
                Console.Write(exception.Message);
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void OnIllegalEnumerable()
            {
                var item = new IllegalEnumerable();
                var exception = Assert.Throws<ArgumentException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
                Assert.AreEqual("", exception.Message);
            }

            [Test]
            public void WithList()
            {
                var item = new WithList();
                var exception = Assert.Throws<ArgumentException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
                Assert.AreEqual("", exception.Message);
            }
        }
    }
}