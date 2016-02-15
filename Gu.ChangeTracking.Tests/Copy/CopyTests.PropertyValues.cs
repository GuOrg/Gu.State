namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public partial class CopyTests
    {
        public class PropertyValues
        {
            [Test]
            public void VerifyCanCopyPropertyValuesHappyPath()
            {
                Copy.VerifyCanCopyPropertyValues<WithSimpleProperties>();
            }

            [Test]
            public void VerifyCanCopyPropertyWithCalculatedProperty()
            {
                Copy.VerifyCanCopyPropertyValues<WithCalculatedProperty>();
            }

            [Test]
            public void PropertyValuesHappyPath()
            {
                var source = new WithSimpleProperties
                {
                    IntValue = 1,
                    NullableIntValue = 2,
                    StringValue = "3",
                    EnumValue = StringSplitOptions.RemoveEmptyEntries
                };
                var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
                Copy.PropertyValues(source, target);
                Assert.AreEqual(1, source.IntValue);
                Assert.AreEqual(1, target.IntValue);
                Assert.AreEqual(2, source.NullableIntValue);
                Assert.AreEqual(2, target.NullableIntValue);
                Assert.AreEqual("3", source.StringValue);
                Assert.AreEqual("3", target.StringValue);
                Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, source.EnumValue);
                Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, target.EnumValue);
            }

            [Test]
            public void WithCalculatedPropertyHappyPath()
            {
                var source = new WithCalculatedProperty { Value = 1 };
                var target = new WithCalculatedProperty { Value = 3 };
                Copy.PropertyValues(source, target);
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreEqual(1, source.CalculatedValue);
                Assert.AreEqual(1, target.CalculatedValue);
            }

            [Test]
            public void WithCalculatedPropertyThrows()
            {
                var source = new WithCalculatedProperty(1) { Value = 1 };
                var target = new WithCalculatedProperty(2) { Value = 3 };
                var exception = Assert.Throws<InvalidOperationException>(() => Copy.PropertyValues(source, target));
                var expected = "Value differs for readonly property WithCalculatedProperty.CalculatedValue";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void PropertyValuesIgnores()
            {
                var source = new WithSimpleProperties
                {
                    IntValue = 1,
                    NullableIntValue = 2,
                    StringValue = "3",
                    EnumValue = StringSplitOptions.RemoveEmptyEntries
                };
                var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
                Copy.PropertyValues(source, target, nameof(WithSimpleProperties.NullableIntValue));
                Assert.AreEqual(1, source.IntValue);
                Assert.AreEqual(1, target.IntValue);
                Assert.AreEqual(2, source.NullableIntValue);
                Assert.AreEqual(4, target.NullableIntValue);
                Assert.AreEqual("3", source.StringValue);
                Assert.AreEqual("3", target.StringValue);
                Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, source.EnumValue);
                Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, target.EnumValue);
            }

            [Test]
            public void WithComplexPropertyThrowsWithoutSpecialHandling()
            {
                var source = new WithComplexProperty();
                var target = new WithComplexProperty();
                var exception = Assert.Throws<InvalidOperationException>(() => Copy.PropertyValues(source, target));
                Assert.AreEqual("", exception.Message);
            }

            [Test]
            public void WithComplexPropertyHappyPath()
            {
                var source = new WithComplexProperty {Name = "a", Value = 1, ComplexType = new ComplexType {Name = "b", Value = 2} };
                var target = new WithComplexProperty();
                var copyProperty = SpecialCopyProperty.Create<WithComplexProperty, ComplexType>(
                    x => x.ComplexType,
                    c =>
                        {
                            var t = new ComplexType();
                            Copy.PropertyValues(c, t);
                            return t;
                        });

                Copy.PropertyValues(source, target, copyProperty);

            }
        }
    }
}
