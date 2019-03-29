namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class StructsTests
    {
        public abstract bool EqualBy<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null)
            where T : class;

        [TestCase("1, 2", "1, 2", true)]
        [TestCase("1, 2", "2, 2", false)]
        [TestCase("1, 2", "1, 3", false)]
        public void WithPoint(string xs, string ys, bool expected)
        {
            var x = new With<Point>(Parse(xs));
            var y = new With<Point>(Parse(xs));
            var result = this.EqualBy(x, y);
            Assert.AreEqual(true, result);

            Point Parse(string text)
            {
                var ds = text.Split(',')
                             .Select(t => int.Parse(t, CultureInfo.InvariantCulture))
                             .ToArray();
                return new Point(ds[0], ds[1]);
            }
        }

        [TestCase("1, 2", "1, 2", true)]
        [TestCase("1, 2", "2, 2", false)]
        [TestCase("1, 2", "1, 3", false)]
        [TestCase(null, null, true)]
        [TestCase(null, "1,2", false)]
        [TestCase("1,2", null, false)]
        public void WithNullablePoint(string xs, string ys, bool expected)
        {
            var x = new With<Point?>(Parse(xs));
            var y = new With<Point?>(Parse(xs));
            var result = this.EqualBy(x, y);
            Assert.AreEqual(true, result);

            Point? Parse(string text)
            {
                if (text == null)
                {
                    return null;
                }

                var ds = text.Split(',')
                             .Select(t => int.Parse(t, CultureInfo.InvariantCulture))
                             .ToArray();
                return new Point(ds[0], ds[1]);
            }
        }
    }
}