namespace Gu.State.Tests.DiffTests.PropertyValues
{
    using System;

    public class ReferenceLoops : ReferenceLoopsTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                _ = builder.IgnoreProperty<T>(excludedMembers);
            }

            if (excludedType != null)
            {
                _ = builder.IgnoreType(excludedType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.DiffBy.PropertyValues(x, y, settings);
        }
    }
}