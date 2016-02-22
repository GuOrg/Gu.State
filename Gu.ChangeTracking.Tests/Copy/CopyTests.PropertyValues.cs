namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

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
            public void VerifyCanCopyPropertyWithComplexPropertyThrows()
            {
                var exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<WithComplexProperty>());
                var expected = "The property WithComplexProperty.ComplexType is not of a supported type.\r\n" +
                               "Expected struct or string but was: ComplexType\r\n" +
                               "Specify ReferenceHandling if you want to copy a graph.\r\n";
                Assert.AreEqual(expected, exception.Message);

                var settings = new CopyPropertiesSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
                exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<WithComplexProperty>(settings));
                Assert.AreEqual(expected, exception.Message);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.Reference)]
            public void VerifyCanCopyPropertyWithComplexPropertyDoesNotThrowWithReferenceHandling(ReferenceHandling referenceHandling)
            {
                var settings = new CopyPropertiesSettings(null, Constants.DefaultPropertyBindingFlags, referenceHandling);
                Copy.VerifyCanCopyPropertyValues<WithComplexProperty>(settings);
            }

            [Test]
            public void VerifyCanCopyPropertyListOfIntsThrows()
            {
                var exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<List<int>>());
                var expected = "Collections must be : IList and ReferenceHandling must be other than Throw";
                Assert.AreEqual(expected, exception.Message);

                var settings = new CopyPropertiesSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
                exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<List<int>>(settings));
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void VerifyCanCopyPropertyEnumerableOfIntsThrows()
            {
                var exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<IEnumerable<int>>());
                var expected = "Collections must be : IList and ReferenceHandling must be other than Throw";
                Assert.AreEqual(expected, exception.Message);

                var settings = new CopyPropertiesSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Throw);
                exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<IEnumerable<int>>(settings));
                Assert.AreEqual(expected, exception.Message);

                settings = new CopyPropertiesSettings(null, Constants.DefaultPropertyBindingFlags, ReferenceHandling.Structural);
                exception = Assert.Throws<NotSupportedException>(() => Copy.VerifyCanCopyPropertyValues<IEnumerable<int>>(settings));
                Assert.AreEqual(expected, exception.Message);
            }

            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.Reference)]
            public void VerifyCanCopyPropertyListOfIntsWithReferenceHandling(ReferenceHandling referenceHandling)
            {
                var settings = new CopyPropertiesSettings(null, Constants.DefaultPropertyBindingFlags, referenceHandling);
                Copy.VerifyCanCopyPropertyValues<List<int>>(settings);
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
            public void WithComplexPropertyThrowsWithoutReferenceHandling()
            {
                var source = new WithComplexProperty();
                var target = new WithComplexProperty();
                var exception = Assert.Throws<NotSupportedException>(() => Copy.PropertyValues(source, target));
                var expectedMessage = "Only properties with types struct or string are supported without specifying ReferenceHandling\r\n" +
                                      "Property WithComplexProperty.ComplexType is a reference type (ComplexType).\r\n" +
                                      "Use the overload Copy.PropertyValues(source, target, ReferenceHandling) if you want to copy a graph";
                Assert.AreEqual(expectedMessage, exception.Message);
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
            public void WithArrayPropertyWhenTargetArrayIsNullStructural()
            {
                var source = new WithArrayProperty("a", 1, new[] { 1, 2 });
                var target = new WithArrayProperty("a", 1, null);
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual("a", source.Name);
                Assert.AreEqual("a", target.Name);
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreNotSame(source.Array, target.Array);
                CollectionAssert.AreEqual(new[] { 1, 2 }, source.Array);
                CollectionAssert.AreEqual(new[] { 1, 2 }, target.Array);
            }

            [Test]
            public void WithArrayPropertyWhenTargetArrayIsNullReference()
            {
                var source = new WithArrayProperty("a", 1, new[] { 1, 2 });
                var target = new WithArrayProperty("a", 1, null);
                Copy.PropertyValues(source, target, ReferenceHandling.Reference);
                Assert.AreEqual("a", source.Name);
                Assert.AreEqual("a", target.Name);
                Assert.AreEqual(1, source.Value);
                Assert.AreEqual(1, target.Value);
                Assert.AreSame(source.Array, target.Array);
                CollectionAssert.AreEqual(new[] { 1, 2 }, source.Array);
            }

            [Test]
            public void ListOfIntsToEmpty()
            {
                var source = new List<int> { 1, 2, 3 };
                var target = new List<int>();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void ListOfIntsToLonger()
            {
                var source = new List<int> { 1, 2, 3 };
                var target = new List<int> { 1, 2, 3, 4 };
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void ObservableCollectionOfIntsToEmpty()
            {
                var source = new ObservableCollection<int> { 1, 2, 3 };
                var target = new ObservableCollection<int>();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void ObservableCollectionOfIntsToLonger()
            {
                var source = new ObservableCollection<int> { 1, 2, 3 };
                var target = new ObservableCollection<int> { 1, 2, 3, 4 };
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void WithListOfIntsPropertyToEmpty()
            {
                var source = new WithListProperty<int>();
                source.Items.AddRange(new[] { 1, 2, 3 });
                var target = new WithListProperty<int>();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source.Items, target.Items);
            }

            [Test]
            public void WithListOfIntsPropertyToLonger()
            {
                var source = new WithListProperty<int>();
                source.Items.AddRange(new[] { 1, 2, 3 });
                var target = new WithListProperty<int>();
                target.Items.AddRange(new[] { 1, 2, 3, 4 });
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source.Items, target.Items);
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

            [Test]
            public void WithListOfComplexPropertyToEmptyStructural()
            {
                var source = new WithListProperty<ComplexType>();
                source.Items.Add(new ComplexType("a", 1));
                var target = new WithListProperty<ComplexType>();
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                var expected = new[] { new ComplexType("a", 1) };
                CollectionAssert.AreEqual(expected, source.Items, ComplexType.Comparer);
                CollectionAssert.AreEqual(expected, target.Items, ComplexType.Comparer);
                Assert.AreNotSame(source.Items[0], target.Items[0]);
            }

            [Test]
            public void WithListOfComplexPropertyToEmptyReference()
            {
                Assert.Inconclusive("Not sure how to handle this");
                var source = new WithListProperty<ComplexType>();
                source.Items.Add(new ComplexType("a", 1));
                var target = new WithListProperty<ComplexType>();
                Copy.PropertyValues(source, target, ReferenceHandling.Reference);
                var expected = new[] { new ComplexType("a", 1) };
                CollectionAssert.AreEqual(expected, source.Items, ComplexType.Comparer);
                CollectionAssert.AreEqual(expected, target.Items, ComplexType.Comparer);
                Assert.AreSame(source.Items[0], target.Items[0]);
            }

            [Test]
            public void WithListOfComplexPropertyToLonger()
            {
                var source = new WithListProperty<ComplexType>();
                source.Items.Add(new ComplexType("a", 1));
                var target = new WithListProperty<ComplexType>();
                target.Items.AddRange(new[] { new ComplexType("b", 2), new ComplexType("c", 3) });
                var item = target.Items[0];
                Copy.PropertyValues(source, target, ReferenceHandling.Structural);
                var expected = new[] { new ComplexType("a", 1) };
                CollectionAssert.AreEqual(expected, source.Items, ComplexType.Comparer);
                CollectionAssert.AreEqual(expected, target.Items, ComplexType.Comparer);
                Assert.AreSame(item, target.Items[0]);
            }
        }
    }
}
