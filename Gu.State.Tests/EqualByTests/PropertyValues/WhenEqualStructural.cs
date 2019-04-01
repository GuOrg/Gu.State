// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using NUnit.Framework;

    public class WhenEqualStructural
    {
        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenEqualStructural))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(true, State.EqualBy.PropertyValues(x, y));
            Assert.AreEqual(true, State.EqualBy.PropertyValues(y, x));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenEqualStructural))]
        public void Explicit(object x, object y)
        {
            Assert.AreEqual(true, State.EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(true, State.EqualBy.PropertyValues(y, x, ReferenceHandling.Structural));
        }
    }
}