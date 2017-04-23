namespace Gu.State.Tests.Internals.Reflection
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    public partial class TypeExtTests
    {
        public class ImmutableCollections
        {
            //[TestCase(typeof(System.Collections.Immutable.IImmutableList<int>), true)]
            [TestCase(typeof(System.Collections.Immutable.ImmutableList<int>), true)]
            [TestCase(typeof(List<int>), false)]
            public void IsImmutableList(Type type, bool expected)
            {
                Assert.AreEqual(expected, type.IsImmutableList());
            }

            [TestCase(typeof(System.Collections.Immutable.ImmutableArray<int>), true)]
            [TestCase(typeof(int[]), false)]
            public void IsImmutableArray(Type type, bool expected)
            {
                Assert.AreEqual(expected, type.IsImmutableArray());
            }

            [TestCase(typeof(System.Collections.Immutable.ImmutableHashSet<int>), true)]
            [TestCase(typeof(HashSet<int>), false)]
            public void IsImmutableHashSet(Type type, bool expected)
            {
                Assert.AreEqual(expected, type.IsImmutableHashSet());
            }

            [TestCase(typeof(System.Collections.Immutable.ImmutableDictionary<int, int>), true)]
            [TestCase(typeof(HashSet<int>), false)]
            public void IsImmutableDictionary(Type type, bool expected)
            {
                Assert.AreEqual(expected, type.IsImmutableDictionary());
            }
        }
    }
}