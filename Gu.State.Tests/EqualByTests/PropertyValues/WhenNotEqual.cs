// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using NUnit.Framework;

    public class WhenNotEqual
    {
        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqual))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.PropertyValues(x, y));
            Assert.AreEqual(false, EqualBy.PropertyValues(y, x));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqual))]
        public void ReferenceHandlingThrow(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(false, EqualBy.PropertyValues(y, x, ReferenceHandling.Throw));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqual))]
        public void ReferenceHandlingStructural(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, EqualBy.PropertyValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqual))]
        public void ReferenceHandlingReferences(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.PropertyValues(x, y, ReferenceHandling.References));
            Assert.AreEqual(false, EqualBy.PropertyValues(y, x, ReferenceHandling.References));
        }
    }
}
