// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable INPC013 // Use nameof.
namespace Gu.State.Tests.CopyTests
{
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static CopyTypes;

    public abstract class ReferenceLoops
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excluded = null)
            where T : class;

        [TestCase("parent", "child")]
        [TestCase("", "child")]
        [TestCase("parent", "")]
        public void ParentChild(string p, string c)
        {
            var source = new Parent("parent", new Child("child"));
            var target = new Parent(p, new Child(c));
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("parent", source.Name);
            Assert.AreEqual("parent", target.Name);
            Assert.AreEqual("child", source.Child.Name);
            Assert.AreEqual("child", target.Child.Name);
        }

        [Test]
        public void ParentChildWhenSourceChildIsNull()
        {
            var source = new Parent("parent", null);
            var target = new Parent(null, new Child("child"));
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("parent", source.Name);
            Assert.AreEqual("parent", target.Name);
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

            var source = new Parent("parent", new Child("child"));
            var target = new Parent(null, null);
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("parent", source.Name);
            Assert.AreEqual("parent", target.Name);
            Assert.AreEqual("child", source.Child.Name);
            Assert.AreEqual("child", target.Child.Name);
        }

        [Test]
        public void CollectionWithSame()
        {
            var source = new ObservableCollection<ComplexType>();
            var complexType = new ComplexType("a", 1);
            source.Add(complexType);
            source.Add(complexType);
            var target = new ObservableCollection<ComplexType>();

            this.CopyMethod(source, target, ReferenceHandling.Structural);
            var expected = new[] { new ComplexType("a", 1), new ComplexType("a", 1) };
            CollectionAssert.AreEqual(expected, source, ComplexType.Comparer);
            Assert.AreSame(source[0], source[1]);

            CollectionAssert.AreEqual(expected, target, ComplexType.Comparer);
            Assert.Inconclusive("Assert.AreSame(target[0], target[1])");
        }

        [Explicit(IgnoredTests.NewFeature)]
        [Test]
        public void CollectionWithSelf()
        {
            var source = new ObservableCollection<object>();
            source.Add(source);
            var target = new ObservableCollection<object>();

            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual(1, source.Count);
            Assert.AreEqual(1, target.Count);

            Assert.AreEqual(1, ((ObservableCollection<object>)source[0]).Count);
            Assert.AreEqual(1, ((ObservableCollection<object>)source[0]).Count);
        }
    }
}