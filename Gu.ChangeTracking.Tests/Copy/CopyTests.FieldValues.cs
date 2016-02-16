namespace Gu.ChangeTracking.Tests
{
    using System;
    using System.Collections.Generic;

    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public partial class CopyTests
    {
        public class FieldValues
        {
            [Test]
            public void VerifyCanCopyFieldValuesHappyPath()
            {
                Copy.VerifyCanCopyFieldValues<WithSimpleProperties>();
            }

            [Test]
            public void FieldValuesHappyPath()
            {
                var source = new WithSimpleProperties
                {
                    IntValue = 1,
                    NullableIntValue = 2,
                    StringValue = "3",
                    EnumValue = StringSplitOptions.RemoveEmptyEntries
                };

                var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
                Copy.FieldValues(source, target);
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
            public void ReadonlyFieldHappyPath()
            {
                var x = new WithReadonlyField(1, 2);
                var y = new WithReadonlyField(1, 3);
                Copy.FieldValues(x, y);
                Assert.AreEqual(1, x.ReadonlyValue);
                Assert.AreEqual(1, y.ReadonlyValue);
                Assert.AreEqual(2, x.Value);
                Assert.AreEqual(2, y.Value);
            }

            [Test]
            public void ReadonlyFieldThrows()
            {
                var x = new WithReadonlyField(1, 2);
                var y = new WithReadonlyField(3, 4);
                var exception = Assert.Throws<InvalidOperationException>(() => Copy.FieldValues(x, y));
                var expected = "Field WithReadonlyField.ReadonlyValue differs but cannot be updated because it is readonly.\r\n" +
                               "Provide Copy.FieldValues(x, y, nameof(WithReadonlyField.ReadonlyValue))";
                Assert.AreEqual(expected, exception.Message);
            }

            [Test]
            public void FieldValuesIgnores()
            {
                var source = new WithSimpleProperties
                {
                    IntValue = 1,
                    NullableIntValue = 2,
                    StringValue = "3",
                    EnumValue = StringSplitOptions.RemoveEmptyEntries
                };
                var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
                Copy.FieldValues(source, target, "nullableIntValue");
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
            public void WithComplexFieldHappyPathStructural()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "b", Value = 2 } };
                var target = new WithComplexProperty();
                Copy.FieldValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.AreEqual(source.ComplexType.Name, target.ComplexType.Name);
                Assert.AreEqual(source.ComplexType.Value, target.ComplexType.Value);
            }

            [Test]
            public void WithComplexFieldHappyPathWhenNullStructural()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1 };
                var target = new WithComplexProperty();
                Copy.FieldValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.IsNull(source.ComplexType);
                Assert.IsNull(target.ComplexType);
            }

            [Test]
            public void WithComplexFieldHappyPathReference()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "b", Value = 2 } };
                var target = new WithComplexProperty();
                Copy.FieldValues(source, target, ReferenceHandling.Reference);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.AreSame(source.ComplexType, target.ComplexType);
            }

            [Test]
            public void WithComplexFieldHappyPathWhenNullReference()
            {
                var source = new WithComplexProperty { Name = "a", Value = 1 };
                var target = new WithComplexProperty();
                Copy.FieldValues(source, target, ReferenceHandling.Reference);
                Assert.AreEqual(source.Name, target.Name);
                Assert.AreEqual(source.Value, target.Value);
                Assert.IsNull(source.ComplexType);
                Assert.IsNull(target.ComplexType);
            }

            [Test]
            public void ListOfIntsToEmpty()
            {
                var source = new List<int> { 1, 2, 3 };
                var target = new List<int>();
                Copy.FieldValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }

            [Test]
            public void ListOfIntsToLonger()
            {
                var source = new List<int> { 1, 2, 3 };
                var target = new List<int> { 1, 2, 3, 4 };
                Copy.FieldValues(source, target, ReferenceHandling.Structural);
                CollectionAssert.AreEqual(source, target);
            }


            [Test]
            public void ListOfComplexToEmpty()
            {
                var source = new List<ComplexType> { new ComplexType("a", 1) };
                var target = new List<ComplexType>();
                Copy.FieldValues(source, target, ReferenceHandling.Structural);
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
                Copy.FieldValues(source, target, ReferenceHandling.Structural);
                Assert.AreEqual(1, source.Count);
                Assert.AreEqual(1, target.Count);
                Assert.AreEqual(source[0].Name, target[0].Name);
                Assert.AreEqual(source[0].Value, target[0].Value);
                Assert.AreSame(item, target[0]);
            }
        }
    }
}