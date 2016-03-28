namespace Gu.State.Tests.DiffTests
{
    using System;

    using NUnit.Framework;

    using static DiffTypes;

    public class Sandbox
    {
        [Test]
        public void TestName()
        {
            var x = new WithSimpleProperties(1, 2, "a", StringSplitOptions.RemoveEmptyEntries);
            var y = new WithSimpleProperties(1, 2, "b", StringSplitOptions.RemoveEmptyEntries);
            var expected = "WithSimpleProperties\r\n StringValue: b c";
            var result = DiffBy.PropertyValues(x, y);
            Assert.AreEqual(expected, result.ToString());
        }
    }
}
