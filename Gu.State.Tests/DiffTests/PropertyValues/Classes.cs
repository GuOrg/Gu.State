namespace Gu.State.Tests.DiffTests.PropertyValues
{
    using System;

    public class Classes : ClassesTests
    {
        public override Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null)
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
            return DiffBy.PropertyValues(x, y, settings);
        }
    }
}
