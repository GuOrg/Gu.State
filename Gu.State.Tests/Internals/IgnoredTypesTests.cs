namespace Gu.State.Tests.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public class IgnoredTypesTests
    {
        [TestCase(typeof(List<int>))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(Collection<int>))]
        [TestCase(typeof(ObservableCollection<int>))]
        [TestCase(typeof(Dictionary<int, int>))]
        public void IgnoresCollectionTypes(Type type)
        {
            var ignoredTypes = KnownTypes.Create(null);
            Assert.AreEqual(true, ignoredTypes.IsKnownType(type));
        }

        [Test]
        public void IgnoresSimple()
        {
            var ignoredType = typeof(IgnoredType);
            var ignoredTypes = KnownTypes.Create(new[] { ignoredType });
            Assert.AreEqual(true, ignoredTypes.IsKnownType(ignoredType));
            Assert.AreEqual(false, ignoredTypes.IsKnownType(typeof(IgnoredGenericType<int>)));
        }

        [Test]
        public void IgnoresGeneric()
        {
            var ignoredType = typeof(IgnoredGenericType<int>);
            var ignoredTypes = KnownTypes.Create(new[] { ignoredType });
            Assert.AreEqual(true, ignoredTypes.IsKnownType(ignoredType));
            Assert.AreEqual(false, ignoredTypes.IsKnownType(typeof(IgnoredGenericType<double>)));
        }

        [Test]
        public void IgnoresOpenGeneric()
        {
            var ignoredType = typeof(IgnoredGenericType<>);
            var ignoredTypes = KnownTypes.Create(new[] { ignoredType });
            Assert.AreEqual(true, ignoredTypes.IsKnownType(ignoredType));
            Assert.AreEqual(true, ignoredTypes.IsKnownType(typeof(IgnoredGenericType<double>)));
        }

        private class IgnoredType
        {
        }

        private class IgnoredGenericType<T>
        {
        }
    }
}
