// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable INPC013 // Use nameof.
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class ReferenceLoopsTests
    {
        public abstract bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null)
            where T : class;

        [TestCase("parent", "child", true)]
        [TestCase("", "child", false)]
        [TestCase("parent", "", false)]
        public void ParentChild(string p, string c, bool expected)
        {
            var x = new Parent("parent", new Child("child"));
            Assert.AreSame(x, x.Child.Parent);

            var y = new Parent(p, new Child(c));
            Assert.AreSame(y, y.Child.Parent);
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [TestCase("parent", "child", true)]
        [TestCase("", "child", false)]
        [TestCase("parent", "", false)]
        public void SealedParentChild(string p, string c, bool expected)
        {
            var x = new SealedParent("parent", new SealedChild("child"));
            Assert.AreSame(x, x.Child.Parent);

            var y = new SealedParent(p, new SealedChild(c));
            Assert.AreSame(y, y.Child.Parent);
            var result = this.EqualBy(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualBy(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [TestCase(ReferenceHandling.Structural, true)]
        [TestCase(ReferenceHandling.References, false)]
        public void WithGetSetObject(ReferenceHandling referenceHandling, bool expected)
        {
            var x = new WithGetSet<object>(null);
            x.Value = x;

            var y = new WithGetSet<object>(null);
            y.Value = y;

            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(expected, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(expected, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ParentChildWhenTargetChildIsNull(ReferenceHandling referenceHandling)
        {
            var x = new Parent("parent", new Child("child"));
            var y = new Parent("parent", null);
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ParentChildWhenSourceChildIsNull(ReferenceHandling referenceHandling)
        {
            var x = new Parent("parent", null);
            var y = new Parent("parent", new Child("child"));
            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural, true)]
        [TestCase(ReferenceHandling.References, false)]
        public void ArrayWithSelf(ReferenceHandling referenceHandling, bool expected)
        {
            var x = new object[1];
            x[0] = x;

            var y = new object[1];
            y[0] = y;

            var result = this.EqualBy(x, y, referenceHandling);
            Assert.AreEqual(expected, result);

            result = this.EqualBy(y, x, referenceHandling);
            Assert.AreEqual(expected, result);
        }
    }
}