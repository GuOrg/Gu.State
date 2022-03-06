// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using NUnit.Framework;

    public class WhenNotEqualStructural
    {
        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqualStructural))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, EqualBy.FieldValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqualStructural))]
        public void ExplicitStructural(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, EqualBy.FieldValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(TestCases), nameof(TestCases.WhenNotEqualStructural))]
        public void ExplicitReferences(object x, object y)
        {
            Assert.AreEqual(false, EqualBy.FieldValues(x, y, ReferenceHandling.References));
            Assert.AreEqual(false, EqualBy.FieldValues(y, x, ReferenceHandling.References));
        }
    }
}
