namespace Gu.State.Tests.Internals.Refelection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;

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

            [Test]
            public void Fuu()
            {
                var a1 = ImmutableArray.Create(1, 2);
                var a2 = ImmutableArray.Create(1, 2);
                Console.WriteLine("a1 is IEquatable<ImmutableArray<int>>: {0}", a1 is IEquatable<ImmutableArray<int>>);
                Console.WriteLine("a1 == a2: {0}", a1 == a2);
                Console.WriteLine("object.Equals(a1, a2): {0}", object.Equals(a1, a2));
                Console.WriteLine("a1.Equals(a2): {0}", a1.Equals(a2));
            }
        }
    }
}