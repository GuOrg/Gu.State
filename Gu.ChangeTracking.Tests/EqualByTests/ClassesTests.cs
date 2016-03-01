namespace Gu.ChangeTracking.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    public abstract class ClassesTests
    {
        public abstract bool EqualMethod<T>(T x, T y) where T : class;

        public abstract bool EqualMethod<T>(T x, T y, ReferenceHandling referenceHandling) where T : class;

        public abstract bool EqualMethod<T>(T x, T y, params string[] excluded) where T : class;

        public abstract bool EqualMethod<T>(T x, T y, string excluded, ReferenceHandling referenceHandling);

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
            var x = new EqualByTypes.WithSimpleProperties(1, 2, xn, StringSplitOptions.RemoveEmptyEntries);
            var y = new EqualByTypes.WithSimpleProperties(1, 2, yn, StringSplitOptions.RemoveEmptyEntries);
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
            var x = new EqualByTypes.WithComplexProperty("a", 1)
            {
                ComplexType = new EqualByTypes.ComplexType { Name = xn, Value = 2 }
            };

            var y = new EqualByTypes.WithComplexProperty("a", 1)
            {
                ComplexType = new EqualByTypes.ComplexType { Name = yn, Value = 2 }
            };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WithComplexStructuralWhenNull()
        {
            var x = new EqualByTypes.WithComplexProperty { Name = "a", Value = 1 };
            var y = new EqualByTypes.WithComplexProperty { Name = "a", Value = 1 };
            this.EqualMethod(x, y, ReferenceHandling.Structural);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithComplexStructuralWhenXIsNull()
        {
            var x = new EqualByTypes.WithComplexProperty { Name = "a", Value = 1, ComplexType = new EqualByTypes.ComplexType("b", 1) };
            var y = new EqualByTypes.WithComplexProperty { Name = "a", Value = 1 };
            this.EqualMethod(x, y, ReferenceHandling.Structural);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithComplexStructuralWhenYIsNull()
        {
            var x = new EqualByTypes.WithComplexProperty { Name = "a", Value = 1 };
            var y = new EqualByTypes.WithComplexProperty { Name = "a", Value = 1, ComplexType = new EqualByTypes.ComplexType("b", 1) };
            this.EqualMethod(x, y, ReferenceHandling.Structural);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithComplexReferenceWhenSame()
        {
            var x = new EqualByTypes.WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new EqualByTypes.ComplexType { Name = "b", Value = 2 }
            };
            var y = new EqualByTypes.WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = x.ComplexType
            };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithComplexReferenceWhenNotSame()
        {
            var x = new EqualByTypes.WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new EqualByTypes.ComplexType { Name = "b", Value = 2 }
            };
            var y = new EqualByTypes.WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new EqualByTypes.ComplexType { Name = "b", Value = 2 }
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
        public void WithReadonlyHappyPath(int xv, int yv, ReferenceHandling? referenceHandling, bool expected)
        {
            var x = new EqualByTypes.WithReadonlyProperty<int>(xv);
            var y = new EqualByTypes.WithReadonlyProperty<int>(yv);
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
            var x = new EqualByTypes.WithReadonlyProperty<EqualByTypes.ComplexType>(new EqualByTypes.ComplexType(xv, 1));
            var y = new EqualByTypes.WithReadonlyProperty<EqualByTypes.ComplexType>(new EqualByTypes.ComplexType(yv, 1));
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void WithListOfIntsToEmpty()
        {
            var x = new EqualByTypes.WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new EqualByTypes.WithListProperty<int>();
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsNullToNull()
        {
            var x = new EqualByTypes.WithListProperty<int> { Items = null };
            var y = new EqualByTypes.WithListProperty<int> { Items = null };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithListOfIntsEmptyToEmpty()
        {
            var x = new EqualByTypes.WithListProperty<int> { Items = new List<int>() };
            var y = new EqualByTypes.WithListProperty<int> { Items = new List<int>() };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);

            result = this.EqualMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsToNull()
        {
            var x = new EqualByTypes.WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new EqualByTypes.WithListProperty<int> { Items = null };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithArrayWhenTargetArrayIsNull(ReferenceHandling referenceHandling)
        {
            var x = new EqualByTypes.WithArrayProperty("a", 1, new[] { 1, 2 });
            var y = new EqualByTypes.WithArrayProperty("a", 1, null);

            var result = this.EqualMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsPropertyToLonger()
        {
            var x = new EqualByTypes.WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new EqualByTypes.WithListProperty<int> { Items = { 1, 2, 3, 4 } };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyStructural()
        {
            var x = new EqualByTypes.WithListProperty<EqualByTypes.ComplexType> { Items = { new EqualByTypes.ComplexType("a", 1) } };
            var y = new EqualByTypes.WithListProperty<EqualByTypes.ComplexType> { Items = { new EqualByTypes.ComplexType("a", 1) } };
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result);
        }

        [Test]
        public void WithListOfComplexPropertyToLonger()
        {
            var source = new EqualByTypes.WithListProperty<EqualByTypes.ComplexType> { Items = { new EqualByTypes.ComplexType("a", 1) } };
            var target = new EqualByTypes.WithListProperty<EqualByTypes.ComplexType> { Items = { new EqualByTypes.ComplexType("a", 1), new EqualByTypes.ComplexType("a", 1) } };
            var result = this.EqualMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [TestCase(1, 1, null, true)]
        [TestCase(1, 2, null, false)]
        [TestCase(1, 1, ReferenceHandling.Throw, true)]
        [TestCase(1, 1, ReferenceHandling.Structural, true)]
        [TestCase(1, 1, ReferenceHandling.References, true)]
        public void Ignores(int xv, int yv, ReferenceHandling? referenceHandling, bool expected)
        {
            var x = new EqualByTypes.WithSimpleProperties(xv, null, "3", StringSplitOptions.RemoveEmptyEntries);
            var y = new EqualByTypes.WithSimpleProperties(yv, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var excluded = this.GetType() == typeof(FieldValues.Classes)
                        ? "nullableIntValue"
                        : nameof(EqualByTypes.WithSimpleProperties.NullableIntValue);
            if (referenceHandling == null)
            {
                var result = this.EqualMethod(x, y, excluded);
                Assert.AreEqual(expected, result);
            }
            else
            {
                var result = this.EqualMethod(x, y, excluded, referenceHandling.Value);
                Assert.AreEqual(expected, result);
            }
        }

        [TestCase("p", "c", true)]
        [TestCase("", "c", false)]
        [TestCase("p", "", false)]
        public void ParentChild(string p, string c, bool expected)
        {
            var x = new EqualByTypes.Parent("p", new EqualByTypes.Child("c"));
            var y = new EqualByTypes.Parent(p, new EqualByTypes.Child(c));
            var result = this.EqualMethod(x, y, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(y, x, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual(expected, result);
        }
    }
}