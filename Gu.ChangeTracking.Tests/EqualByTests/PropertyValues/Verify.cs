namespace Gu.ChangeTracking.Tests.EqualByTests.PropertyValues
{
    using System;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                builder.AddIgnoredProperty<T>(excludedMembers);
            }

            if (excludedType != null)
            {
                builder.AddImmutableType(excludedType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            EqualBy.VerifyCanEqualByPropertyValues<T>(settings);
        }
    }
}