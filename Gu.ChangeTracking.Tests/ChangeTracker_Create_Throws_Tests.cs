namespace Gu.ChangeTracking.Tests
{
    using System;

    using Gu.ChangeTracking.Tests.Helpers;

    using NUnit.Framework;

    public class ChangeTracker_Create_Throws_Tests
    {
        [Test]
        public void ThrowsOnIllegalObjectNested()
        {
            var item = new Dummy();
            var exception = Assert.Throws<ArgumentException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void ThrowsOnIllegalEnumerableNested()
        {
            var item = new IllegalEnumerable();
            var exception = Assert.Throws<ArgumentException>(() => ChangeTracker.Track(item, ChangeTrackerSettings.Default));
            Console.WriteLine(exception.Message);
        }
    }
}