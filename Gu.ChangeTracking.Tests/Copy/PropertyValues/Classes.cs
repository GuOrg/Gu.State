namespace Gu.ChangeTracking.Tests.PropertyValues
{
    using Gu.ChangeTracking.Tests.CopyStubs;

    using NUnit.Framework;

    public class Classes : Tests.ClassesTests
    {
        public override void CopyMethod<T>(T source, T target)
        {
            Copy.PropertyValues(source, target);
        }

        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling)
        {
            Copy.PropertyValues(source, target, referenceHandling);
        }

        public override void CopyMethod<T>(T source, T target, params string[] excluded)
        {
            Copy.PropertyValues(source, target, excluded);
        }

        [Test]
        public void WithCalculatedPropertyHappyPath()
        {
            var source = new WithCalculatedProperty { Value = 1 };
            var target = new WithCalculatedProperty { Value = 3 };
            Copy.PropertyValues(source, target);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreEqual(1, source.CalculatedValue);
            Assert.AreEqual(1, target.CalculatedValue);
        }

        [Test]
        public void WithSpecialCopyPropertyWhenNull()
        {
            var source = new WithProperty<WithReadonlyProperty<int>>(new WithReadonlyProperty<int>(1));
            var target = new WithProperty<WithReadonlyProperty<int>>();
            var copyProperty = SpecialCopyProperty.CreateClone<WithComplexProperty, ComplexType>(
                x => x.ComplexType,
                () => new ComplexType());
            Assert.Inconclusive("Not sure we want to keep this mess");
            //Copy.PropertyValues(source, target, new[] { copyProperty });
            //Assert.AreEqual(source.Name, target.Name);
            //Assert.AreEqual(source.Value, target.Value);
            //Assert.IsNull(source.ComplexType);
            //Assert.IsNull(target.ComplexType);
        }
    }
}
