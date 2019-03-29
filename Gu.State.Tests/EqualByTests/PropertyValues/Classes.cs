namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System;
    using System.Collections.Generic;

    public class Classes : ClassesTests
    {
        public static new IReadOnlyList<EqualByTestsShared.EqualsData> EqualsSource => EqualByTestsShared.EqualsSource;

        public override bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type ignoredType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                builder.IgnoreProperty<T>(excludedMembers);
            }

            if (ignoredType != null)
            {
                builder.IgnoreType(ignoredType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.EqualBy.PropertyValues(x, y, settings);
        }
    }
}
