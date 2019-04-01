namespace Gu.State.Tests.CopyTests.FieldValues
{
    using System;

    using NUnit.Framework;

    using static CopyTypes;

    public class Classes : ClassesTests
    {
        public override void CopyMethod<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excluded = null,
            Type ignoredType = null,
            Type immutableType = null)
        {
            var builder = FieldsSettings.Build();
            if (excluded != null)
            {
                _ = builder.AddIgnoredField<T>(excluded);
            }

            if (ignoredType != null)
            {
                _ = builder.IgnoreType(ignoredType);
            }

            if (immutableType != null)
            {
                _ = builder.AddImmutableType(immutableType);
            }

            var settings = builder.CreateSettings(referenceHandling);
            Copy.FieldValues(source, target, settings);
        }

        [Test]
        public void WithReadonlyFieldThrows()
        {
            var x = new WithReadonlyField(1, 2);
            var y = new WithReadonlyField(3, 4);
            var expected = "Copy.FieldValues(x, y) failed.\r\n" +
                           "The readonly field WithReadonlyField.ReadonlyValue differs after copy.\r\n" +
                           " - Source value (int): 1.\r\n" +
                           " - Target value (int): 3.\r\n" +
                           "Solve the problem by any of:\r\n" +
                           "* Use FieldsSettings and specify how copying is performed:\r\n" +
                           "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                           "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                           "  - ReferenceHandling.References means that references are copied.\r\n" +
                           "  - Exclude a combination of the following:\r\n" +
                           "    - The field WithReadonlyField.ReadonlyValue.\r\n";

            var exception = Assert.Throws<InvalidOperationException>(() => Copy.FieldValues(x, y));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}