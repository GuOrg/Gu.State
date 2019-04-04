// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;

    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Verify
        {
            [Test]
            public void WithComplexProperty()
            {
                var expected = "Track.VerifyCanTrackIsDirty(x, y) failed.\r\n" +
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

                var exception = Assert.Throws<NotSupportedException>(() => Track.VerifyCanTrackIsDirty<WithComplexProperty>(ReferenceHandling.Throw));
                Assert.AreEqual(expected, exception.Message);
                exception = Assert.Throws<NotSupportedException>(() => Track.VerifyCanTrackIsDirty(typeof(WithComplexProperty), PropertiesSettings.GetOrCreate(ReferenceHandling.Throw)));
                Assert.AreEqual(expected, exception.Message);

                Assert.DoesNotThrow(() => Track.VerifyCanTrackIsDirty<WithComplexProperty>(ReferenceHandling.Structural));
                Assert.DoesNotThrow(() => Track.VerifyCanTrackIsDirty<WithComplexProperty>(ReferenceHandling.References));
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void WithSimpleProperties(ReferenceHandling referenceHandling)
            {
                Track.VerifyCanTrackIsDirty<WithSimpleProperties>(referenceHandling);
                Track.VerifyCanTrackIsDirty<WithSimpleProperties>(PropertiesSettings.GetOrCreate(referenceHandling));
                Track.VerifyCanTrackIsDirty(typeof(WithSimpleProperties), PropertiesSettings.GetOrCreate(referenceHandling));
            }

            [Test]
            public void WithExplicitImmutableAndComparer()
            {
                var settings = PropertiesSettings.Build()
                                                 .AddImmutableType<IntCollection>()
                                                 .AddComparer(IntCollection.Comparer)
                                                 .CreateSettings();
                Track.VerifyCanTrackIsDirty<With<IntCollection>>(settings);
            }
        }
    }
}