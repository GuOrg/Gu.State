// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests.PropertyValues
{
    using NUnit.Framework;

    public class WhenNotEqualStructural
    {
        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqualStructural))]
        public void Default(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.PropertyValues(x, y));
            Assert.AreEqual(false, State.EqualBy.PropertyValues(y, x));
        }

        [TestCaseSource(typeof(Gu.State.Tests.EqualByTests.TestCases), nameof(Gu.State.Tests.EqualByTests.TestCases.WhenNotEqualStructural))]
        public void Explicit(object x, object y)
        {
            Assert.AreEqual(false, State.EqualBy.PropertyValues(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, State.EqualBy.PropertyValues(y, x, ReferenceHandling.Structural));
        }
    }
    }