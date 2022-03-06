namespace Gu.State.Tests.Internals.Reflection
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    public partial class TypeExtTests
    {
        public class ImmutableCollections
        {
            [TestCase(typeof(System.Collections.Immutable.IImmutableList<int>), true)]
            [TestCase(typeof(System.Collections.Immutable.ImmutableList<int>), true)]
            [TestCase(typeof(List<int>), false)]
            public void IsInSystemCollectionsImmutable(Type type, bool expected)
            {
                Assert.AreEqual(expected, type.IsInSystemCollectionsImmutable());
            }
        }
    }
}
