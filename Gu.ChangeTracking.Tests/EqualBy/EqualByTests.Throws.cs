namespace Gu.ChangeTracking.Tests
{
    using System;

    using NUnit.Framework;

    public partial class EqualByTests
    {
        public class Throws
        {
            [Test]
            public void FieldValuesWithComplexValueThrowsWithoutReferenceHandling()
            {
                var expected = "EqualBy.FieldValues(x, y) does not support comparing the field WithComplexValue.complexValue.\r\n" +
                               "The field is of type ComplexType.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                               "* Use EqualByFieldsSettings to specify ReferenceHandling\r\n" +
                               "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                               "  - ReferenceHandling.References means that reference equality is used.\r\n";
                var exception = Assert.Throws<NotSupportedException>(() => EqualBy.FieldValues<WithComplexValue>(null, null));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void PropertyValuesWithComplexValueThrowsWithoutReferenceHandling()
            {
                var expected = "EqualBy.PropertyValues(x, y) does not support comparing the property WithComplexValue.ComplexValue.\r\n" +
                               "The property is of type ComplexType.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                               "* Use EqualByFieldsSettings to specify ReferenceHandling\r\n" +
                               "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                               "  - ReferenceHandling.References means that reference equality is used.\r\n";
                var exception = Assert.Throws<NotSupportedException>(() => EqualBy.PropertyValues<WithComplexValue>(null, null));
                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}