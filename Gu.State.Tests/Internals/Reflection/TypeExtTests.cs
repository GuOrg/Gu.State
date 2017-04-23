﻿namespace Gu.State.Tests.Internals.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using NUnit.Framework;

    public partial class TypeExtTests
    {
        [TestCase(typeof(ISet<>), true)]
        [TestCase(typeof(IEnumerable<>), true)]
        [TestCase(typeof(IEnumerable), false)]
        [TestCase(typeof(IList), false)]
        [TestCase(typeof(ISet<int>), false)]
        [TestCase(typeof(ISet<string>), false)]
        public void IsOpenGenericType(Type type, bool expected)
        {
            var actual = type.IsOpenGenericType();
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(HashSet<int>), typeof(ISet<>), true)]
        [TestCase(typeof(HashSet<int>), typeof(IEnumerable<>), true)]
        [TestCase(typeof(HashSet<int>), typeof(IEnumerable), true)]
        [TestCase(typeof(HashSet<int>), typeof(IList), false)]
        [TestCase(typeof(HashSet<int>), typeof(ISet<int>), true)]
        [TestCase(typeof(HashSet<int>), typeof(ISet<string>), false)]
        [TestCase(typeof(Dictionary<int, int>), typeof(IDictionary<,>), true)]
        public void Implements(Type type, Type interfaceType, bool expected)
        {
            var actual = type.Implements(interfaceType);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(HashSet<int>), typeof(ISet<>), typeof(int), true)]
        [TestCase(typeof(HashSet<int>), typeof(IEnumerable<>), typeof(int), true)]
        [TestCase(typeof(HashSet<int>), typeof(ISet<>), typeof(string), false)]
        //[TestCase(typeof(HashSet<int>), typeof(IEnumerable), typeof(string), false)]
        public void Implements(Type type, Type interfaceType, Type parameterType, bool expected)
        {
            var actual = type.Implements(interfaceType, parameterType);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(typeof(IEnumerable<int>), typeof(int))]
        [TestCase(typeof(List<int>), typeof(int))]
        [TestCase(typeof(int[]), typeof(int))]
        [TestCase(typeof(int[,]), typeof(int))]
        public void GetItemType(Type enumerableType, Type expected)
        {
            var itemType = enumerableType.GetItemType();
            Assert.AreEqual(expected, itemType);
        }
    }
}
