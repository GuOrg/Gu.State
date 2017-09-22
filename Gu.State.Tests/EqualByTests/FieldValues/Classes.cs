namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using System;
    using System.Collections.Generic;

    public class Classes : ClassesTests
    {
        public static new IReadOnlyList<EqualByTestsShared.EqualsData> EqualsSource => EqualByTestsShared.EqualsSource;

        public override bool EqualMethod<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null)
        {
            var builder = FieldsSettings.Build();
            if (excludedMembers != null)
            {
                builder.AddIgnoredField<T>(excludedMembers);
            }

            if (ignoredType != null)
            {
                builder.IgnoreType(ignoredType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return EqualBy.FieldValues(x, y, settings);
        }
    }
}
