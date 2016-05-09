namespace Gu.State.Tests
{
    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Caching
        {
            [Test]
            public void ReturnsSameForSameWhileAlive()
            {
                var x = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var t1 = Track.IsDirty(x, y);
                var t2 = Track.IsDirty(x, y);
                Assert.AreSame(t1, t2);
                t1.Dispose();
                t2.Dispose();

                var t3 = Track.IsDirty(x, y);
                Assert.AreNotSame(t1, t3);
            }

            [Test]
            public void ReturnsDifferentForDifferentPairs()
            {
                var x = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var t1 = Track.IsDirty(x, y);
                var t2 = Track.IsDirty(y, x);
                Assert.AreNotSame(t1, t2);
            }

            [Test]
            public void ReturnsDifferentForDifferentSettings()
            {
                var x = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var y = new DirtyTrackerTypes.SimpleDirtyTrackClass { Value1 = 1, Value2 = 2 };
                var t1 = Track.IsDirty(x, y, PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.Structural));
                var t2 = Track.IsDirty(x, y, PropertiesSettings.GetOrCreate(referenceHandling: ReferenceHandling.References));
                Assert.AreNotSame(t1, t2);
            }
        }
    }
}