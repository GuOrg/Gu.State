// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.CopyTests.PropertyValues
{
    using System;

    using NUnit.Framework;

    public class Throws : ThrowTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excluded = null)
        {
            var builder = PropertiesSettings.Build();
            if (excluded != null)
            {
                _ = builder.IgnoreProperty<T>(excluded);
            }

            var settings = builder.CreateSettings(referenceHandling);
            Copy.PropertyValues(source, target, settings);
        }

        [Test]
        public void WithCalculatedPropertyThrows()
        {
            var expected = "Copy.PropertyValues(x, y) failed.\r\n" +
                           "The readonly property WithCalculatedProperty.CalculatedValue differs after copy.\r\n" +
                           " - Source value (int): 1.\r\n" +
                           " - Target value (int): 2.\r\n" +
                           "Below are a couple of suggestions that may solve the problem:\r\n" +
                           "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                           "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                           "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                           "  - ReferenceHandling.References means that references are copied.\r\n" +
                           "  - Exclude a combination of the following:\r\n" +
                           "    - The property WithCalculatedProperty.CalculatedValue.\r\n";

            var source = new CopyTypes.WithCalculatedProperty(1) { Value = 1 };
            var target = new CopyTypes.WithCalculatedProperty(2) { Value = 3 };
            var exception = Assert.Throws<InvalidOperationException>(() => Copy.PropertyValues(source, target));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}
