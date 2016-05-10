namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using System;
    using System.Collections.Generic;

    public class Classes : ClassesTests
    {
        public override bool EqualMethod<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
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
            return EqualBy.FieldValues(x, y, settings);
        }

        public new static IReadOnlyList<EqualByTestsShared.EqualsData> EqualsSource => EqualByTestsShared.EqualsSource;
    }
}
