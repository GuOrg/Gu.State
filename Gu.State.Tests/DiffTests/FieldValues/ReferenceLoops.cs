namespace Gu.State.Tests.DiffTests.FieldValues
{
    using System;

    public class ReferenceLoops : ReferenceLoopsTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
        {
            var builder = FieldsSettings.Build();
            if (excludedMembers != null)
            {
                _ = builder.AddIgnoredField<T>(excludedMembers);
            }

            if (excludedType != null)
            {
                _ = builder.AddImmutableType(excludedType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.DiffBy.FieldValues(x, y, settings);
        }
    }
}
