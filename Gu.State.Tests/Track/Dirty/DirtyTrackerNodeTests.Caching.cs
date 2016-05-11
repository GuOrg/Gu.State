// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public class DirtyTrackerNodeTests
    {
        public class Caching
        {
            [Test]
            public void ReturnsSameForSameWhileAlive()
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var y = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Structural);
                var t1 = DirtyTrackerNode.GetOrCreate(x, y, settings, true);
                var t2 = DirtyTrackerNode.GetOrCreate(x, y, settings, true);
                Assert.AreSame(t1, t2);
                t1.Dispose();
                t2.Dispose();

                var t3 = DirtyTrackerNode.GetOrCreate(x, y, settings, true);
                Assert.AreNotSame(t1, t3);
            }

            [Test]
            public void ReturnsDifferentForDifferentPairs()
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var y = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var settings = PropertiesSettings.GetOrCreate(ReferenceHandling.Structural);
                var t1 = DirtyTrackerNode.GetOrCreate(x, y, settings, true);
                var t2 = DirtyTrackerNode.GetOrCreate(y, x, settings, true);
                Assert.AreNotSame(t1, t2);
            }

            [Test]
            public void ReturnsDifferentForDifferentSettings()
            {
                var x = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var y = new WithSimpleProperties { Value1 = 1, Value2 = 2 };
                var t1 = DirtyTrackerNode.GetOrCreate(x, y, PropertiesSettings.GetOrCreate(ReferenceHandling.Structural), true);
                var t2 = DirtyTrackerNode.GetOrCreate(x, y, PropertiesSettings.GetOrCreate(ReferenceHandling.References), true);
                Assert.AreNotSame(t1, t2);
            }
        }
    }
}