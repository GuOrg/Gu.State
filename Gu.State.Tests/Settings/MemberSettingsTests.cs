namespace Gu.State.Tests.Settings
{
    using System;
    using System.Collections.Generic;

    using Gu.Units;

    using NUnit.Framework;

    using static SettingsTypes;

    public class MemberSettingsTests
    {
        //[TestCase(typeof(IEnumerable<int>), true)]
        [TestCase(typeof(string), true)]
        [TestCase(typeof(bool), true)]
        [TestCase(typeof(bool?), true)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(decimal), true)]
        [TestCase(typeof(decimal?), true)]
        [TestCase(typeof(TimeSpan), true)]
        [TestCase(typeof(TimeSpan?), true)]
        [TestCase(typeof(StringSplitOptions), true)]
        [TestCase(typeof(StringSplitOptions?), true)]
        [TestCase(typeof(Length), true)]
        [TestCase(typeof(Delegate), true)]
        [TestCase(typeof(Func<int>), true)]
        [TestCase(typeof(Action<int>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<int>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<int?>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertyStruct<WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertyStruct<WithGetReadOnlyPropertyStruct<int>>), true)]
        [TestCase(typeof(WithReadonlyFieldSealed<int>), true)]
        [TestCase(typeof(WithReadonlyFieldSealed<int?>), true)]
        [TestCase(typeof(WithSelfFieldSealed), true)]
        [TestCase(typeof(KeyValuePair<int, string>), true)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableList<int>), true)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableArray<int>), true)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableHashSet<int>), true)]
        [TestCase(typeof(WithSelfPropSealed), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithReadonlyField<int>>), false)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(WithReadonlyField<WithGetReadOnlyProperty<int>>), false)]
        [TestCase(typeof(object), false)]
        [TestCase(typeof(int[]), false)]
        [TestCase(typeof(List<int>), false)]
        [TestCase(typeof(WithGetPrivateSet), false)]
        [TestCase(typeof(WithGetPublicSet), false)]
        [TestCase(typeof(WithMutableField), false)]
        [TestCase(typeof(WithSelfField), false)]
        [TestCase(typeof(WithSelfProp), false)]
        [TestCase(typeof(WithImmutableSubclassingMutable), false)]
        [TestCase(typeof(WithImmutableImplementingMutableInterfaceExplicit), false)]
        [TestCase(typeof(KeyValuePair<int, ComplexType>), false)]
        [TestCase(typeof(KeyValuePair<ComplexType, string>), false)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableList<ComplexType>), false)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableArray<ComplexType>), false)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableHashSet<ComplexType>), false)]
        public void IsImmutable(Type type, bool expected)
        {
            var settings = PropertiesSettings.GetOrCreate();
            var isImmutable = settings.IsImmutable(type);
            Assert.AreEqual(expected, isImmutable);
        }

        [TestCase(typeof(string), true)]
        [TestCase(typeof(bool), true)]
        [TestCase(typeof(bool?), true)]
        [TestCase(typeof(int), true)]
        [TestCase(typeof(int?), true)]
        [TestCase(typeof(decimal), true)]
        [TestCase(typeof(decimal?), true)]
        [TestCase(typeof(TimeSpan), true)]
        [TestCase(typeof(TimeSpan?), true)]
        [TestCase(typeof(StringSplitOptions), true)]
        [TestCase(typeof(StringSplitOptions?), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<int>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<int?>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithGetReadOnlyPropertySealed<int>>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertyStruct<WithGetReadOnlyPropertySealed<int>>), false)]
        [TestCase(typeof(WithGetReadOnlyPropertyStruct<WithGetReadOnlyPropertyStruct<int>>), false)]
        [TestCase(typeof(WithReadonlyFieldSealed<int>), false)]
        [TestCase(typeof(WithReadonlyFieldSealed<int?>), false)]
        [TestCase(typeof(WithSelfFieldSealed), false)]
        [TestCase(typeof(WithSelfPropSealed), false)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithReadonlyField<int>>), true)]
        [TestCase(typeof(WithGetReadOnlyPropertySealed<WithGetReadOnlyProperty<int>>), true)]
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
        [TestCase(typeof(System.Collections.Immutable.ImmutableList<int>), false)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableArray<int>), false)]
        [TestCase(typeof(System.Collections.Immutable.ImmutableHashSet<int>), false)]
        public void IsEquatable(Type type, bool expected)
        {
            var settings = PropertiesSettings.GetOrCreate();
            var isEquatable = settings.IsEquatable(type);
            Assert.AreEqual(expected, isEquatable);
        }
    }
}
