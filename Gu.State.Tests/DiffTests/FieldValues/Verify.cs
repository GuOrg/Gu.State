namespace Gu.State.Tests.DiffTests.FieldValues
{
    using System;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
        {
            var builder = FieldsSettings.Build();
            if (excludedMembers != null)
            {
                builder.AddIgnoredField<T>(excludedMembers);
            }

            if (excludedType != null)
            {
                builder.AddImmutableType(excludedType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            DiffBy.VerifyCanDiffByFieldValues<T>(settings);
        }
    }
}