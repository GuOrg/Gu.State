namespace Gu.State.Tests.DiffTests.PropertyValues
{
    using System;

    public class Structs : StructsTests
    {
        public override Diff DiffBy<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
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

            if (immutableType != null)
            {
                _ = builder.AddImmutableType(immutableType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.DiffBy.PropertyValues(x, y, settings);
        }
    }
}
