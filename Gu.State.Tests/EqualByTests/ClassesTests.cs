// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class ClassesTests
    {
        public abstract bool EqualMethod<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type excludedType = null) where T : class;

        public static IReadOnlyList<EqualByTestsShared.EqualsData> EqualsSource => EqualByTestsShared.EqualsSource;

        [TestCaseSource(nameof(EqualsSource))]
        public void PropertyValuesHappyPath(EqualByTestsShared.EqualsData data)
        {
            Assert.AreEqual(data.Equals, this.EqualMethod(data.Source, data.Target));
        }

        [TestCase("b", "b", true)]
        [TestCase("b", "c", false)]
        public void WithSimpleHappyPath(string xn, string yn, bool expected)
        {
            var x = new WithSimpleProperties(1, 2, xn, StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(1, 2, yn, StringSplitOptions.RemoveEmptyEntries);
            var result = this.EqualMethod(x, y);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(x, y, ReferenceHandling.Throw);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(expected, result);
        }

        [TestCase("b", "b", true)]
        [TestCase("b", "c", false)]
        public void WithComplexStructural(string xn, string yn, bool expected)
        {
            var x = new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = xn, Value = 2 } };

            var y = new WithComplexProperty("a", 1) { ComplexType = new ComplexType { Name = yn, Value = 2 } };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WithComplexStructuralWhenNull()
        {
            var x = new WithComplexProperty { Name = "a", Value = 1 };
            var y = new WithComplexProperty { Name = "a", Value = 1 };
            this.EqualMethod(x, y, ReferenceHandling.Structural);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithComplexStructuralWhenXIsNull()
        {
            var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType("b", 1) };
            var y = new WithComplexProperty { Name = "a", Value = 1 };
            this.EqualMethod(x, y, ReferenceHandling.Structural);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithComplexStructuralWhenYIsNull()
        {
            var x = new WithComplexProperty { Name = "a", Value = 1 };
            var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType("b", 1) };
            this.EqualMethod(x, y, ReferenceHandling.Structural);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithComplexReferenceWhenSame()
        {
            var x = new WithComplexProperty
                        {
                            Name = "a",
                            Value = 1,
                            ComplexType = new ComplexType { Name = "b", Value = 2 }
                        };
            var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = x.ComplexType };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithComplexReferenceWhenNotSame()
        {
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
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [TestCase(1, 1, null, true)]
        [TestCase(1, 2, null, false)]
        [TestCase(1, 1, ReferenceHandling.Throw, true)]
        [TestCase(1, 1, ReferenceHandling.Structural, true)]
        [TestCase(1, 1, ReferenceHandling.References, true)]
        public void WithReadonlyIntHappyPath(int xv, int yv, ReferenceHandling? referenceHandling, bool expected)
        {
            var x = new WithReadonlyProperty<int>(xv);
            var y = new WithReadonlyProperty<int>(yv);
            if (referenceHandling == null)
            {
                var result = this.EqualMethod(x, y);
                Assert.AreEqual(expected, result);
            }
            else
            {
                var result = this.EqualMethod(x, y, referenceHandling.Value);
                Assert.AreEqual(expected, result);
            }
        }

        [TestCase("a", "a", true)]
        [TestCase("a", "b", false)]
        public void WithReadonlyComplex(string xv, string yv, bool expected)
        {
            var x = new WithReadonlyProperty<ComplexType>(new ComplexType(xv, 1));
            var y = new WithReadonlyProperty<ComplexType>(new ComplexType(yv, 1));
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WithListOfIntsToEmpty()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int>();
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsNullToNull()
        {
            var x = new WithListProperty<int> { Items = null };
            var y = new WithListProperty<int> { Items = null };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithListOfIntsEmptyToEmpty()
        {
            var x = new WithListProperty<int> { Items = new List<int>() };
            var y = new WithListProperty<int> { Items = new List<int>() };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsToNull()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int> { Items = null };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithArrayWhenTargetArrayIsNull(ReferenceHandling referenceHandling)
        {
            var x = new WithArrayProperty("a", 1, new[] { 1, 2 });
            var y = new WithArrayProperty("a", 1, null);

            var result = this.EqualMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsPropertyToLonger()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int> { Items = { 1, 2, 3, 4 } };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyStructural()
        {
            var x = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var y = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
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
            var result = this.EqualMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [TestCase(1, 1, null, true)]
        [TestCase(1, 2, null, false)]
        [TestCase(1, 1, ReferenceHandling.Throw, true)]
        [TestCase(1, 1, ReferenceHandling.Structural, true)]
        [TestCase(1, 1, ReferenceHandling.References, true)]
        public void IgnoresMember(int xv, int yv, ReferenceHandling? referenceHandling, bool expected)
        {
            var x = new WithSimpleProperties(xv, null, "3", StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(yv, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var excluded = this is FieldValues.Classes
                               ? "nullableIntValue"
                               : nameof(WithSimpleProperties.NullableIntValue);
            if (referenceHandling == null)
            {
                var result = this.EqualMethod(x, y, excludedMembers: excluded);
                Assert.AreEqual(expected, result);
            }
            else
            {
                var result = this.EqualMethod(x, y, referenceHandling.Value, excluded);
                Assert.AreEqual(expected, result);
            }
        }

        [TestCase("a", true)]
        [TestCase("b", false)]
        public void IgnoresType(string xv, bool expected)
        {
            var x = new EqualByTypes.WithComplexProperty(xv, 1, new EqualByTypes.ComplexType("b", 2));
            var y = new EqualByTypes.WithComplexProperty("a", 1, new EqualByTypes.ComplexType("c", 2));
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural, excludedType: typeof(EqualByTypes.ComplexType));
            Assert.AreEqual(expected, result);
        }
    }
}