namespace Gu.State.Tests.CopyTests.FieldValues
{
    using System.Reflection;

    using NUnit.Framework;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>()
        {
            Copy.VerifyCanCopyFieldValues<T>();
        }

        public override void VerifyMethod<T>(ReferenceHandling referenceHandling)
        {
            Copy.VerifyCanCopyFieldValues<T>(referenceHandling: referenceHandling);
        }

        [Test]
        public void WithSimpleFieldsHappyPath()
        {
            Copy.VerifyCanCopyFieldValues<CopyTypes.WithSimpleFields>();
            Copy.VerifyCanCopyFieldValues<CopyTypes.WithSimpleFields>(BindingFlags.Public | BindingFlags.Instance);
            Copy.VerifyCanCopyFieldValues<CopyTypes.WithSimpleFields>(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}