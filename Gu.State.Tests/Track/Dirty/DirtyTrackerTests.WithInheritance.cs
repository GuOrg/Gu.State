namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public static partial class DirtyTrackerTests
    {
        public static class WithInheritance
        {
            [Test]
            public static void NotDirtyWhenSameType()
            {
                var x = new With<BaseClass>();
                var y = new With<BaseClass>();
                using var tracker = Track.IsDirty(x, y);
                x.Value = new Derived2();
                y.Value = new Derived2();
                Assert.AreEqual(false, tracker.IsDirty);
            }

            [Test]
            public static void DirtyWhenDifferentTypes()
            {
                var x = new With<BaseClass>();
                var y = new With<BaseClass>();
                using var tracker = Track.IsDirty(x, y);
                x.Value = new Derived2();
                y.Value = new Derived1();
                Assert.AreEqual(true, tracker.IsDirty);
            }
        }
    }
}
