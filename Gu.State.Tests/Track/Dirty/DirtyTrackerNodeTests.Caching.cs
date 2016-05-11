namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerNodeTests
    {
        public class Caching
        {
            [Test]
            public void ReturnsSameForSameWhileAlive()
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var y = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var settings = PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.Structural);
                var t1 = DirtyTrackerNode.GetOrCreate(x, y, settings);
                var t2 = DirtyTrackerNode.GetOrCreate(x, y, settings);
                Assert.AreSame(t1, t2);
                t1.Dispose();
                t2.Dispose();

                var t3 = DirtyTrackerNode.GetOrCreate(x, y, settings);
                Assert.AreNotSame(t1, t3);
            }

            [Test]
            public void ReturnsDifferentForDifferentPairs()
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var y = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var settings = PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.Structural);
                var t1 = DirtyTrackerNode.GetOrCreate(x, y, settings);
                var t2 = DirtyTrackerNode.GetOrCreate(y, x, settings);
                Assert.AreNotSame(t1, t2);
            }

            [Test]
            public void ReturnsDifferentForDifferentSettings()
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var y = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var t1 = DirtyTrackerNode.GetOrCreate(x, y, PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.Structural));
                var t2 = DirtyTrackerNode.GetOrCreate(x, y, PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.References));
                Assert.AreNotSame(t1, t2);
            }
        }
    }
}