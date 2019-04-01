// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using NUnit.Framework;

    public class WhenNotEqualStructural
    {
        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqualStructural))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, State.EqualBy.FieldValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqualStructural))]
        public void Explicit(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, State.EqualBy.FieldValues(y, x, ReferenceHandling.Structural));
        }
    }
}