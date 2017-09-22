// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static DiffTypes;

    public abstract class ClassesTests
    {
        public abstract Diff DiffMethod<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
            where T : class;

        [TestCase("b", "b", "Empty")]
        [TestCase("b", "c", "WithSimpleProperties <member> x: b y: c")]
        public void WithSimpleHappyPath(string xn, string yn, string expected)
        {
            expected = expected?.Replace(
                "<member>",
                this is FieldValues.Classes
                    ? "stringValue"
                    : "StringValue");
            var x = new WithSimpleProperties(1, 2, xn, StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(1, 2, yn, StringSplitOptions.RemoveEmptyEntries);
            var result = this.DiffMethod(x, y);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffMethod(x, y, ReferenceHandling.Throw);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase("b", "b", "Empty")]
        [TestCase("b", "c", "WithComplexProperty <member1> <member2> x: b y: c")]
        public void WithComplexStructural(string xn, string yn, string expected)
        {
            expected = this is FieldValues.Classes
                           ? expected.Replace("<member1>", "complexType")
                                     .Replace("<member2>", "name")
                           : expected.Replace("<member1>", "ComplexType")
                                     .Replace("<member2>", "Name");
            var x = new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = xn, Value = 2 } };

            var y = new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = yn, Value = 2 } };

            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void WithComplexStructuralWhenNull()
        {
            var x = new WithComplexProperty { Name = "a", Value = 1 };
            var y = new WithComplexProperty { Name = "a", Value = 1 };
            this.DiffMethod(x, y, ReferenceHandling.Structural);
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void WithComplexStructuralWhenXIsNull()
        {
            var expected = this is FieldValues.Classes
                               ? "WithComplexProperty complexType x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: null"
                               : "WithComplexProperty ComplexType x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: null";
            var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType("b", 1) };
            var y = new WithComplexProperty { Name = "a", Value = 1 };
            this.DiffMethod(x, y, ReferenceHandling.Structural);
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void WithComplexStructuralWhenYIsNull()
        {
            var expected = this is FieldValues.Classes
                               ? "WithComplexProperty complexType x: null y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType"
                               : "WithComplexProperty ComplexType x: null y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";
            var x = new WithComplexProperty { Name = "a", Value = 1 };
            var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType("b", 1) };
            this.DiffMethod(x, y, ReferenceHandling.Structural);
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithComplexReferenceWhenSame(ReferenceHandling referenceHandling)
        {
            var x = new WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new ComplexType { Name = "b", Value = 2 }
            };
            var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = x.ComplexType };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Structural, "Empty")]
        [TestCase(ReferenceHandling.References, "WithComplexProperty <member> x: Gu.State.Tests.DiffTests.DiffTypes+ComplexType y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType")]
        public void WithComplexReferenceWhenNotSame(ReferenceHandling referenceHandling, string expected)
        {
            expected = expected?.Replace(
                "<member>",
                this is FieldValues.Classes
                    ? "complexType"
                    : "ComplexType");
            var x = new WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new ComplexType { Name = "b", Value = 2 }
            };
            var y = new WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new ComplexType { Name = "b", Value = 2 }
            };
            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(expected == "Empty", result.IsEmpty);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void EqualWhenSameType()
        {
            var x = new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 });
            var y = new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 });
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString(string.Empty, " "));
        }

        [Test]
        public void NotEqualWhenSameType()
        {
            var x = new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 2 });
            var y = new With<BaseClass>(new Derived1 { BaseValue = 1, Derived1Value = 3 });
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            var expected = this is PropertyValues.Classes
                               ? "With<BaseClass> Value Derived1Value x: 2 y: 3"
                               : "With<BaseClass> <Value>k__BackingField <Derived1Value>k__BackingField x: 2 y: 3";
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NotEqualWhenDifferentTypes()
        {
            var x = new With<BaseClass>(new Derived1());
            var y = new With<BaseClass>(new Derived2());
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            var expected = this is PropertyValues.Classes
                               ? "With<BaseClass> Value x: Gu.State.Tests.DiffTests.DiffTypes+Derived1 y: Gu.State.Tests.DiffTests.DiffTypes+Derived2"
                               : "With<BaseClass> <Value>k__BackingField x: Gu.State.Tests.DiffTests.DiffTypes+Derived1 y: Gu.State.Tests.DiffTests.DiffTypes+Derived2";
            var actual = result.ToString(string.Empty, " ");
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 1, null, "Empty")]
        [TestCase(1, 2, null, "WithReadonlyProperty<int> <member> x: 1 y: 2")]
        [TestCase(1, 1, ReferenceHandling.Throw, "Empty")]
        [TestCase(1, 1, ReferenceHandling.Structural, "Empty")]
        [TestCase(1, 1, ReferenceHandling.References, "Empty")]
        public void WithReadonlyIntHappyPath(int xv, int yv, ReferenceHandling? referenceHandling, string expected)
        {
            expected = expected?.Replace(
                "<member>",
                this is FieldValues.Classes
                    ? "<Value>k__BackingField"
                    : "Value");
            var x = new WithReadonlyProperty<int>(xv);
            var y = new WithReadonlyProperty<int>(yv);
            if (referenceHandling == null)
            {
                var result = this.DiffMethod(x, y);
                Assert.AreEqual(expected, result.ToString(string.Empty, " "));
            }
            else
            {
                var result = this.DiffMethod(x, y, referenceHandling.Value);
                Assert.AreEqual(expected, result.ToString(string.Empty, " "));
            }
        }

        [TestCase("a", "a", "Empty")]
        [TestCase("a", "b", "WithReadonlyProperty<ComplexType> <member1> <member2> x: a y: b")]
        public void WithReadonlyComplex(string xv, string yv, string expected)
        {
            expected = expected?.Replace(
                "<member1>",
                this is FieldValues.Classes
                    ? "<Value>k__BackingField"
                    : "Value")
                                .Replace(
                                    "<member2>",
                                    this is FieldValues.Classes
                                        ? "name"
                                        : "Name");
            var x = new WithReadonlyProperty<ComplexType>(new ComplexType(xv, 1));
            var y = new WithReadonlyProperty<ComplexType>(new ComplexType(yv, 1));
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void WithListOfIntsToEmpty()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int>();
            var expected = this is FieldValues.Classes
                   ? "WithListProperty<int> <Items>k__BackingField [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item"
                   : "WithListProperty<int> Items [0] x: 1 y: missing item [1] x: 2 y: missing item [2] x: 3 y: missing item";

            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithListOfIntsNullToNull(ReferenceHandling referenceHandling)
        {
            var x = new WithListProperty<int> { Items = null };
            var y = new WithListProperty<int> { Items = null };

            var result = this.DiffMethod(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void WithListOfIntsEmptyToEmpty()
        {
            var x = new WithListProperty<int> { Items = new List<int>() };
            var y = new WithListProperty<int> { Items = new List<int>() };

            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result.IsEmpty);
            var expected = this is FieldValues.Classes
                               ? "WithListProperty<int> <Items>k__BackingField x: System.Collections.Generic.List`1[System.Int32] y: System.Collections.Generic.List`1[System.Int32]"
                               : "WithListProperty<int> Items x: System.Collections.Generic.List`1[System.Int32] y: System.Collections.Generic.List`1[System.Int32]";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void WithListOfIntsToNull()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int> { Items = null };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = this is FieldValues.Classes
                               ? "WithListProperty<int> <Items>k__BackingField x: System.Collections.Generic.List`1[System.Int32] y: null"
                               : "WithListProperty<int> Items x: System.Collections.Generic.List`1[System.Int32] y: null";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithArrayWhenTargetArrayIsNull(ReferenceHandling referenceHandling)
        {
            var x = new WithArrayProperty("a", 1, new[] { 1, 2 });
            var y = new WithArrayProperty("a", 1, null);

            var result = this.DiffMethod(x, y, referenceHandling);
            var expected = this is FieldValues.Classes
                               ? "WithArrayProperty <Array>k__BackingField x: System.Int32[] y: null"
                               : "WithArrayProperty Array x: System.Int32[] y: null";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void WithListOfIntsPropertyToLonger()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int> { Items = { 1, 2, 3, 4 } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            var expected = this is FieldValues.Classes
                               ? "WithListProperty<int> <Items>k__BackingField [3] x: missing item y: 4"
                               : "WithListProperty<int> Items [3] x: missing item y: 4";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyStructural()
        {
            var x = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var y = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var result = this.DiffMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void WithListOfComplexPropertyToLonger()
        {
            var source = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var target = new WithListProperty<ComplexType>
            {
                Items =
                                     {
                                         new ComplexType("a", 1),
                                         new ComplexType("a", 1)
                                     }
            };
            var result = this.DiffMethod(source, target, ReferenceHandling.Structural);
            var expected = this is FieldValues.Classes
                               ? "WithListProperty<ComplexType> <Items>k__BackingField [1] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType"
                               : "WithListProperty<ComplexType> Items [1] x: missing item y: Gu.State.Tests.DiffTests.DiffTypes+ComplexType";
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase(1, 1, null, "Empty")]
        [TestCase(1, 2, null, "WithSimpleProperties <member> x: 1 y: 2")]
        [TestCase(1, 1, ReferenceHandling.Throw, "Empty")]
        [TestCase(1, 1, ReferenceHandling.Structural, "Empty")]
        [TestCase(1, 1, ReferenceHandling.References, "Empty")]
        public void IgnoresMember(int xv, int yv, ReferenceHandling? referenceHandling, string expected)
        {
            expected = expected?.Replace(
                "<member>",
                this is FieldValues.Classes
                    ? "intValue"
                    : "IntValue");
            var x = new WithSimpleProperties(xv, null, "3", StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(yv, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var excluded = this is FieldValues.Classes
                               ? "nullableIntValue"
                               : nameof(WithSimpleProperties.NullableIntValue);
            if (referenceHandling == null)
            {
                var result = this.DiffMethod(x, y, excludedMembers: excluded);
                Assert.AreEqual(expected, result.ToString(string.Empty, " "));
            }
            else
            {
                var result = this.DiffMethod(x, y, referenceHandling.Value, excluded);
                Assert.AreEqual(expected, result.ToString(string.Empty, " "));
            }
        }

        [TestCase("a", "Empty")]
        [TestCase("b", "WithComplexProperty <member> x: b y: a")]
        public void IgnoresType(string xv, string expected)
        {
            expected = expected?.Replace(
                "<member>",
                this is FieldValues.Classes
                    ? "name"
                    : "Name");
            var x = new WithComplexProperty(xv, 1, new ComplexType("b", 2));
            var y = new WithComplexProperty("a", 1, new ComplexType("c", 2));
            var result = this.DiffMethod(
                x,
                y,
                ReferenceHandling.Structural,
                ignoredType: typeof(ComplexType));
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }
    }
}