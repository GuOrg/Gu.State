namespace Gu.ChangeTracking.Tests.PropertyValues
{
    public class Verify : Tests.VerifyTests
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
