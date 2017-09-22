// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using NUnit.Framework;

    using static SynchronizeTypes;

    public partial class SynchronizeTests
    {
        public class ReferenceLoops
        {
            [Test]
            public void CreateAndDisposeParentChild()
            {
                var source = new Parent("a", new Child("b"));
                var target = new Parent("b", new Child());
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.Structural))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual("b", source.Child.Name);
                    Assert.AreEqual("b", target.Child.Name);

                    source.Name = "a1";
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual("b", source.Child.Name);
                    Assert.AreEqual("b", target.Child.Name);

                    source.Child.Name = "b1";
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual("b1", source.Child.Name);
                    Assert.AreEqual("b1", target.Child.Name);
                    var sc = source.Child;
                    var tc = target.Child;

                    source.Child = null;
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual(null, source.Child);
                    Assert.AreEqual(null, target.Child);

                    sc.Name = "new";
                    Assert.AreEqual("b1", tc.Name);
                    Assert.AreEqual("a1", source.Name);
                    Assert.AreEqual("a1", target.Name);
                    Assert.AreEqual(null, source.Child);
                    Assert.AreEqual(null, target.Child);
                }

                source.Name = "_";
                Assert.AreEqual("_", source.Name);
                Assert.AreEqual("a1", target.Name);
                Assert.AreEqual(null, source.Child);
                Assert.AreEqual(null, target.Child);
            }
        }
    }
}