﻿namespace Gu.State.Tests.DiffTests.PropertyValues
{
    using System;

    public class Throws : ThrowsTests
    {
        public override Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                builder.IgnoreProperty<T>(excludedMembers);
            }

            if (excludedType != null)
            {
                builder.IgnoreType(excludedType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            return State.DiffBy.PropertyValues(x, y, settings);
        }
    }
}
