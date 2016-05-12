namespace Gu.State.Tests.CopyTests.PropertyValues
{
    using NUnit.Framework;

    public class Classes : ClassesTests
    {
        public override void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excluded = null)
        {
            var builder = PropertiesSettings.Build();
            if (excluded != null)
            {
                builder.IgnoreProperty<T>(excluded);
            }

            //if (excludedType != null)
            //{
            //    builder.IgnoreType(excludedType);
            //}

            var settings = builder.CreateSettings(referenceHandling);
            Copy.PropertyValues(source, target, settings);
        }

        [Test]
        public void WithCalculatedPropertyHappyPath()
        {
            var source = new CopyTypes.WithCalculatedProperty { Value = 1 };
            var target = new CopyTypes.WithCalculatedProperty { Value = 3 };
            Copy.PropertyValues(source, target);
            Assert.AreEqual(1, source.Value);
            Assert.AreEqual(1, target.Value);
            Assert.AreEqual(1, source.CalculatedValue);
            Assert.AreEqual(1, target.CalculatedValue);
        }

        [Test]
        public void WithSpecialCopyPropertyWhenNull()
        {
            Assert.Fail();
            //var source = new CopyTypes.WithProperty<CopyTypes.WithReadonlyProperty<int>>(new CopyTypes.WithReadonlyProperty<int>(1));
            //var target = new CopyTypes.WithProperty<CopyTypes.WithReadonlyProperty<int>>();
            //var copyProperty = SpecialCopyProperty.CreateClone<CopyTypes.WithComplexProperty, CopyTypes.ComplexType>(
            //    x => x.ComplexType,
            //    () => new CopyTypes.ComplexType());
            //Assert.Inconclusive("Not sure we want to keep this mess");
            //Copy.PropertyValues(source, target, new[] { copyProperty });
            //Assert.AreEqual(source.Name, target.Name);
            //Assert.AreEqual(source.Value, target.Value);
            //Assert.IsNull(source.ComplexType);
            //Assert.IsNull(target.ComplexType);
        }
    }
}
