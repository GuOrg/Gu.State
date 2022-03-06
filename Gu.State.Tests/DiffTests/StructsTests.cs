// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.DiffTests
{
    using System;
    using System.Drawing;
    using NUnit.Framework;

    using static DiffTypes;

    public abstract class StructsTests
    {
        public abstract Diff DiffBy<T>(
            T x,
            T y,
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
            where T : class;

        [Test]
        public void WithPointWhenEqual()
        {
            var x = new With<Point>(new Point(1, 2));
            var y = new With<Point>(new Point(1, 2));
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void WithNullablePointWhenEqual()
        {
            var x = new With<Point?>(new Point(1, 2));
            var y = new With<Point?>(new Point(1, 2));
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [Test]
        public void WithNullablePointWhenNull()
        {
            var x = new With<Point?>(null);
            var y = new With<Point?>(null);
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(x, y, ReferenceHandling.References);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }
    }
}
