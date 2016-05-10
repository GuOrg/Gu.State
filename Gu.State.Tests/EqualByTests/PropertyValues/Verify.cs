namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
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
            EqualBy.VerifyCanEqualByPropertyValues<T>(settings);
        }
    }
}