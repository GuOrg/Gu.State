﻿// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using NUnit.Framework;

    public class WhenEqual
    {
        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenEqual))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(true, State.EqualBy.PropertyValues(x, y));
            Assert.AreEqual(true, State.EqualBy.PropertyValues(y, x));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenEqual))]
        public void ReferenceHandlingThrow(object x, object y)
        {
            Assert.AreEqual(true, State.EqualBy.PropertyValues(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(true, State.EqualBy.PropertyValues(y, x, ReferenceHandling.Throw));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenEqual))]
        public void ReferenceHandlingStructural(object x, object y)
        {
            Assert.AreEqual(true, State.EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(true, State.EqualBy.PropertyValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenEqual))]
        public void ReferenceHandlingReferences(object x, object y)
        {
            Assert.AreEqual(true, State.EqualBy.PropertyValues(x, y, ReferenceHandling.References));
            Assert.AreEqual(true, State.EqualBy.PropertyValues(y, x, ReferenceHandling.References));
        }
    }
}