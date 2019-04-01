// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.FieldValues
{
    using NUnit.Framework;

    public class WhenNotEqual
    {
        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqual))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.FieldValues(x, y));
            Assert.AreEqual(false, State.EqualBy.FieldValues(y, x));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqual))]
        public void ReferenceHandlingThrow(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.FieldValues(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(false, State.EqualBy.FieldValues(y, x, ReferenceHandling.Throw));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqual))]
        public void ReferenceHandlingStructural(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.FieldValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, State.EqualBy.FieldValues(y, x, ReferenceHandling.Structural));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqual))]
        public void ReferenceHandlingReferences(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.FieldValues(x, y, ReferenceHandling.References));
            Assert.AreEqual(false, State.EqualBy.FieldValues(y, x, ReferenceHandling.References));
        }
    }
}