namespace Gu.ChangeTracking.Tests.CopyTests.PropertyValues
{
    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>()
        {
            Copy.VerifyCanCopyPropertyValues<T>();
        }

        public override void VerifyMethod<T>(ReferenceHandling referenceHandling)
        {
            Copy.VerifyCanCopyPropertyValues<T>(referenceHandling);
        }
    }
}
