// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using NUnit.Framework;

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

        //[TestCaseSource(typeof(TestCases), nameof(TestCases.WhenEqualStructural))]
        //public void ExplicitReferences(object x, object y)
        //{
        //    Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.References));
        //    Assert.AreEqual(false, EqualBy.PropertyValues(y, x, ReferenceHandling.References));
        //}
    }
}