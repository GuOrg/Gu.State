﻿namespace Gu.State.Tests.DiffTests.FieldValues
{
    using System;

    public class Throws : ThrowsTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
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
            return State.DiffBy.FieldValues(x, y, settings);
        }
    }
}
