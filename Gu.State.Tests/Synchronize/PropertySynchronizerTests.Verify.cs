namespace Gu.State.Tests
{
    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class Verify
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public void WithSimpleProperties(ReferenceHandling referenceHandling)
            {
                Synchronize.VerifyCanSynchronize<DirtyTrackerTypes.WithSimpleProperties>(referenceHandling);
                Synchronize.VerifyCanSynchronize<DirtyTrackerTypes.WithSimpleProperties>(PropertiesSettings.GetOrCreate(referenceHandling));
                Synchronize.VerifyCanSynchronize(typeof(DirtyTrackerTypes.WithSimpleProperties), PropertiesSettings.GetOrCreate(referenceHandling));
            }
        }
    }
}