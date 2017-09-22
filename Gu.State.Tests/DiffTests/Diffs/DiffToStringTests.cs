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
            Assert.AreEqual(expected, diff.ToString(string.Empty, " "));
        }

        [Test]
        public void Loop()
        {
            //var p1 = new Parent("p1", new Child("c"));
            //var p2 = new Parent("p2", new Child("c"));
            //var nameDiff = new PropertyDiff(typeof(Parent).GetProperty(nameof(Parent.Name)), "p1", "p2");
            //var parentPropertyDiff = new PropertyDiff(typeof(Child).GetProperty(nameof(Child.Parent)), p1, p2);
            //var childPropertyDiff = new PropertyDiff(typeof(Parent).GetProperty(nameof(Parent.Child)), p1.Child, p2.Child);
            //var childDiffs = new List<SubDiff>();
            //var childValueDiff = new ValueDiff(p1.Child, p2.Child, childDiffs);
            //var parentValueDiff = new ValueDiff(p1, p2, new List<SubDiff> { nameDiff, parentPropertyDiff });
            //var expected = "WithSimpleProperties";
            //Assert.AreEqual(expected, parentValueDiff.ToString("", " "));
        }
    }
}
