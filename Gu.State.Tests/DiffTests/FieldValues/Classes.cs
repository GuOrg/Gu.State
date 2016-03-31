namespace Gu.State.Tests.DiffTests.FieldValues
{
    using System;

    public class Classes : ClassesTests
    {
        public override Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null)
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
            return DiffBy.FieldValues(x, y, settings);
        }
    }
}
