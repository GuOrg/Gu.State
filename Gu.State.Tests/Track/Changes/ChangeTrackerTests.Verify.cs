namespace Gu.State.Tests
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using static ChangeTrackerTypes;

    public partial class ChangeTrackerTests
    {
        class Verify
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void WithSimpleProperties(ReferenceHandling referenceHandling)
            {
                Track.VerifyCanTrackChanges<WithSimpleProperties>(referenceHandling);
                Track.VerifyCanTrackChanges<WithSimpleProperties>(PropertiesSettings.GetOrCreate(referenceHandling));
                Track.VerifyCanTrackChanges(typeof(WithSimpleProperties), PropertiesSettings.GetOrCreate(referenceHandling));
            }
        }
    }
}