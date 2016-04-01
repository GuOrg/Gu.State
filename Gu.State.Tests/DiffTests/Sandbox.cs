namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using static DiffTypes;

    public class Sandbox
    {
        [Test]
        public void WithPropertyDiff()
        {
            var settings = PropertiesSettings.GetOrCreate();
            var x = new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(1, 2, "b", StringSplitOptions.RemoveEmptyEntries);
            var result = DiffBy.PropertyValues(x, y, settings);
            Assert.AreEqual(result.X, x);
            Assert.AreEqual(result.Y, y);
            var propertyDiff = (PropertyDiff)result.Diffs.Single();
            Assert.AreEqual(x.GetType().GetProperty(nameof(WithSimpleProperties.StringValue)), propertyDiff.PropertyInfo);
            Assert.AreEqual(x.StringValue, propertyDiff.X);
            Assert.AreEqual(y.StringValue, propertyDiff.Y);
            CollectionAssert.IsEmpty(propertyDiff.Diffs);
            Console.Write(result);

            var expected = "x: Gu.State.Tests.DiffTests.DiffTypes+WithSimpleProperties y: Gu.State.Tests.DiffTests.DiffTypes+WithSimpleProperties diffs: 1";
            Assert.AreEqual(expected, result.ToString());

            expected = "WithSimpleProperties StringValue x: a y: b";
            var actual = result.ToString("", " ");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ValueDiff()
        {
            var settings = PropertiesSettings.GetOrCreate();
            var x = 1;
            var y = 2;
            var result = (ValueDiff)DiffBy.PropertyValues(x, y, settings);
            Assert.AreEqual(x, result.X);
            Assert.AreEqual(y, result.Y);
            CollectionAssert.IsEmpty(result.Diffs);
            var expected = "x: 1 y: 2 diffs: 0";
            Assert.AreEqual(expected, result.ToString());

            expected = "int x: 1 y: 2";
            Assert.AreEqual(expected, result.ToString("", ""));
        }
    }
}
