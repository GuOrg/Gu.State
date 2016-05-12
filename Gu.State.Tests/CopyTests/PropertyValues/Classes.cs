namespace Gu.State.Tests.CopyTests.PropertyValues
{
    using NUnit.Framework;

    using static CopyTypes;

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
            var source = new With<WithReadonlyProperty<int>>(new WithReadonlyProperty<int>(1));
            var target = new With<WithReadonlyProperty<int>>();
            var settings = PropertiesSettings.Build()
                                             .AddCustomCopy<WithReadonlyProperty<int>>((x, y) => new WithReadonlyProperty<int>(5))
                                             .CreateSettings();
            Copy.PropertyValues(source, target, settings);
            Assert.AreEqual(1, source.Value.Value);
            Assert.AreEqual(5, target.Value.Value);
        }
    }
}
