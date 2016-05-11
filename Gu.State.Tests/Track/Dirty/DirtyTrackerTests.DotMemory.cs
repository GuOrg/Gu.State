namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using JetBrains.dotMemoryUnit;
    using JetBrains.dotMemoryUnit.Kernel;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class DotMemory
        {
            [SetUp]
            public void SetUp()
            {
#if (DEBUG)
                Assert.Inconclusive("debug build keeps instances alive longer for nicer debugging experience");
#endif
            }

            [Test]
            public void ComplexType()
            {
                var x = new ComplexType("a", 1);
                var y = new ComplexType("a", 1);

                using (var tracker = Track.IsDirty(x, y))
                {
                    Assert.AreEqual(false, tracker.IsDirty);
                }

                x = null;
                y = null;
                dotMemory.Check(m => Assert.AreEqual(0, m.GetObjects(o => o.Type.Is<ComplexType>()).ObjectsCount));
            }

            [Test]
            public void WithComplexProperty()
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var settings = PropertiesSettings.GetOrCreate();
                using (var tracker = Track.IsDirty(x, y, settings))
                {
                    Assert.AreEqual(false, tracker.IsDirty);
                }

                x = null;
                y = null;
                dotMemory.Check(m => Assert.AreEqual(0, m.GetObjects(o => o.Type.Is<WithComplexProperty>()).ObjectsCount));
                dotMemory.Check(m => Assert.AreEqual(0, m.GetObjects(o => o.Type.Is<ComplexType>()).ObjectsCount));
            }

            [Test]
            public void DoesNotLeakTrackedProperty()
            {
                var x = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };
                var y = new WithComplexProperty { ComplexType = new ComplexType("a", 1) };

                using (var tracker = Track.IsDirty(x, y))
                {
                    Assert.AreEqual(false, tracker.IsDirty);
                    dotMemory.Check(m => Assert.AreEqual(2, m.GetObjects(o => o.Type.Is<ComplexType>()).ObjectsCount));

                    x.ComplexType = null;
                    dotMemory.Check(m => Assert.AreEqual(1, m.GetObjects(o => o.Type.Is<ComplexType>()).ObjectsCount));

                    y.ComplexType = null;
                    dotMemory.Check(m => Assert.AreEqual(0, m.GetObjects(o => o.Type.Is<ComplexType>()).ObjectsCount));
                }
            }
        }
    }
}