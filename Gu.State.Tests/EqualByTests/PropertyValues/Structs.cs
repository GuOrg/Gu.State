namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System;

    public class Structs : StructsTests
    {
        public override bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type ignoredType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                _ = builder.IgnoreProperty<T>(excludedMembers);
            }

            if (ignoredType != null)
            {
                _ = builder.IgnoreType(ignoredType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.EqualBy.PropertyValues(x, y, settings);
        }
    }
}
