namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static SynchronizeTypes;

    public static partial class SynchronizeTests
    {
        public static class Verify
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.References)]
            [TestCase(ReferenceHandling.Structural)]
            public static void WithSimpleProperties(ReferenceHandling referenceHandling)
            {
                Synchronize.VerifyCanSynchronize<WithSimpleProperties>(referenceHandling);
                Synchronize.VerifyCanSynchronize<WithSimpleProperties>(PropertiesSettings.GetOrCreate(referenceHandling));
                Synchronize.VerifyCanSynchronize(typeof(WithSimpleProperties), PropertiesSettings.GetOrCreate(referenceHandling));
            }
        }
    }
}
