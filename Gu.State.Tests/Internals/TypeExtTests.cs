namespace Gu.State.Tests.Internals
{
    using System;

    using NUnit.Framework;

    public class TypeExtTests
    {
        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(TimeSpan), true)]
        [TestCase(typeof(TimeSpan?), true)]
        [TestCase(typeof(StringSplitOptions), true)]
        [TestCase(typeof(StringSplitOptions?), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<int>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<int?>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<TypeExtTypes.WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertyStruct<TypeExtTypes.WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertyStruct<TypeExtTypes.WithGetReadOnlyPropertyStruct<int>>), true)]
        [TestCase(typeof(TypeExtTypes.WithReadonlyFieldSealed<int>), true)]
        [TestCase(typeof(TypeExtTypes.WithReadonlyFieldSealed<int?>), true)]
        [TestCase(typeof(TypeExtTypes.WithSelfFieldSealed), true)]
        [TestCase(typeof(TypeExtTypes.WithSelfPropSealed), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<TypeExtTypes.WithReadonlyField<int>>), false)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<TypeExtTypes.WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(TypeExtTypes.WithReadonlyField<TypeExtTypes.WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(object), false)]
        [TestCase(typeof(int[]), false)]
        [TestCase(typeof(TypeExtTypes.WithGetPrivateSet), false)]
        [TestCase(typeof(TypeExtTypes.WithGetPublicSet), false)]
        [TestCase(typeof(TypeExtTypes.WithMutableField), false)]
        [TestCase(typeof(TypeExtTypes.WithSelfField), false)]
        [TestCase(typeof(TypeExtTypes.WithSelfProp), false)]
        [TestCase(typeof(TypeExtTypes.WithImmutableSubclassingMutable), false)]
        [TestCase(typeof(TypeExtTypes.WithImmutableImplementingMutableInterfaceExplicit), false)]
        public void IsImmutable(Type type, bool expected)
        {
            var isImmutable = type.IsImmutable();
            Assert.AreEqual(expected, isImmutable);
        }

        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(TimeSpan), true)]
        [TestCase(typeof(TimeSpan?), true)]
        [TestCase(typeof(StringSplitOptions), true)]
        [TestCase(typeof(StringSplitOptions?), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<int>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<int?>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<TypeExtTypes.WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertyStruct<TypeExtTypes.WithGetReadOnlyPropertySealed<int>>), false)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertyStruct<TypeExtTypes.WithGetReadOnlyPropertyStruct<int>>), false)]
        [TestCase(typeof(TypeExtTypes.WithReadonlyFieldSealed<int>), false)]
        [TestCase(typeof(TypeExtTypes.WithReadonlyFieldSealed<int?>), false)]
        [TestCase(typeof(TypeExtTypes.WithSelfFieldSealed), false)]
        [TestCase(typeof(TypeExtTypes.WithSelfPropSealed), false)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<TypeExtTypes.WithReadonlyField<int>>), true)]
        [TestCase(typeof(TypeExtTypes.WithGetReadOnlyPropertySealed<TypeExtTypes.WithGetReadOnlyProperty<int>>), true)]
        [TestCase(typeof(TypeExtTypes.WithReadonlyField<TypeExtTypes.WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(object), false)]
        [TestCase(typeof(int[]), false)]
        [TestCase(typeof(TypeExtTypes.WithGetPrivateSet), false)]
        [TestCase(typeof(TypeExtTypes.WithGetPublicSet), false)]
        [TestCase(typeof(TypeExtTypes.WithMutableField), false)]
        [TestCase(typeof(TypeExtTypes.WithSelfField), false)]
        [TestCase(typeof(TypeExtTypes.WithSelfProp), false)]
        [TestCase(typeof(TypeExtTypes.WithImmutableSubclassingMutable), false)]
        [TestCase(typeof(TypeExtTypes.WithImmutableImplementingMutableInterfaceExplicit), false)]
        public void IsEquatable(Type type, bool expected)
        {
            var isImmutable = type.IsEquatable();
            Assert.AreEqual(expected, isImmutable);
        }
    }
}
