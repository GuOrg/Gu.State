namespace Gu.ChangeTracking.Tests.CopyTests
{
    using System;

    using NUnit.Framework;

    public abstract class ThrowTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excluded = null) where T : class;

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        public void WithComplexWithoutReferenceHandling(ReferenceHandling? referenceHandling)
        {
            var expected = this.GetType() == typeof (FieldValues.Throws)
                ? "Copy.FieldValues(x, y) failed.\r\n" +
                  "The field WithComplexProperty.<ComplexType>k__BackingField is not supported.\r\n" +
                  "The field is of type ComplexType.\r\n" +
                  "Solve the problem by any of:\r\n" +
                  "* Make ComplexType immutable or use an immutable type. For immutable types the following must hold:\r\n" +
                  "  - Must be a sealed class or a struct.\r\n" +
                  "  - All fields and properties must be readonly.\r\n" +
                  "  - All field and property types must be immutable.\r\n" +
                  "  - All indexers must be readonly.\r\n" +
                  "  - Event fields are ignored.\r\n" +
                  "* Use FieldsSettings and specify how copying is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                  "  - ReferenceHandling.References means that references are copied.\r\n" +
                  "  - Exclude the type WithComplexProperty.\r\n" +
                  "  - Exclude the field WithComplexProperty.<ComplexType>k__BackingField.\r\n"

                : "Copy.PropertyValues(x, y) failed.\r\n" +
                  "The property WithComplexProperty.ComplexType is not supported.\r\n" +
                  "The property is of type ComplexType.\r\n" +
                  "Solve the problem by any of:\r\n" +
                  "* Make ComplexType immutable or use an immutable type. For immutable types the following must hold:\r\n" +
                  "  - Must be a sealed class or a struct.\r\n" +
                  "  - All fields and properties must be readonly.\r\n" +
                  "  - All field and property types must be immutable.\r\n" +
                  "  - All indexers must be readonly.\r\n" +
                  "  - Event fields are ignored.\r\n" +
                  "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                  "  - ReferenceHandling.References means that references are copied.\r\n" +
                  "  - Exclude the type WithComplexProperty.\r\n" +
                  "  - Exclude the property WithComplexProperty.ComplexType.\r\n";
            var source = new CopyTypes.WithComplexProperty();
            var target = new CopyTypes.WithComplexProperty();

            if (referenceHandling == null)
            {
                var exception = Assert.Throws<NotSupportedException>(() => this.CopyMethod(source, target));
                Assert.AreEqual(expected, exception.Message);
            }
            else
            {
                var exception = Assert.Throws<NotSupportedException>(() => this.CopyMethod(source, target, ReferenceHandling.Throw));
                Assert.AreEqual(expected, exception.Message);
            }
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithReadonlyImmutableThrows(ReferenceHandling referenceHandling)
        {
            var expected = this.GetType() == typeof (FieldValues.Throws)
                ? "Copy.FieldValues(x, y) failed.\r\n" +
                  "The readonly field WithReadonlyProperty<Immutable>.<Value>k__BackingField differs after copy.\r\n" +
                  " - Source value: Gu.ChangeTracking.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                  " - Target value: Gu.ChangeTracking.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                  "The field is of type Immutable.\r\n" +
                  "Solve the problem by any of:\r\n" +
                  "* Use FieldsSettings and specify how copying is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                  "  - ReferenceHandling.References means that references are copied.\r\n" +
                  "  - Exclude the type WithReadonlyProperty<Immutable>.\r\n" +
                  "  - Exclude the field WithReadonlyProperty<Immutable>.<Value>k__BackingField.\r\n"
          
                  : "Copy.PropertyValues(x, y) failed.\r\n" +
                  "The readonly property WithReadonlyProperty<Immutable>.Value differs after copy.\r\n" +
                  " - Source value: Gu.ChangeTracking.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                  " - Target value: Gu.ChangeTracking.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                  "The property is of type Immutable.\r\n" +
                  "Solve the problem by any of:\r\n" +
                  "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                  "  - ReferenceHandling.References means that references are copied.\r\n" +
                  "  - Exclude the type WithReadonlyProperty<Immutable>.\r\n" +
                  "  - Exclude the property WithReadonlyProperty<Immutable>.Value.\r\n";

            var source = new CopyTypes.WithReadonlyProperty<CopyTypes.Immutable>(new CopyTypes.Immutable(1));
            var target = new CopyTypes.WithReadonlyProperty<CopyTypes.Immutable>(new CopyTypes.Immutable(2));

            var exception = Assert.Throws<InvalidOperationException>(() => this.CopyMethod(source, target, referenceHandling));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ArrayOfIntsDifferentLengthsThrows()
        {
            var expected = this.GetType() == typeof(FieldValues.Throws)
                               ? "Copy.FieldValues(x, y) failed.\r\n" +
                                 "The collections are fixed size type: Int32[]\r\n" +
                                 "Source count: 3\r\n" +
                                 "Target count: 1\r\n" + 
                                 "Solve the problem by any of:\r\n" +
                                 "* Use a resizable collection like List<T>.\r\n" +
                                 "* Check that the collections are the same size before calling.\r\n"

                               : "Copy.PropertyValues(x, y) failed.\r\n" +
                                 "The collections are fixed size type: Int32[]\r\n" +
                                 "Source count: 3\r\n" +
                                 "Target count: 1\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use a resizable collection like List<T>.\r\n" +
                                 "* Check that the collections are the same size before calling.\r\n";
            var source = new[] { 1, 2, 3 };
            var target = new[] { 4 };

            var exception = Assert.Throws<InvalidOperationException>(() => this.CopyMethod(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this.GetType() == typeof(FieldValues.Throws)
                               ? "Copy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item is not supported.\r\n" +
                                 "The property is of type int.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use FieldsSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n"

                               : "Copy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item is not supported.\r\n" +
                                 "The property is of type int.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n" +
                                 "  - Exclude the property WithIndexerType.Item.\r\n";
            var source = new CopyTypes.WithIndexerType();
            var target = new CopyTypes.WithIndexerType();

            var exception = Assert.Throws<NotSupportedException>(() => this.CopyMethod(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenDefaultConstructorIsMissing()
        {
            var expected = this.GetType() == typeof(FieldValues.Throws)
                   ? "Copy.FieldValues(x, y) failed.\r\n" +
                     "Activator.CreateInstance failed for type WithoutDefaultCtor.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Add a parameterless constructor to WithoutDefaultCtor, can be private.\r\n" +
                     "* Use FieldsSettings and specify how copying is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                     "  - ReferenceHandling.References means that references are copied.\r\n" +
                     "  - Exclude the type WithoutDefaultCtor.\r\n" +
                     "  - Exclude the field WithoutDefaultCtor.<Value>k__BackingField.\r\n"

                   : "Copy.PropertyValues(x, y) failed.\r\n" +
                     "Activator.CreateInstance failed for type WithoutDefaultCtor.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Add a parameterless constructor to WithoutDefaultCtor, can be private.\r\n" +
                     "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep copy is performed.\r\n" +
                     "  - ReferenceHandling.References means that references are copied.\r\n" +
                     "  - Exclude the type WithoutDefaultCtor.\r\n" +
                     "  - Exclude the property WithoutDefaultCtor.Value.\r\n";

            var x = new CopyTypes.WithProperty<CopyTypes.WithoutDefaultCtor>(new CopyTypes.WithoutDefaultCtor(1));
            var y = new CopyTypes.WithProperty<CopyTypes.WithoutDefaultCtor>(null);

            var exception = Assert.Throws<NotSupportedException>(() => this.CopyMethod(x, y, referenceHandling: ReferenceHandling.Structural));

            Assert.AreEqual(expected, exception.Message);
        }
    }
}