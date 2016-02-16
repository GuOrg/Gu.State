namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

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
                Assert.Inconclusive("", exception.Message);
            }

            [Test]
            public void WithComplexPropertyHappyPath()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "b", Value = 2 } };
                var target = new WithComplexProperty();
                var copyProperty = SpecialCopyProperty.CreateClone<WithComplexProperty, ComplexType>(
                    x => x.ComplexType,
                    () => new ComplexType());

                Copy.PropertyValues(source, target, new[] { copyProperty });
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.AreEqual(source.ComplexType.Name, target.ComplexType.Name);
                Assert.AreEqual(source.ComplexType.Value, target.ComplexType.Value);
            }

            [Test]
            public void WithComplexPropertyHappyPathWhenNull()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1 };
                var target = new WithComplexProperty();
                var copyProperty = SpecialCopyProperty.CreateClone<WithComplexProperty, ComplexType>(
                    x => x.ComplexType,
                    () => new ComplexType());

                Copy.PropertyValues(source, target, new[] { copyProperty });
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.IsNull(source.ComplexType);
                Assert.IsNull(target.ComplexType);
            }

            [Test]
            public void WithComplexPropertyHappyPathStructural()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "b", Value = 2 } };
                var target = new WithComplexProperty();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.AreEqual(source.ComplexType.Name, target.ComplexType.Name);
                Assert.AreEqual(source.ComplexType.Value, target.ComplexType.Value);
            }

            [Test]
            public void WithComplexPropertyHappyPathWhenNullStructural()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1 };
                var target = new WithComplexProperty();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.IsNull(source.ComplexType);
                Assert.IsNull(target.ComplexType);
            }

            [Test]
            public void WithComplexPropertyHappyPathReference()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "b", Value = 2 } };
                var target = new WithComplexProperty();
                Copy.PropertyValues(source, target, ReferenceHandling.Reference);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.AreSame(source.ComplexType, target.ComplexType);
            }

            [Test]
            public void WithComplexPropertyHappyPathWhenNullReference()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1 };
                var target = new WithComplexProperty();
                Copy.PropertyValues(source, target, ReferenceHandling.Reference);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.IsNull(source.ComplexType);
                Assert.IsNull(target.ComplexType);
            }

            [Test]
            public void ListToEmpty()
            {
                var source = new List<int> { 1, 2, 3 };
                var target = new List<int>();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void ListToLonger()
            {
                var source = new List<int> { 1, 2, 3 };
                var target = new List<int> { 1, 2, 3, 4 };
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void ListOfComplexToEmpty()
            {
                var source = new List<ComplexType> { new ComplexType("a", 1) };
                var target = new List<ComplexType>();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(1, source.Count);
                Assert.AreEqual(1, target.Count);
                Assert.AreEqual(source[0].Name, target[0].Name);
                Assert.AreEqual(source[0].Value, target[0].Value);
            }

            [Test]
            public void ListOfComplexToLonger()
            {
                var source = new List<ComplexType> { new ComplexType("a", 1) };
                var target = new List<ComplexType> { new ComplexType("b", 2), new ComplexType("c", 3) };
                var item = target[0];
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(1, source.Count);
                Assert.AreEqual(1, target.Count);
                Assert.AreEqual(source[0].Name, target[0].Name);
                Assert.AreEqual(source[0].Value, target[0].Value);
                Assert.AreSame(item, target[0]);
            }
        }
    }
}
