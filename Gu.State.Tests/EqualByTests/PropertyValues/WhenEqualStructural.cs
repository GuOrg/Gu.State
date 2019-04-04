// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using NUnit.Framework;

    using static EqualByTypes;

    public class WhenEqualStructural
    {
        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(true, EqualBy.PropertyValues(x, y));
            Assert.AreEqual(true, EqualBy.PropertyValues(y, x));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        public void ExplicitStructural(object x, object y)
        {
            Assert.AreEqual(true, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(true, EqualBy.PropertyValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        public void ExplicitReferences(object x, object y)
        {
            if (x is IWith xw &&
                xw.Value is null &&
                y is IWith yw &&
                yw.Value is null)
            {
                Assert.AreEqual(true, EqualBy.PropertyValues(x, y, ReferenceHandling.References));
                Assert.AreEqual(true, EqualBy.PropertyValues(y, x, ReferenceHandling.References));
            }
            else
            {
                Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.References));
                Assert.AreEqual(false, EqualBy.PropertyValues(y, x, ReferenceHandling.References));
            }
        }
    }
}