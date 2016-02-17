namespace Gu.ChangeTracking.Tests
{
    using System;
    using Gu.ChangeTracking.Tests.DirtyTrackerStubs;

    using NUnit.Framework;

    public partial class DirtyTrackerTests
    {
        public class Complex
        {
            [Test]
            public void ThrowsWithoutReferenceHandling()
            {
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();
                var exception = Assert.Throws<NotSupportedException>(() => DirtyTracker.Track(x, y, ReferenceHandling.Structural));
                var expectedMessage = "Only equatable properties are supported without specifying ReferenceHandling\r\n" +
                                      "Property WithComplexProperty.ComplexType is not IEquatable<ComplexType>.\r\n" +
                                      "Use the overload DirtyTracker.Track(x, y, ReferenceHandling) if you want to track a graph";
                Assert.AreEqual(expectedMessage, exception.Message);
            }
        }
    }
}
