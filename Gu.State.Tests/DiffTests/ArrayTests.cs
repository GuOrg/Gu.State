﻿namespace Gu.State.Tests.DiffTests
{
    using System.Linq;

    using NUnit.Framework;

    using static DiffTypes;

    public abstract class ArrayTests
    {
        public abstract Diff DiffBy<T>(T source, T target, ReferenceHandling referenceHandling);

        [TestCase("1, 2, 3", "1, 2, 3", "Empty")]
        [TestCase("1, 2, 3", "1, 2", "int[] [2] x: 3 y: missing item")]
        [TestCase("1, 2", "1, 2, 3", "int[] [2] x: missing item y: 3")]
        [TestCase("5, 2, 3", "1, 2, 3", "int[] [0] x: 5 y: 1")]
        public void IntsStructural(string xs, string ys, string expected)
        {
            var x = xs.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var y = ys.Split(',')
                      .Select(int.Parse)
                      .ToArray();
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void IntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = new[] { 1, 2, 3 };
            var y = new[] { 1, 2, 3 };
            var result = this.DiffBy(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ImmutableArrayOfIntsWhenEqual(ReferenceHandling referenceHandling)
        {
            var x = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
            var y = System.Collections.Immutable.ImmutableArray.Create(1, 2, 3);
            var result = this.DiffBy(x, y, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());

            result = this.DiffBy(y, x, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }

        [TestCase(0, 0, 1, "Empty")]
        [TestCase(0, 0, 10, "int[,] [0,0] x: 1 y: 10")]
        [TestCase(2, 1, 10, "int[,] [2,1] x: 6 y: 10")]
        public void Ints2D(int i1, int i2, int yValue, string expected)
        {
            var x = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            var y = new[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            y[i1, i2] = yValue;
            var result = this.DiffBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void Ints2DWhenRankDiffers(ReferenceHandling referenceHandling)
        {
            var x = new int[2, 3];
            var y = new int[3, 2];
            var result = this.DiffBy(x, y, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual("int[,] x: [2,3] y: [3,2]", result.ToString(string.Empty, " "));

            result = this.DiffBy(y, x, referenceHandling);
            Assert.AreEqual(false, result.IsEmpty);
            Assert.AreEqual("int[,] x: [3,2] y: [2,3]", result.ToString(string.Empty, " "));
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void EqualImmutables(ReferenceHandling referenceHandling)
        {
            var source = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var target = new[] { new Immutable(1), new Immutable(2), new Immutable(3) };
            var result = this.DiffBy(source, target, referenceHandling);
            Assert.AreEqual(true, result.IsEmpty);
            Assert.AreEqual("Empty", result.ToString());
        }
    }
}