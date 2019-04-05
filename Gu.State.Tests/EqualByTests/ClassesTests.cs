// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests
{
    using System;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class ClassesTests
    {
        public abstract bool EqualBy<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null)
            where T : class;

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithComplexReferenceWhenSame(ReferenceHandling referenceHandling)
        {
            var x = new WithComplexProperty { Name = "a", Value = 1, ComplexType = new ComplexType { Name = "b", Value = 2 } };
            var y = new WithComplexProperty { Name = "a", Value = 1, ComplexType = x.ComplexType };
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(true, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithArrayWhenTargetArrayIsNull(ReferenceHandling referenceHandling)
        {
            var x = new WithArrayProperty("a", 1, new[] { 1, 2 });
            var y = new WithArrayProperty("a", 1, null);

            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfIntsPropertyToLonger()
        {
            var x = new WithListProperty<int> { Items = { 1, 2, 3 } };
            var y = new WithListProperty<int> { Items = { 1, 2, 3, 4 } };
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyStructural()
        {
            var x = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var y = new WithListProperty<ComplexType> { Items = { new ComplexType("a", 1) } };
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
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
                    new ComplexType("a", 1),
                },
            };
            var result = this.EqualBy(source, target, ReferenceHandling.Structural);
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
                var result = this.EqualBy(x, y, excludedMembers: excluded);
                Assert.AreEqual(expected, result);
            }
            else
            {
                var result = this.EqualBy(x, y, referenceHandling.Value, excluded);
                Assert.AreEqual(expected, result);
            }
        }

        [TestCase("a", true)]
        [TestCase("b", false)]
        public void IgnoresType(string xv, bool expected)
        {
            var x = new WithComplexProperty(xv, 1, new ComplexType("b", 2));
            var y = new WithComplexProperty("a", 1, new ComplexType("c", 2));
            var result = this.EqualBy(x, y, ReferenceHandling.Structural, ignoredType: typeof(ComplexType));
            Assert.AreEqual(expected, result);
        }
    }
}