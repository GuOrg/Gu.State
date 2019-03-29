namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System;

    public class ReferenceLoops : ReferenceLoopsTests
    {
        public override bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
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
            return State.EqualBy.PropertyValues(x, y, settings);
        }
    }
}