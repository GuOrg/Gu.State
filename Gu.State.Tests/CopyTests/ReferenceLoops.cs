﻿// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.CopyTests
{
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static CopyTypes;

    public abstract class ReferenceLoops
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excluded = null) where T : class;

        [TestCase("p", "c", true)]
        [TestCase("", "c", false)]
        [TestCase("p", "", false)]
        public void ParentChild(string p, string c, bool expected)
        {
            var source = new Parent("p", new Child("c"));
            var target = new Parent(p, new Child(c));
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("p", source.Name);
            Assert.AreEqual("p", target.Name);
            Assert.AreEqual("c", source.Child.Name);
            Assert.AreEqual("c", target.Child.Name);
        }

        [Test]
        public void ParentChildWhenSourceChildIsNull()
        {
            var source = new Parent("p", null);
            var target = new Parent(null, new Child("c"));
            this.CopyMethod(source, target, ReferenceHandling.Structural);
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

            var source = new Parent("p", new Child("c"));
            var target = new Parent(null, null);
            this.CopyMethod(source, target, ReferenceHandling.Structural);
            Assert.AreEqual("p", source.Name);
            Assert.AreEqual("p", target.Name);
            Assert.AreEqual("c", source.Child.Name);
            Assert.AreEqual("c", target.Child.Name);
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