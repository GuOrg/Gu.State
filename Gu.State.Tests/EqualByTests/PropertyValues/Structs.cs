namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System;

    public class Structs : StructsTests
    {
        public override bool EqualMethod<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type ignoredType = null)
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
            return EqualBy.PropertyValues(x, y, settings);
        }
    }
}
