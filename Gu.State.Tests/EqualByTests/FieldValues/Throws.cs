namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using System;

    public class Throws : ThrowsTests
    {
        public override bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
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
            return State.EqualBy.FieldValues(x, y, settings);
        }
    }
}
