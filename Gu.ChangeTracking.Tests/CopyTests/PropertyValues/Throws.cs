namespace Gu.ChangeTracking.Tests.CopyTests.PropertyValues
{
    using System;

    using NUnit.Framework;

    public class Throws : ThrowTests
    {
        public override void CopyMethod<T>(T source, T target)
        {
            Copy.PropertyValues(source, target);
        }

        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.PropertyValues(source, target, referenceHandling: referenceHandling);
        }

        public override void CopyMethod<T>(T source, T target, params string[] excluded)
        {
            Copy.PropertyValues(source, target, ignoreProperties: excluded);
        }

        [Test]
        public void WithCalculatedPropertyThrows()
        {
            var source = new CopyTypes.WithCalculatedProperty(1) { Value = 1 };
            var target = new CopyTypes.WithCalculatedProperty(2) { Value = 3 };
            var exception = Assert.Throws<InvalidOperationException>(() => Copy.PropertyValues(source, target));
            var expected = "Copy.PropertyValues(x, y) failed.\r\n" +
                           "The readonly property WithCalculatedProperty.CalculatedValue differs after copy.\r\n" +
                           " - Source value: 1.\r\n" +
                           " - Target value: 2.\r\n" +
                           "The property is of type int.\r\n" +
                           "Solve the problem by any of:\r\n" +
                           "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                           "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                           "  - ReferenceHandling.References means that references are copied.\r\n" +
                           "  - Exclude the type WithCalculatedProperty.\r\n" +
                           "  - Exclude the property WithCalculatedProperty.CalculatedValue.\r\n";
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
