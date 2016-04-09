namespace Gu.State.Tests.DiffTests
{
    using System;

    using NUnit.Framework;

    public abstract class ReferenceLoopsTests
    {
        public abstract Diff DiffMethod<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null) where T : class;

        [TestCase("p", "c", null)]
        [TestCase("", "c", "Parent <member2> x: p y: ")]
        [TestCase("p", "", "Parent <member1> <member2> x: c y: ")]
        public void ParentChild(string p, string c, string expected)
        {
            expected = expected?.Replace("<member1>", this is FieldValues.ReferenceLoops ? "<Child>k__BackingField" : "Child")
                                .Replace("<member2>", this is FieldValues.ReferenceLoops ? "<Name>k__BackingField" : "Name");
            var x = new DiffTypes.Parent("p", new DiffTypes.Child("c"));
            var y = new DiffTypes.Parent(p, new DiffTypes.Child(c));
            var result = this.DiffMethod(x, y, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual(expected, result?.ToString("", " "));

            result = this.DiffMethod(x, y, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual(expected, result?.ToString("", " "));
        }

        [Test]
        public void ParentChildWhenTargetChildIsNull()
        {
            var x = new DiffTypes.Parent("p", new DiffTypes.Child("c"));
            var y = new DiffTypes.Parent("p", null);
            var result = this.DiffMethod(x, y, ReferenceHandling.StructuralWithReferenceLoops);
            var expected = this is FieldValues.ReferenceLoops
                               ? "Parent <Child>k__BackingField x: Gu.State.Tests.DiffTests.DiffTypes+Child y: null"
                               : "Parent Child x: Gu.State.Tests.DiffTests.DiffTypes+Child y: null";
            Assert.AreEqual(expected, result.ToString("", " "));

            //result = this.EqualMethod(y, x, ReferenceHandling.Structural);
            //Assert.AreEqual(false, result);

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(expected, result.ToString("", " "));
        }

        [Test]
        public void ParentChildWhenSourceChildIsNull()
        {
            var x = new DiffTypes.Parent("p", null);
            var y = new DiffTypes.Parent("p", new DiffTypes.Child("c"));
            var result = this.DiffMethod(x, y, ReferenceHandling.StructuralWithReferenceLoops);
            var expected = this is FieldValues.ReferenceLoops
                               ? "Parent <Child>k__BackingField x: null y: Gu.State.Tests.DiffTests.DiffTypes+Child"
                               : "Parent Child x: null y: Gu.State.Tests.DiffTests.DiffTypes+Child";
            Assert.AreEqual(expected, result.ToString("", " "));

            //result = this.EqualMethod(y, x, ReferenceHandling.Structural);
            //Assert.AreEqual(false, result);

            result = this.DiffMethod(x, y, ReferenceHandling.References);
            Assert.AreEqual(expected, result.ToString("", " "));
        }
    }
}