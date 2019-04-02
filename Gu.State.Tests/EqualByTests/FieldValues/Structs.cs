﻿namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using System;

    public class Structs : StructsTests
    {
        public override bool EqualBy<T>(
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
            return State.EqualBy.FieldValues(x, y, settings);
        }
    }
}