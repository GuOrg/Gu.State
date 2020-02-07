namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class WithInheritance
        {
            [Test]
            public void NotDirtyWhenSameType()
            {
                var x = new With<BaseClass>();
                var y = new With<BaseClass>();
                using var tracker = Track.IsDirty(x, y);
                x.Value = new Derived2();
                y.Value = new Derived2();
                Assert.AreEqual(false, tracker.IsDirty);
            }

            [Test]
            public void DirtyWhenDifferentTypes()
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