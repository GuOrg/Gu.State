namespace Gu.State.Tests.DiffTests.FieldValues
{
    using System;

    public class Classes : ClassesTests
    {
        public override Diff DiffBy<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
        {
            var builder = FieldsSettings.Build();
            if (excludedMembers != null)
            {
                _ = builder.AddIgnoredField<T>(excludedMembers);
            }

            if (ignoredType != null)
            {
                _ = builder.IgnoreType(ignoredType);
            }

            if (immutableType != null)
            {
                _ = builder.AddImmutableType(immutableType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.DiffBy.FieldValues(x, y, settings);
        }
    }
}
