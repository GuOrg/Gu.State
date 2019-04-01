﻿// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using NUnit.Framework;

    public class WhenEqualStructural
    {
        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(true, EqualBy.FieldValues(x, y));
            Assert.AreEqual(true, EqualBy.FieldValues(y, x));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        public void ExplicitStructural(object x, object y)
        {
            Assert.AreEqual(true, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(true, EqualBy.FieldValues(y, x, ReferenceHandling.Structural));
        }

        //[TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        //public void ExplicitReferences(object x, object y)
        //{
        //    Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.References));
        //    Assert.AreEqual(false, EqualBy.FieldValues(y, x, ReferenceHandling.References));
        //}
    }
}