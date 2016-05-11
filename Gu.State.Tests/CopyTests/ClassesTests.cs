// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.CopyTests
{
    using System;

    using NUnit.Framework;

    public abstract class ClassesTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excluded = null) where T : class;

        [Test]
        public void WithSimpleHappyPath()
        {
            var source = new CopyTypes.WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var target = new CopyTypes.WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
            this.CopyMethod(source, target);
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
        public void WithComplexStructuralHappyPath()
        {
            var source = new CopyTypes.WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new CopyTypes.ComplexType { Name = "b", Value = 2 }
            };
            var target = new CopyTypes.WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.AreEqual(source.ComplexType.Name, target.ComplexType.Name);
            Assert.AreEqual(source.ComplexType.Value, target.ComplexType.Value);
        }

        [Test]
        public void WithComplexStructuralHappyPathWhenTargetMemberIsNull()
        {
            var source = new CopyTypes.WithComplexProperty { Name = "a", Value = 1 };
            var target = new CopyTypes.WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.IsNull(source.ComplexType);
            Assert.IsNull(target.ComplexType);
        }

        [Test]
        public void WithComplexReferenceHappyPath()
        {
            var source = new CopyTypes.WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new CopyTypes.ComplexType { Name = "b", Value = 2 }
            };
            var target = new CopyTypes.WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.AreSame(source.ComplexType, target.ComplexType);
        }

        [Test]
        public void WithComplexReferenceHappyPathSourceMemberIsNull()
        {
            var source = new CopyTypes.WithComplexProperty { Name = "a", Value = 1 };
            var target = new CopyTypes.WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.IsNull(source.ComplexType);
            Assert.IsNull(target.ComplexType);
        }

        [Test]
        public void WithComplexReferenceHappyPathTargetMemberIsNull()
        {
            var source = new CopyTypes.WithComplexProperty { Name = "a", Value = 1 };
            var target = new CopyTypes.WithComplexProperty { ComplexType = new CopyTypes.ComplexType("b", 1) };
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.IsNull(source.ComplexType);
            Assert.IsNull(target.ComplexType);
        }

        [Test]
        public void WithReadonlyHappyPath()
        {
            var x = new CopyTypes.WithReadonlyProperty<int>(1);
            var y = new CopyTypes.WithReadonlyProperty<int>(1);
            this.CopyMethod(x, y);
            Assert.AreEqual(1, x.Value);
            Assert.AreEqual(1, y.Value);
        }

        [Test]
        public void WithReadonlyComplex()
        {
            var source = new CopyTypes.WithReadonlyProperty<CopyTypes.ComplexType>(new CopyTypes.ComplexType("a", 1));
            var target = new CopyTypes.WithReadonlyProperty<CopyTypes.ComplexType>(new CopyTypes.ComplexType("b", 2));
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("a", source.Value.Name);
            Assert.AreEqual("a", target.Value.Name);
            Assert.AreEqual(1, source.Value.Value);
            Assert.AreEqual(1, target.Value.Value);
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithImmutableStructural(ReferenceHandling? referenceHandling)
        {
            var source = new CopyTypes.WithProperty<CopyTypes.Immutable>(new CopyTypes.Immutable(1));
            var target = new CopyTypes.WithProperty<CopyTypes.Immutable>(new CopyTypes.Immutable(2));
            if (referenceHandling == null)
            {
                this.CopyMethod(source, target);
            }
            else
            {
                this.CopyMethod(source, target, referenceHandling.Value);
            }
            Assert.AreEqual(1, source.Value.Value);
            Assert.AreEqual(1, target.Value.Value);
            Assert.AreSame(source.Value, target.Value);
        }

        [Test]
        public void WithListOfIntsToEmpty()
        {
            var source = new CopyTypes.WithListProperty<int>();
            source.Items.AddRange(new[] { 1, 2, 3 });
            var target = new CopyTypes.WithListProperty<int>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source.Items, target.Items);
        }

        [Test]
        public void WithArrayWhenTargetArrayIsNullStructural()
        {
            var source = new CopyTypes.WithArrayProperty("a", 1, new[] { 1, 2 });
            var target = new CopyTypes.WithArrayProperty("a", 1, null);
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("a", source.Name);
            Assert.AreEqual("a", target.Name);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreNotSame(source.Array, target.Array);
            CollectionAssert.AreEqual(new[] { 1, 2 }, source.Array);
            CollectionAssert.AreEqual(new[] { 1, 2 }, target.Array);
        }

        [Test]
        public void WithArrayWhenTargetArrayIsNullReference()
        {
            var source = new CopyTypes.WithArrayProperty("a", 1, new[] { 1, 2 });
            var target = new CopyTypes.WithArrayProperty("a", 1, null);
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual("a", source.Name);
            Assert.AreEqual("a", target.Name);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreSame(source.Array, target.Array);
            CollectionAssert.AreEqual(new[] { 1, 2 }, source.Array);
        }

        [Test]
        public void WithListOfIntsPropertyToLonger()
        {
            var source = new CopyTypes.WithListProperty<int>();
            source.Items.AddRange(new[] { 1, 2, 3 });
            var target = new CopyTypes.WithListProperty<int>();
            target.Items.AddRange(new[] { 1, 2, 3, 4 });
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source.Items, target.Items);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyStructural()
        {
            var source = new CopyTypes.WithListProperty<CopyTypes.ComplexType>();
            source.Items.Add(new CopyTypes.ComplexType("a", 1));
            var target = new CopyTypes.WithListProperty<CopyTypes.ComplexType>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source.Items, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target.Items, CopyTypes.ComplexType.Comparer);
            Assert.AreNotSame(source.Items[0], target.Items[0]);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyReference()
        {
            Assert.Inconclusive("Not sure how to handle this");
            var source = new CopyTypes.WithListProperty<CopyTypes.ComplexType>();
            source.Items.Add(new CopyTypes.ComplexType("a", 1));
            var target = new CopyTypes.WithListProperty<CopyTypes.ComplexType>();
            this.CopyMethod(source, target, ReferenceHandling.References);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source.Items, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target.Items, CopyTypes.ComplexType.Comparer);
            Assert.AreSame(source.Items[0], target.Items[0]);
        }

        [Test]
        public void WithListOfComplexPropertyToLonger()
        {
            var source = new CopyTypes.WithListProperty<CopyTypes.ComplexType>();
            source.Items.Add(new CopyTypes.ComplexType("a", 1));
            var target = new CopyTypes.WithListProperty<CopyTypes.ComplexType>();
            target.Items.AddRange(new[] { new CopyTypes.ComplexType("b", 2), new CopyTypes.ComplexType("c", 3) });
            var item = target.Items[0];
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source.Items, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target.Items, CopyTypes.ComplexType.Comparer);
            Assert.AreSame(item, target.Items[0]);
        }

        [Test]
        public void Ignores()
        {
            var source = new CopyTypes.WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var target = new CopyTypes.WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
            var excluded = this.GetType() == typeof(FieldValues.Classes)
                        ? "nullableIntValue"
                        : nameof(CopyTypes.WithSimpleProperties.NullableIntValue);
            this.CopyMethod(source, target, excluded: excluded);
            Assert.AreEqual(1, source.IntValue);
            Assert.AreEqual(1, target.IntValue);
            Assert.AreEqual(2, source.NullableIntValue);
            Assert.AreEqual(4, target.NullableIntValue);
            Assert.AreEqual("3", source.StringValue);
            Assert.AreEqual("3", target.StringValue);
            Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, source.EnumValue);
            Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, target.EnumValue);
        }
    }
}