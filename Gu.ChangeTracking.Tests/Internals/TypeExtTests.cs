namespace Gu.ChangeTracking.Tests.Internals
{
    using System;
    using Gu.ChangeTracking.Tests.Internals.Stubs;
    using NUnit.Framework;

    public class TypeExtTests
    {
        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(TimeSpan), true)]
        [TestCase(typeof(TimeSpan?), true)]
        [TestCase(typeof(StringSplitOptions), true)]
        [TestCase(typeof(StringSplitOptions?), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<int>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<int?>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertyStruct<WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertyStruct<WithGetReadOnlyPropertyStruct<int>>), true)]
        [TestCase(typeof(WithReadonlyFieldSealed<int>), true)]
        [TestCase(typeof(WithReadonlyFieldSealed<int?>), true)]
        [TestCase(typeof(WithSelfFieldSealed), true)]
        [TestCase(typeof(WithSelfPropSealed), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithReadonlyField<int>>), false)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(WithReadonlyField<WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(object), false)]
        [TestCase(typeof(int[]), false)]
        [TestCase(typeof(WithGetPrivateSet), false)]
        [TestCase(typeof(WithGetPublicSet), false)]
        [TestCase(typeof(WithMutableField), false)]
        [TestCase(typeof(WithSelfField), false)]
        [TestCase(typeof(WithSelfProp), false)]
        [TestCase(typeof(WithImmutableSubclassingMutable), false)]
        [TestCase(typeof(WithImmutableImplementingMutableInterfaceExplicit), false)]
        public void IsImmutable(Type type, bool expected)
        {
            var isImmutable = type.IsImmutable();
            Assert.AreEqual(expected, isImmutable);
        }
    }
}
