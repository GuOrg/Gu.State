namespace Gu.ChangeTracking.Tests.Internals
{
    using System;

    using Gu.ChangeTracking.Tests.Internals.Stubs;

    using NUnit.Framework;

    public class TypeExtTests
    {
        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(WithGetReadOnlyProperty<int>), true)]
        [TestCase(typeof(WithGetReadOnlyProperty<int?>), true)]
        [TestCase(typeof(WithGetReadOnlyProperty<WithReadonlyField<int>>), true)]
        [TestCase(typeof(WithReadonlyField<int>), true)]
        [TestCase(typeof(WithReadonlyField<int?>), true)]
        [TestCase(typeof(WithReadonlyField<WithGetReadOnlyProperty<int>>), true)]
        [TestCase(typeof(WithSelfField), true)]
        [TestCase(typeof(WithSelfProp), true)]
        [TestCase(typeof(int[]), false)]
        [TestCase(typeof(WithGetPrivateSet), false)]
        [TestCase(typeof(WithGetPublicSet), false)]
        [TestCase(typeof(WithMutableField), false)]
        public void IsImmutable(Type type, bool expected)
        {
            var isImmutable = type.IsImmutable();
            Assert.AreEqual(expected, isImmutable);
        }
    }
}
