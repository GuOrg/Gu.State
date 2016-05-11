namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static DirtyTrackerTypes;

    public partial class DirtyTrackerTests
    {
        public class Verify
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void WithSimpleProperties(ReferenceHandling referenceHandling)
            {
                Track.VerifyCanTrackIsDirty<WithSimpleProperties>(referenceHandling);
                Track.VerifyCanTrackIsDirty<WithSimpleProperties>(PropertiesSettings.GetOrCreate(referenceHandling));
                Track.VerifyCanTrackIsDirty(typeof(WithSimpleProperties), PropertiesSettings.GetOrCreate(referenceHandling));
            }
        }
    }
}