namespace Gu.State.Tests
{
    using System;

    using NUnit.Framework;

    using static SynchronizeTypes;

    public partial class SynchronizeTests
    {
        public class Simple
        {
            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void HappyPath(ReferenceHandling referenceHandling)
            {
                var source = new WithSimpleProperties
                {
                    IntValue = 1,
                    NullableIntValue = 2,
                    StringValue = "3",
                    EnumValue = StringSplitOptions.RemoveEmptyEntries
                };
                var target = new WithSimpleProperties { IntValue = 3, NullableIntValue = 4 };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    Assert.AreEqual(1, source.IntValue);
                    Assert.AreEqual(1, target.IntValue);
                    Assert.AreEqual(2, source.NullableIntValue);
                    Assert.AreEqual(2, target.NullableIntValue);
                    Assert.AreEqual("3", source.StringValue);
                    Assert.AreEqual("3", target.StringValue);
                    Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, source.EnumValue);
                    Assert.AreEqual(StringSplitOptions.RemoveEmptyEntries, target.EnumValue);

                    source.IntValue = 5;
                    Assert.AreEqual(5, source.IntValue);
                    Assert.AreEqual(5, target.IntValue);
                }

                source.IntValue = 6;
                Assert.AreEqual(6, source.IntValue);
                Assert.AreEqual(5, target.IntValue);
            }

            [Test]
            public void WithCalculated()
            {
                var source = new WithCalculatedProperty { Value = 1 };
                var target = new WithCalculatedProperty { Value = 3 };
                using (Synchronize.PropertyValues(source, target))
                {
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    source.Value = 5;
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);
                }

                source.Value = 6;
                Assert.AreEqual(6, source.Value);
                Assert.AreEqual(5, target.Value);
            }

            [Test]
            public void Excludes()
            {
                var source = new WithSimpleProperties { IntValue = 1, StringValue = "2" };
                var target = new WithSimpleProperties { IntValue = 3, StringValue = "4" };
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<WithSimpleProperties>(x => x.StringValue)
                                                 .CreateSettings();
                using (Synchronize.PropertyValues(source, target, settings))
                {
                    Assert.AreEqual(1, source.IntValue);
                    Assert.AreEqual(1, target.IntValue);
                    Assert.AreEqual("2", source.StringValue);
                    Assert.AreEqual("4", target.StringValue);

                    source.IntValue = 5;
                    Assert.AreEqual(5, source.IntValue);
                    Assert.AreEqual(5, target.IntValue);

                    source.StringValue = "7";
                    Assert.AreEqual("7", source.StringValue);
                    Assert.AreEqual("4", target.StringValue);
                }

                source.IntValue = 6;
                Assert.AreEqual(6, source.IntValue);
                Assert.AreEqual(5, target.IntValue);
                Assert.AreEqual("7", source.StringValue);
                Assert.AreEqual("4", target.StringValue);
            }

            [Test]
            public void HandlesMissingProperty()
            {
                var source = new WithSimpleProperties { IntValue = 1, StringValue = "2" };
                var target = new WithSimpleProperties { IntValue = 3, StringValue = "4" };
                using (Synchronize.PropertyValues(source, target))
                {
                    Assert.AreEqual(1, source.IntValue);
                    Assert.AreEqual(1, target.IntValue);
                    Assert.AreEqual("2", source.StringValue);
                    Assert.AreEqual("2", target.StringValue);
                    source.OnPropertyChanged("Missing");
                    source.IntValue = 5;
                    Assert.AreEqual(5, source.IntValue);
                    Assert.AreEqual(5, target.IntValue);
                    Assert.AreEqual("2", source.StringValue);
                    Assert.AreEqual("2", target.StringValue);
                }

                source.IntValue = 6;
                Assert.AreEqual(6, source.IntValue);
                Assert.AreEqual(5, target.IntValue);
                Assert.AreEqual("2", source.StringValue);
                Assert.AreEqual("2", target.StringValue);
            }

            [TestCase(null)]
            [TestCase("")]
            public void HandlesPropertyChangedEmptyAndNull(string prop)
            {
                var source = new WithSimpleProperties { IntValue = 1, StringValue = "2" };
                var target = new WithSimpleProperties { IntValue = 3, StringValue = "4" };
                using (Synchronize.PropertyValues(source, target))
                {
                    Assert.AreEqual(1, source.IntValue);
                    Assert.AreEqual(1, target.IntValue);
                    Assert.AreEqual("2", source.StringValue);
                    Assert.AreEqual("2", target.StringValue);

                    source.SetFields(5, "6");
                    source.OnPropertyChanged(prop);
                    Assert.AreEqual(5, source.IntValue);
                    Assert.AreEqual(5, target.IntValue);
                    Assert.AreEqual("6", source.StringValue);
                    Assert.AreEqual("6", target.StringValue);
                }
            }
        }
    }
}
