// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests
{
    using System;
    using System.Collections.ObjectModel;

    using NUnit.Framework;

    using static SynchronizeTypes;

    public partial class SynchronizeTests
    {
        public class Throws
        {
            [Test]
            public void WithComplexPropertyThrows()
            {
                var expected = "Synchronize.PropertyValues(x, y) failed.\r\n" +
                               "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                               "Below are a couple of suggestions that may solve the problem:\r\n" +
                               "* Make ComplexType immutable or use an immutable type.\r\n" +
                               "  - For immutable types the following must hold:\r\n" +
                               "    - Must be a sealed class or a struct.\r\n" +
                               "    - All fields and properties must be readonly.\r\n" +
                               "    - All field and property types must be immutable.\r\n" +
                               "    - All indexers must be readonly.\r\n" +
                               "    - Event fields are ignored.\r\n" +
                               "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                               "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                               "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                               "  - ReferenceHandling.References means that references are copied.\r\n" +
                               "  - Exclude a combination of the following:\r\n" +
                               "    - The property WithComplexProperty.ComplexType.\r\n" +
                               "    - The type ComplexType.\r\n";
                var source = new WithComplexProperty();
                var target = new WithComplexProperty();
                var exception = Assert.Throws<NotSupportedException>(() => Synchronize.PropertyValues(source, target, ReferenceHandling.Throw));

                Assert.AreEqual(expected, exception.Message);

                Assert.DoesNotThrow(() => Synchronize.PropertyValues(source, target, ReferenceHandling.Structural));
                Assert.DoesNotThrow(() => Synchronize.PropertyValues(source, target, ReferenceHandling.References));
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void ThrowsIfTargetChanges(ReferenceHandling referenceHandling)
            {
                var source = new WithSimpleProperties();
                var target = new WithSimpleProperties();
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => target.IntValue++);
                    var expected = "Target cannot be modified when a synchronizer is applied to it\r\n" +
                                   "The change would just trigger a dirty notification and the value would be updated with the value from source.";
                    Assert.AreEqual(expected, exception.Message);
                }
            }

            [TestCase(ReferenceHandling.Throw)]
            [TestCase(ReferenceHandling.Structural)]
            [TestCase(ReferenceHandling.References)]
            public void ThrowsIfTargetCollectionChanges(ReferenceHandling referenceHandling)
            {
                var source = new ObservableCollection<int> { 1, 2 };
                var target = new ObservableCollection<int> { 1, 2 };
                using (Synchronize.PropertyValues(source, target, referenceHandling))
                {
                    var exception = Assert.Throws<InvalidOperationException>(() => target.Add(3));
                    var expected = "Target cannot be modified when a synchronizer is applied to it\r\n" +
                                   "The change would just trigger a dirty notification and the value would be updated with the value from source.";
                    Assert.AreEqual(expected, exception.Message);
                }
            }
        }
    }
}