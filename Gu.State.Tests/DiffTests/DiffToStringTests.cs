namespace Gu.State.Tests.DiffTests
{
    using NUnit.Framework;

    using static DiffTypes;

    public class DiffToStringTests
    {
        [TestCase("a", "b", "StringValue x: a y: b")]
        [TestCase(null, "b", "StringValue x: null y: b")]
        public void PropertyDiff(string x, string y, string expected)
        {
            var diff = new PropertyDiff(typeof(WithSimpleProperties).GetProperty(nameof(WithSimpleProperties.StringValue)), x, y);
            Assert.AreEqual(expected, diff.ToString());
        }

        [Test]
        public void WithPropertyDiff()
        {
            Assert.Inconclusive();
            var propertyDiff = new PropertyDiff(typeof(WithSimpleProperties).GetProperty(nameof(WithSimpleProperties.StringValue)), "a", "b");
            var diff = new Diff(new[] { propertyDiff });
            var expected = "WithSimpleProperties\r\n" +
                           "  StringValue x: a y: b";
            Assert.AreEqual(expected, diff.ToString());
        }
    }
}
