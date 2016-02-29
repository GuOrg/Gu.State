namespace Gu.ChangeTracking.Tests.FieldValues
{
    using System;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public class Throws : ThrowTests
    {
        public override void CopyMethod<T>(T source, T target)
        {
            Copy.FieldValues(source, target);
        }

        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.FieldValues(source, target, referenceHandling);
        }

        public override void CopyMethod<T>(T source, T target, params string[] excluded)
        {
            Copy.FieldValues(source, target, excluded);
        }
    }
}
