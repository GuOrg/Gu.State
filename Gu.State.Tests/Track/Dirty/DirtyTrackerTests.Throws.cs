﻿// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Throws
        {
            [Test]
            public void WithComplexProperty()
            {
                var expected = "Track.IsDirty(x, y) failed.\r\n" +
                               "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                               "Below are a couple of suggestions that may solve the problem:\r\n" +
                               "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                               "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                               "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                               "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property WithComplexProperty.ComplexType.\r\n" +
                               "    - The type ComplexType.\r\n";
                var x = new WithComplexProperty();
                var y = new WithComplexProperty();

                var exception = Assert.Throws<NotSupportedException>(() => Track.IsDirty(x, y, ReferenceHandling.Throw));
                Assert.AreEqual(expected, exception.Message);
                Assert.DoesNotThrow(() => Track.IsDirty(x, y, ReferenceHandling.Structural));
                Assert.DoesNotThrow(() => Track.IsDirty(x, y, ReferenceHandling.References));
            }
        }
    }
}
