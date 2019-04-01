namespace Gu.State.Tests.DiffTests.PropertyValues
{
    using System;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
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
            DiffBy.VerifyCanDiffByPropertyValues<T>(settings);
        }
    }
}