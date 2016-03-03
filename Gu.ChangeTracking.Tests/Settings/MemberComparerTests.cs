namespace Gu.ChangeTracking.Tests.Settings
{
    using System.Reflection;

    using Gu.ChangeTracking.Internals;

    using NUnit.Framework;

    public class MemberComparerTests
    {
        [Test]
        public void CompareSame()
        {
            var nameProperty = typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name));

            var comparer = MemberInfoComparer<PropertyInfo>.Default;
            Assert.AreEqual(true, comparer.Equals(nameProperty, nameProperty));
            Assert.AreEqual(comparer.GetHashCode(nameProperty), comparer.GetHashCode(nameProperty));
        }

        [Test]
        public void CompareDerived()
        {
            var complexNameProperty = typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name));
            var derivedNameProperty = typeof(SettingsTypes.Derived).GetProperty(nameof(SettingsTypes.Derived.Name));

            var comparer = MemberInfoComparer<PropertyInfo>.Default;
            Assert.AreEqual(true, comparer.Equals(complexNameProperty, derivedNameProperty));
            Assert.AreEqual(comparer.GetHashCode(complexNameProperty), comparer.GetHashCode(derivedNameProperty));
        }

        [Test]
        public void CompareDifferent()
        {
            var complexNameProperty = typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Name));
            var complexValueProperty = typeof(SettingsTypes.ComplexType).GetProperty(nameof(SettingsTypes.ComplexType.Value));

            var comparer = MemberInfoComparer<PropertyInfo>.Default;
            Assert.AreEqual(false, comparer.Equals(complexNameProperty, complexValueProperty));
            Assert.AreNotEqual(comparer.GetHashCode(complexNameProperty), comparer.GetHashCode(complexValueProperty));
        }
    }
}