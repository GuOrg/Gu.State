// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests
{
    using System;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class ReferenceLoopsTests
    {
        public abstract bool EqualMethod<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null) where T : class;

        [TestCase("p", "c", true)]
        [TestCase("", "c", false)]
        [TestCase("p", "", false)]
        public void ParentChild(string p, string c, bool expected)
        {
            var x = new Parent("p", new Child("c"));
            Assert.AreSame(x, x.Child.Parent);

            var y = new Parent(p, new Child(c));
            Assert.AreSame(y, y.Child.Parent);
            var result = this.EqualMethod(x, y, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);

            result = this.EqualMethod(y, x, ReferenceHandling.Structural);
            Assert.AreEqual(expected, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ParentChildWhenTargetChildIsNull(ReferenceHandling referenceHandling)
        {
            var x = new Parent("p", new Child("c"));
            var y = new Parent("p", null);
            var result = this.EqualMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void ParentChildWhenSourceChildIsNull(ReferenceHandling referenceHandling)
        {
            var x = new Parent("p", null);
            var y = new Parent("p", new Child("c"));
            var result = this.EqualMethod(x, y, referenceHandling);
            Assert.AreEqual(false, result);

            result = this.EqualMethod(y, x, referenceHandling);
            Assert.AreEqual(false, result);
        }
    }
}