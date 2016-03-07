namespace Gu.State.Tests
{
    using System;

    using NUnit.Framework;

    public partial class PropertySynchronizerTests
    {
        public class Complex
        {
            [Test]
            public void CreateAndDisposeStructural()
            {
                var source = new SynchronizerTypes.WithComplexProperty("a", 1) { ComplexType = new SynchronizerTypes.ComplexType("b", 2) };
                var target = new SynchronizerTypes.WithComplexProperty("c", 3) { ComplexType = new SynchronizerTypes.ComplexType("d", 4) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
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

                    source.Value++;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(2, source.ComplexType.Value);
                    Assert.AreEqual(2, target.ComplexType.Value);

                    source.ComplexType.Value++;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("b", source.ComplexType.Name);
                    Assert.AreEqual("b", target.ComplexType.Name);
                    Assert.AreEqual(3, source.ComplexType.Value);
                    Assert.AreEqual(3, target.ComplexType.Value);

                    var sourceComplexType = source.ComplexType;
                    var targetComplexType = target.ComplexType;
                    source.ComplexType = null;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreEqual(null, source.ComplexType);
                    Assert.AreEqual(null, target.ComplexType);

                    sourceComplexType.Value++;

                    Assert.AreNotEqual(sourceComplexType.Value, targetComplexType.Value);

                    source.ComplexType = new SynchronizerTypes.ComplexType("c", 5);
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreNotSame(source.ComplexType, target.ComplexType);
                    Assert.AreEqual("c", source.ComplexType.Name);
                    Assert.AreEqual("c", target.ComplexType.Name);
                    Assert.AreEqual(5, source.ComplexType.Value);
                    Assert.AreEqual(5, target.ComplexType.Value);
                }

                source.Value++;
                Assert.AreEqual("a", source.Name);
                Assert.AreEqual("a", target.Name);
                Assert.AreEqual(3, source.Value);
                Assert.AreEqual(2, target.Value);

                Assert.AreNotSame(source.ComplexType, target.ComplexType);
                Assert.AreEqual("c", source.ComplexType.Name);
                Assert.AreEqual("c", target.ComplexType.Name);
                Assert.AreEqual(5, source.ComplexType.Value);
                Assert.AreEqual(5, target.ComplexType.Value);

                source.ComplexType.Value++;
                Assert.AreEqual("a", source.Name);
                Assert.AreEqual("a", target.Name);
                Assert.AreEqual(3, source.Value);
                Assert.AreEqual(2, target.Value);

                Assert.AreNotSame(source.ComplexType, target.ComplexType);
                Assert.AreEqual("c", source.ComplexType.Name);
                Assert.AreEqual("c", target.ComplexType.Name);
                Assert.AreEqual(6, source.ComplexType.Value);
                Assert.AreEqual(5, target.ComplexType.Value);
            }

            [Test]
            public void CreateAndDisposeReference()
            {
                var source = new SynchronizerTypes.WithComplexProperty("a", 1) { ComplexType = new SynchronizerTypes.ComplexType("b", 2) };
                var target = new SynchronizerTypes.WithComplexProperty("c", 3) { ComplexType = new SynchronizerTypes.ComplexType("d", 4) };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.References))
                {
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(1, source.Value);
                    Assert.AreEqual(1, target.Value);

                    Assert.AreSame(source.ComplexType, target.ComplexType);

                    source.Value++;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreSame(source.ComplexType, target.ComplexType);

                    source.ComplexType = null;
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreEqual(null, source.ComplexType);
                    Assert.AreEqual(null, target.ComplexType);

                    source.ComplexType = new SynchronizerTypes.ComplexType("c", 5);
                    Assert.AreEqual("a", source.Name);
                    Assert.AreEqual("a", target.Name);
                    Assert.AreEqual(2, source.Value);
                    Assert.AreEqual(2, target.Value);

                    Assert.AreSame(source.ComplexType, target.ComplexType);
                }
            }

            [Test]
            public void HappyPath()
            {
                var source = new SynchronizerTypes.WithComplexProperty("a", 1)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("b", 2)
                };
                var target = new SynchronizerTypes.WithComplexProperty("c", 3)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("d", 4)
                };

                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
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

                    source.ComplexType = new SynchronizerTypes.ComplexType("f", 7);
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
                var source = new SynchronizerTypes.WithComplexProperty("a", 1)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("b", 2)
                };
                var target = new SynchronizerTypes.WithComplexProperty("c", 3)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("d", 4)
                };
                var settings = PropertiesSettings.Build()
                                                 .IgnoreProperty<SynchronizerTypes.WithComplexProperty>(nameof(SynchronizerTypes.WithComplexProperty.Name))
                                                 .CreateSettings(ReferenceHandling.Structural);
                using (Synchronize.CreatePropertySynchronizer(source, target, settings))
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

                    source.ComplexType = new SynchronizerTypes.ComplexType("f", 7);
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
                var source = new SynchronizerTypes.WithComplexProperty("a", 1)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("b", 2)
                };
                var target = new SynchronizerTypes.WithComplexProperty("c", 3)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("d", 4)
                };
                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
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
                var source = new SynchronizerTypes.WithComplexProperty("a", 1)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("b", 2)
                };
                var target = new SynchronizerTypes.WithComplexProperty("c", 3)
                {
                    ComplexType = new SynchronizerTypes.ComplexType("d", 4)
                };

                using (Synchronize.CreatePropertySynchronizer(source, target, referenceHandling: ReferenceHandling.Structural))
                {
                    source.SetFields("e", 5, new SynchronizerTypes.ComplexType("f", 6));
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

            [Test]
            public void WithComplexPropertyThrowsWithoutReferenceHandling()
            {
                var expected = "Copy.PropertyValues(x, y) failed.\r\n" +
                               "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                               "Solve the problem by any of:\r\n" +
                               "* Make ComplexType immutable or use an immutable type.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                               "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but tracks reference loops.\r\n" +
                               "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                               "  - ReferenceHandling.References means that references are copied.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property WithComplexProperty.ComplexType.\r\n" +
                               "    - The type ComplexType.\r\n";
                var source = new SynchronizerTypes.WithComplexProperty();
                var target = new SynchronizerTypes.WithComplexProperty();
                var exception = Assert.Throws<NotSupportedException>(() => Synchronize.CreatePropertySynchronizer(source, target));

                Assert.AreEqual(expected, exception.Message);
            }
        }
    }
}
