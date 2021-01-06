namespace Gu.State.Tests
{
    using NUnit.Framework;
    using static SynchronizeTypes;

    public static partial class SynchronizeTests
    {
        public static class WithInheritance
        {
            [Test]
            public static void Updates()
            {
                var source = new With<BaseClass>();
                var target = new With<BaseClass>();
                using (Synchronize.PropertyValues(source, target, ReferenceHandling.Structural))
                {
                    source.Value = new Derived1 { BaseValue = 1, Derived1Value = 2 };
                    Assert.AreEqual(null, source.Name);
                    Assert.AreEqual(null, target.Name);
                    Assert.IsInstanceOf<Derived1>(source.Value);
                    Assert.IsInstanceOf<Derived1>(target.Value);
                    Assert.AreEqual(1, source.Value.BaseValue);
                    Assert.AreEqual(1, target.Value.BaseValue);

                    Assert.AreEqual(2, ((Derived1)source.Value).Derived1Value);
                    Assert.AreEqual(2, ((Derived1)target.Value).Derived1Value);

                    source.Value = new Derived2 { BaseValue = 1, Derived2Value = 2 };
                    Assert.AreEqual(null, source.Name);
                    Assert.AreEqual(null, target.Name);
                    Assert.IsInstanceOf<Derived2>(source.Value);
                    Assert.IsInstanceOf<Derived2>(target.Value);
                    Assert.AreEqual(1, source.Value.BaseValue);
                    Assert.AreEqual(1, target.Value.BaseValue);

                    Assert.AreEqual(2, ((Derived2)source.Value).Derived2Value);
                    Assert.AreEqual(2, ((Derived2)target.Value).Derived2Value);
                }
            }
        }
    }
}
