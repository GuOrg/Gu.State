namespace Gu.ChangeTracking.Tests
{
    using Gu.ChangeTracking.Tests.PropertySynchronizerStubs;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class Complex
        {
            [Test]
            public void HappyPath()
            {
                var source = new WithComplexProperty("a", 1)
                {
                    ComplexType = new ComplexType("b", 2)
                };
                var target = new WithComplexProperty("c", 3)
                {
                    ComplexType = new ComplexType("d", 4)
                };

                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.Value = 5;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.ComplexType.Value = 6;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(6, source.ComplexType.Value);
                    Assert.AreEqual(6, target.ComplexType.Value);

                    source.ComplexType = null;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual(null, source.ComplexType);
                    Assert.AreEqual(null, target.ComplexType);

                    source.ComplexType = new ComplexType("f", 7);
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual("f", source.ComplexType.Name);
                    Assert.AreEqual("f", target.ComplexType.Name);
                    Assert.AreEqual(7, source.ComplexType.Value);
                    Assert.AreEqual(7, target.ComplexType.Value);

                    source.ComplexType.Value = 8;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("f", source.ComplexType.Name);
                    Assert.AreEqual("f", target.ComplexType.Name);
                    Assert.AreEqual(8, source.ComplexType.Value);
                    Assert.AreEqual(8, target.ComplexType.Value);
                }

                source.Value = 6;
                Assert.AreEqual("a", source.Name);
                Assert.AreEqual("a", target.Name);
                Assert.AreEqual(6, source.Value);
                Assert.AreEqual(5, target.Value);

                Assert.AreEqual("f", source.ComplexType.Name);
                Assert.AreEqual("f", target.ComplexType.Name);
                Assert.AreEqual(8, source.ComplexType.Value);
                Assert.AreEqual(8, target.ComplexType.Value);
            }

            [Test]
            public void Excludes()
            {
                var source = new WithComplexProperty("a", 1)
                {
                    ComplexType = new ComplexType("b", 2)
                };
                var target = new WithComplexProperty("c", 3)
                {
                    ComplexType = new ComplexType("d", 4)
                };
                var excluded = new[] { typeof(WithComplexProperty).GetProperty(nameof(WithComplexProperty.Name)) };
                var settings = new CopyPropertiesSettings(
                    excluded,
                    Constants.DefaultPropertyBindingFlags,
                    ReferenceHandling.Structural);
                using (PropertySynchronizer.Create(source, target, settings))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.Value = 5;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.Name = "ignored";
                    Assert.AreEqual("ignored", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.ComplexType.Value = 6;
                    Assert.AreEqual("ignored", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(6, source.ComplexType.Value);
                    Assert.AreEqual(6, target.ComplexType.Value);

                    source.ComplexType = null;
                    Assert.AreEqual("ignored", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual(null, source.ComplexType);
                    Assert.AreEqual(null, target.ComplexType);

                    source.ComplexType = new ComplexType("f", 7);
                    Assert.AreEqual("ignored", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreEqual("f", source.ComplexType.Name);
                    Assert.AreEqual("f", target.ComplexType.Name);
                    Assert.AreEqual(7, source.ComplexType.Value);
                    Assert.AreEqual(7, target.ComplexType.Value);

                    source.ComplexType.Value = 8;
                    Assert.AreEqual("ignored", source.Name);
                    Assert.AreEqual("c", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("f", source.ComplexType.Name);
                    Assert.AreEqual("f", target.ComplexType.Name);
                    Assert.AreEqual(8, source.ComplexType.Value);
                    Assert.AreEqual(8, target.ComplexType.Value);
                }

                source.Value = 6;
                Assert.AreEqual("ignored", source.Name);
                Assert.AreEqual("c", target.Name);
                Assert.AreEqual(6, source.Value);
                Assert.AreEqual(5, target.Value);

                Assert.AreEqual("f", source.ComplexType.Name);
                Assert.AreEqual("f", target.ComplexType.Name);
                Assert.AreEqual(8, source.ComplexType.Value);
                Assert.AreEqual(8, target.ComplexType.Value);
            }

            [Test]
            public void HandlesMissingProperty()
            {
                var source = new WithComplexProperty("a", 1)
                {
                    ComplexType = new ComplexType("b", 2)
                };
                var target = new WithComplexProperty("c", 3)
                {
                    ComplexType = new ComplexType("d", 4)
                };
                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    source.OnPropertyChanged("Missing");
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.Value = 5;
                    source.ComplexType.Value = 6;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(6, source.ComplexType.Value);
                    Assert.AreEqual(6, target.ComplexType.Value);
                }
            }

            [TestCase(null)]
            [TestCase("")]
            public void UpdatesAll(string prop)
            {
                var source = new WithComplexProperty("a", 1)
                {
                    ComplexType = new ComplexType("b", 2)
                };
                var target = new WithComplexProperty("c", 3)
                {
                    ComplexType = new ComplexType("d", 4)
                };

                using (PropertySynchronizer.Create(source, target, ReferenceHandling.Structural))
                {
                    source.SetFields("e", 5, new ComplexType("f", 6));
                    source.OnPropertyChanged(prop);
                    Assert.AreEqual("e", source.Name);
                    Assert.AreEqual("e", target.Name);
                    Assert.AreEqual(5, source.Value);
                    Assert.AreEqual(5, target.Value);


                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("f", source.ComplexType.Name);
                    Assert.AreEqual("f", target.ComplexType.Name);
                    Assert.AreEqual(6, source.ComplexType.Value);
                    Assert.AreEqual(6, target.ComplexType.Value);
                }
            }
        }
    }
}
