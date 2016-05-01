namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class CustomComparerTests
    {
        public abstract bool EqualMethod<T, TValue>(
            T x,
            T y,
            IEqualityComparer<TValue> comparer,
            ReferenceHandling referenceHandling = ReferenceHandling.Throw)
            where T : class;

        [TestCase("b", "b", true)]
        [TestCase("b", "c", false)]
        public void WithSimpleHappyPath(string xn, string yn, bool expected)
        {
            var x = new WithSimpleProperties(1, 2, xn, StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(1, 2, yn, StringSplitOptions.RemoveEmptyEntries);
            var result = this.EqualMethod(x, y, WithSimpleProperties.AllMembersComparer);
            Assert.AreEqual(expected, result);

            //result = this.EqualMethod(x, y, ReferenceHandling.Throw);
            //Assert.AreEqual(expected, result);

            //result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            //Assert.AreEqual(expected, result);

            //result = this.EqualMethod(x, y, ReferenceHandling.References);
            //Assert.AreEqual(expected, result);
        }
    }
}