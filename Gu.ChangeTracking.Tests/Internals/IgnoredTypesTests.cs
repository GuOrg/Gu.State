namespace Gu.ChangeTracking.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    class IgnoredTypesTests
    {
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionTypes(Type type)
        {
            var ignoredTypes = IgnoredTypes.Create(null);
            Assert.AreEqual(true, ignoredTypes.IsIgnoringType(type));
        }

        [Test]
        public void IgnoresSimple()
        {
            var ignoredType = typeof(IgnoredType);
            var ignoredTypes = IgnoredTypes.Create(new[] { ignoredType });
            Assert.AreEqual(true, ignoredTypes.IsIgnoringType(ignoredType));
            Assert.AreEqual(false, ignoredTypes.IsIgnoringType(typeof(IgnoredGenericType<int>)));
        }

        [Test]
        public void IgnoresGeneric()
        {
            var ignoredType = typeof(IgnoredGenericType<int>);
            var ignoredTypes = IgnoredTypes.Create(new[] { ignoredType });
            Assert.AreEqual(true, ignoredTypes.IsIgnoringType(ignoredType));
            Assert.AreEqual(false, ignoredTypes.IsIgnoringType(typeof(IgnoredGenericType<double>)));
        }

        [Test]
        public void IgnoresOpenGeneric()
        {
            var ignoredType = typeof(IgnoredGenericType<>);
            var ignoredTypes = IgnoredTypes.Create(new[] { ignoredType });
            Assert.AreEqual(true, ignoredTypes.IsIgnoringType(ignoredType));
            Assert.AreEqual(true, ignoredTypes.IsIgnoringType(typeof(IgnoredGenericType<double>)));
        }

        class IgnoredType
        {
        }

        class IgnoredGenericType<T>
        {
        }
    }
}
