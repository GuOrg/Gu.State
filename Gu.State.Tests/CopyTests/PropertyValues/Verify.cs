namespace Gu.State.Tests.CopyTests.PropertyValues
{
    using System;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>()
        {
            Copy.VerifyCanCopyPropertyValues<T>();
        }

        public override void VerifyMethod<T>(
            ReferenceHandling referenceHandling,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
        {
            var builder = PropertiesSettings.Build();
            if (excludedMembers != null)
            {
                builder.IgnoreProperty<T>(excludedMembers);
            }

            if (ignoredType != null)
            {
                builder.AddImmutableType(ignoredType);
            }

            if (immutableType != null)
            {
                builder.AddImmutableType(immutableType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            Copy.VerifyCanCopyPropertyValues<T>(settings);
        }
    }
}
