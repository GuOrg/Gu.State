// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.CopyTests
{
    using System;

    using NUnit.Framework;

    using static CopyTypes;

    public abstract class ClassesTests
    {
        public abstract void CopyMethod<T>(
            T source,
            T target,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excluded = null,
            Type ignoredType = null,
            Type immutableType = null)
            where T : class;

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithSimpleHappyPath(ReferenceHandling referenceHandling)
        {
            var source = new WithSimpleProperties(1, 2, "3", StringSplitOptions.RemoveEmptyEntries);
            var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
            this.CopyMethod(source, target, referenceHandling);
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
        public void WithExplitImmutable()
        {
            var source = new WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new ComplexType { Name = "b", Value = 2 },
            };
            var target = new WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.Structural, immutableType: typeof(ComplexType));
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.AreSame(source.ComplexType, target.ComplexType);
        }

        [Test]
        public void WithComplexStructuralHappyPath()
        {
            var source = new WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new ComplexType { Name = "b", Value = 2 },
            };
            var target = new WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.AreEqual(source.ComplexType.Name, target.ComplexType.Name);
            Assert.AreEqual(source.ComplexType.Value, target.ComplexType.Value);
        }

        [Test]
        public void WithComplexStructuralHappyPathWhenTargetMemberIsNull()
        {
            var source = new WithComplexProperty { Name = "a", Value = 1 };
            var target = new WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.IsNull(source.ComplexType);
            Assert.IsNull(target.ComplexType);
        }

        [Test]
        public void WithComplexReferenceHappyPath()
        {
            var source = new WithComplexProperty
            {
                Name = "a",
                Value = 1,
                ComplexType = new ComplexType { Name = "b", Value = 2 },
            };
            var target = new WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.AreSame(source.ComplexType, target.ComplexType);
        }

        [Test]
        public void WithComplexReferenceHappyPathSourceMemberIsNull()
        {
            var source = new WithComplexProperty { Name = "a", Value = 1 };
            var target = new WithComplexProperty();
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.IsNull(source.ComplexType);
            Assert.IsNull(target.ComplexType);
        }

        [Test]
        public void WithComplexReferenceHappyPathTargetMemberIsNull()
        {
            var source = new WithComplexProperty { Name = "a", Value = 1 };
            var target = new WithComplexProperty { ComplexType = new ComplexType("b", 1) };
            this.CopyMethod(source, target, ReferenceHandling.References);
            Assert.AreEqual(source.Name, target.Name);
            Assert.AreEqual(source.Value, target.Value);
            Assert.IsNull(source.ComplexType);
            Assert.IsNull(target.ComplexType);
        }

        [Test]
        public void WithInheritanceWhenSameType()
        {
            var source = new With<BaseClass> { Value = new Derived1 { BaseValue = 1, Derived1Value = 2 } };
            var target = new With<BaseClass>();
            Copy.PropertyValues(source, target, ReferenceHandling.Structural);
            Assert.IsInstanceOf<Derived1>(source.Value);
            Assert.IsInstanceOf<Derived1>(target.Value);
            Assert.AreEqual(1, source.Value.BaseValue);
            Assert.AreEqual(1, target.Value.BaseValue);

            Assert.AreEqual(2, ((Derived1)source.Value).Derived1Value);
            Assert.AreEqual(2, ((Derived1)target.Value).Derived1Value);
        }

        [Test]
        public void WithInheritanceWhenDifferentTypes()
        {
            var source = new With<BaseClass> { Value = new Derived1 { BaseValue = 1, Derived1Value = 2 } };
            var target = new With<BaseClass> { Value = new Derived2() };
            Copy.PropertyValues(source, target, ReferenceHandling.Structural);
            Assert.IsInstanceOf<Derived1>(source.Value);
            Assert.IsInstanceOf<Derived1>(target.Value);
            Assert.AreEqual(1, source.Value.BaseValue);
            Assert.AreEqual(1, target.Value.BaseValue);

            Assert.AreEqual(2, ((Derived1)source.Value).Derived1Value);
            // ReSharper disable once PossibleInvalidCastException
            Assert.AreEqual(2, ((Derived1)target.Value).Derived1Value);
        }

        [Test]
        public void WithReadonlyHappyPath()
        {
            var x = new WithReadonlyProperty<int>(1);
            var y = new WithReadonlyProperty<int>(1);
            this.CopyMethod(x, y);
            Assert.AreEqual(1, x.Value);
            Assert.AreEqual(1, y.Value);
        }

        [Test]
        public void WithReadonlyComplex()
        {
            var source = new WithReadonlyProperty<ComplexType>(new ComplexType("a", 1));
            var target = new WithReadonlyProperty<ComplexType>(new ComplexType("b", 2));
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
            var source = new With<Immutable>(new Immutable(1));
            var target = new With<Immutable>(new Immutable(2));
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
            var source = new WithListProperty<int>();
            source.Items.AddRange(new[] { 1, 2, 3 });
            var target = new WithListProperty<int>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source.Items, target.Items);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithArrayWhenTargetArrayIsNullStructural(ReferenceHandling referenceHandling)
        {
            var source = new WithArrayProperty("a", 1, new[] { 1, 2 });
            var target = new WithArrayProperty("a", 1, null);
            this.CopyMethod(source, target, referenceHandling);
            Assert.AreEqual("a", source.Name);
            Assert.AreEqual("a", target.Name);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            if (referenceHandling == ReferenceHandling.Structural)
            {
                Assert.AreNotSame(source.Array, target.Array);
            }
            else
            {
                Assert.AreSame(source.Array, target.Array);
            }

            CollectionAssert.AreEqual(new[] { 1, 2 }, source.Array);
            CollectionAssert.AreEqual(new[] { 1, 2 }, target.Array);
        }

        [Test]
        public void WithArrayWhenTargetArrayIsNullReference()
        {
            var source = new WithArrayProperty("a", 1, new[] { 1, 2 });
            var target = new WithArrayProperty("a", 1, null);
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
            var source = new WithListProperty<int>();
            source.Items.AddRange(new[] { 1, 2, 3 });
            var target = new WithListProperty<int>();
            target.Items.AddRange(new[] { 1, 2, 3, 4 });
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            CollectionAssert.AreEqual(source.Items, target.Items);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyStructural()
        {
            var source = new WithListProperty<ComplexType>();
            source.Items.Add(new ComplexType("a", 1));
            var target = new WithListProperty<ComplexType>();
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source.Items, ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target.Items, ComplexType.Comparer);
            Assert.AreNotSame(source.Items[0], target.Items[0]);
        }

        [Test]
        public void WithListOfComplexPropertyToEmptyReference()
        {
            var source = new WithListProperty<ComplexType>();
            source.Items.Add(new ComplexType("a", 1));
            var target = new WithListProperty<ComplexType>();
            this.CopyMethod(source, target, ReferenceHandling.References);
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
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source.Items, ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target.Items, ComplexType.Comparer);
            Assert.AreSame(item, target.Items[0]);
        }

        [Test]
        public void Ignores()
        {
            var source = new WithSimpleProperties { IntValue = 1, NullableIntValue = 2, StringValue = "3", EnumValue = StringSplitOptions.RemoveEmptyEntries };
            var target = new WithSimpleProperties { IntValue = 2, NullableIntValue = 4, StringValue = "4", EnumValue = StringSplitOptions.None };
            var excluded = this is FieldValues.Classes
                        ? "nullableIntValue"
                        : nameof(WithSimpleProperties.NullableIntValue);
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