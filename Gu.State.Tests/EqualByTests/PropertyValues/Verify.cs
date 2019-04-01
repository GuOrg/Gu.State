namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using System;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                _ = builder.IgnoreProperty<T>(excludedMembers);
            }

            if (ignoredType != null)
            {
                builder.IgnoreType(ignoredType);
            }

            if (immutableType != null)
            {
                _ = builder.AddImmutableType(immutableType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            EqualBy.VerifyCanEqualByPropertyValues<T>(settings);
        }
    }
}