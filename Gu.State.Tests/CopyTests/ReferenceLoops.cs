namespace Gu.State.Tests.CopyTests
{
    using System.Collections;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    public abstract class ReferenceLoops
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excluded = null) where T : class;

        [TestCase("p", "c", true)]
        [TestCase("", "c", false)]
        [TestCase("p", "", false)]
        public void ParentChild(string p, string c, bool expected)
        {
            var source = new CopyTypes.Parent("p", new CopyTypes.Child("c"));
            var target = new CopyTypes.Parent(p, new CopyTypes.Child(c));
            this.CopyMethod(source, target, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual("p", source.Name);
            Assert.AreEqual("p", target.Name);
            Assert.AreEqual("c", source.Child.Name);
            Assert.AreEqual("c", target.Child.Name);
        }

        [Test]
        public void ParentChildWhenSourceChildIsNull()
        {
            var source = new CopyTypes.Parent("p", null);
            var target = new CopyTypes.Parent(null, new CopyTypes.Child("c"));
            this.CopyMethod(source, target, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual("p", source.Name);
            Assert.AreEqual("p", target.Name);
            Assert.AreEqual(null, source.Child);
            Assert.AreEqual(null, target.Child);
        }

        [Test]
        public void ParentChildWhenTargetChildIsNull()
        {
            if (this is FieldValues.ReferenceLoops)
            {
                Assert.Inconclusive("Not supporting this");
            }
            var source = new CopyTypes.Parent("p", new CopyTypes.Child("c"));
            var target = new CopyTypes.Parent(null, null);
            this.CopyMethod(source, target, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual("p", source.Name);
            Assert.AreEqual("p", target.Name);
            Assert.AreEqual("c", source.Child.Name);
            Assert.AreEqual("c", target.Child.Name);
        }

        [Test]
        public void CollectionWithSame()
        {
            var source = new ObservableCollection<CopyTypes.ComplexType>();
            var complexType = new CopyTypes.ComplexType("a", 1);
            source.Add(complexType);
            source.Add(complexType);
            var target = new ObservableCollection<CopyTypes.ComplexType>();

            this.CopyMethod(source, target, ReferenceHandling.StructuralWithReferenceLoops);
            var expected = new[] { new CopyTypes.ComplexType("a", 1), new CopyTypes.ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, CopyTypes.ComplexType.Comparer);
            CollectionAssert.AreEqual(expected, target, CopyTypes.ComplexType.Comparer);
        }

        [Test]
        public void CollectionWithSelf()
        {
            Assert.Inconclusive("Probably not possible without references");
            var source = new ObservableCollection<ICollection>();
            source.Add(source);
            var target = new ObservableCollection<ICollection>();

            this.CopyMethod(source, target, ReferenceHandling.StructuralWithReferenceLoops);
            Assert.AreEqual(1, source.Count);
            Assert.AreEqual(1, target.Count);

            Assert.AreEqual(1, source[0].Count);
            Assert.AreEqual(1, target[0].Count);
        }
    }
}