namespace Gu.ChangeTracking.Tests.FieldValues
{
    using System;
    using System.Reflection;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public class Verify : Tests.VerifyTests
    {
        public override void VerifyMethod<T>()
        {
            Copy.VerifyCanCopyFieldValues<T>();
        }

        public override void VerifyMethod<T>(ReferenceHandling referenceHandling)
        {
            Copy.VerifyCanCopyFieldValues<T>(referenceHandling);
        }

        [Test]
        public void WithSimpleFieldsHappyPath()
        {
            Copy.VerifyCanCopyFieldValues<WithSimpleFields>();
            Copy.VerifyCanCopyFieldValues<WithSimpleFields>(BindingFlags.Public | BindingFlags.Instance);
            Copy.VerifyCanCopyFieldValues<WithSimpleFields>(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}