namespace Gu.State.Tests.DiffTests.FieldValues
{
    using System;

    using NUnit.Framework;

    public class Verify : VerifyTests
    {
        public override void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null)
        {
            Assert.Inconclusive();
            //var builder = FieldsSettings.Build();
            //if (excludedMembers != null)
            //{
            //    builder.AddIgnoredField<T>(excludedMembers);
            //}

            //if (excludedType != null)
            //{
            //    builder.AddImmutableType(excludedType);
            //}

            //var settings = builder.CreateSettings(referenceHandling);
            //DiffBy.VerifyCanDiffByFieldValues<T>(settings);
        }
    }
}