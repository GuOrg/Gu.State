// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.CopyTests.FieldValues
{
    using System;
    using System.Reflection;

    using NUnit.Framework;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>()
        {
            Copy.VerifyCanCopyFieldValues<T>();
        }

        public override void VerifyMethod<T>(
            ReferenceHandling referenceHandling,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
        {
            var builder = FieldsSettings.Build();
            if (excludedMembers != null)
            {
                builder.AddIgnoredField<T>(excludedMembers);
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
            Copy.VerifyCanCopyFieldValues<T>(settings);
        }

        [Test]
        public void WithSimpleFieldsHappyPath()
        {
            Copy.VerifyCanCopyFieldValues<CopyTypes.WithSimpleFields>();
            Copy.VerifyCanCopyFieldValues<CopyTypes.WithSimpleFields>(ReferenceHandling.Structural, BindingFlags.Public | BindingFlags.Instance);
            Copy.VerifyCanCopyFieldValues<CopyTypes.WithSimpleFields>(ReferenceHandling.Structural, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}