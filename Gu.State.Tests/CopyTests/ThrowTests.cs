// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.CopyTests
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using static CopyTypes;

    public abstract class ThrowTests
    {
        public abstract void CopyMethod<T>(T source, T target, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excluded = null) where T : class;

        [TestCase(ReferenceHandling.Throw)]
        public void WithComplexThrows(ReferenceHandling? referenceHandling)
        {
            var expected = this is FieldValues.Throws
                   ? "Copy.FieldValues(x, y) failed.\r\n" +
                     "The field WithComplexProperty.<ComplexType>k__BackingField of type ComplexType is not supported.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Make ComplexType immutable or use an immutable type.\r\n" +
                     "  - For immutable types the following must hold:\r\n" +
                     "    - Must be a sealed class or a struct.\r\n" +
                     "    - All fields and properties must be readonly.\r\n" +
                     "    - All field and property types must be immutable.\r\n" +
                     "    - All indexers must be readonly.\r\n" +
                     "    - Event fields are ignored.\r\n" +
                     "* Use FieldsSettings and specify how copying is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                     "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                     "  - ReferenceHandling.References means that references are copied.\r\n" +
                     "  - Exclude a combination of the following:\r\n" +
                     "    - The field WithComplexProperty.<ComplexType>k__BackingField.\r\n" +
                     "    - The type ComplexType.\r\n"

                   : "Copy.PropertyValues(x, y) failed.\r\n" +
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
                     "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                     "  - ReferenceHandling.References means that references are copied.\r\n" +
                     "  - Exclude a combination of the following:\r\n" +
                     "    - The property WithComplexProperty.ComplexType.\r\n" +
                     "    - The type ComplexType.\r\n";
            var source = new WithComplexProperty();
            var target = new WithComplexProperty();

            var exception = referenceHandling == null
                ? Assert.Throws<NotSupportedException>(() => this.CopyMethod(source, target))
                : Assert.Throws<NotSupportedException>(() => this.CopyMethod(source, target, referenceHandling.Value));
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void WithReadonlyImmutableThrows(ReferenceHandling referenceHandling)
        {
            var expected = this is FieldValues.Throws
                ? "Copy.FieldValues(x, y) failed.\r\n" +
                  "The readonly field WithReadonlyProperty<Immutable>.<Value>k__BackingField differs after copy.\r\n" +
                  " - Source value (Immutable): Gu.State.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                  " - Target value (Immutable): Gu.State.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                  "Solve the problem by any of:\r\n" +
                  "* Use FieldsSettings and specify how copying is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                  "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                  "  - ReferenceHandling.References means that references are copied.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The field WithReadonlyProperty<Immutable>.<Value>k__BackingField.\r\n"

                  : "Copy.PropertyValues(x, y) failed.\r\n" +
                    "The readonly property WithReadonlyProperty<Immutable>.Value differs after copy.\r\n" +
                    " - Source value (Immutable): Gu.State.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                    " - Target value (Immutable): Gu.State.Tests.CopyTests.CopyTypes+Immutable.\r\n" +
                    "Solve the problem by any of:\r\n" +
                    "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                    "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                    "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                    "  - ReferenceHandling.References means that references are copied.\r\n" +
                    "  - Exclude a combination of the following:\r\n" +
                    "    - The property WithReadonlyProperty<Immutable>.Value.\r\n";

            var source = new WithReadonlyProperty<Immutable>(new Immutable(1));
            var target = new WithReadonlyProperty<Immutable>(new Immutable(2));

            var exception = Assert.Throws<InvalidOperationException>(() => this.CopyMethod(source, target, referenceHandling));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ArrayOfIntsDifferentLengthsThrows()
        {
            var expected = this is FieldValues.Throws
                               ? "Copy.FieldValues(x, y) failed.\r\n" +
                                 "The collections are fixed size type: int[]\r\n" +
                                 "  - Source count: 3\r\n" +
                                 "  - Target count: 1\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use a resizable collection like List<int> instead of int[].\r\n" +
                                 "* Check that the collections are the same size before calling.\r\n" +
                                 "* Use FieldsSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                                 "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n"

                               : "Copy.PropertyValues(x, y) failed.\r\n" +
                                 "The collections are fixed size type: int[]\r\n" +
                                 "  - Source count: 3\r\n" +
                                 "  - Target count: 1\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use a resizable collection like List<int> instead of int[].\r\n" +
                                 "* Check that the collections are the same size before calling.\r\n" +
                                 "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                                 "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n";
            var source = new[] { 1, 2, 3 };
            var target = new[] { 4 };

            var exception = Assert.Throws<InvalidOperationException>(() => this.CopyMethod(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this is FieldValues.Throws
                               ? "Copy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use FieldsSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                                 "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n"

                               : "Copy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                                 "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n";
            var source = new WithIndexerType();
            var target = new WithIndexerType();

            var exception = Assert.Throws<NotSupportedException>(() => this.CopyMethod(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WhenDefaultConstructorIsMissing()
        {
            var expected = this is FieldValues.Throws
                   ? "Copy.FieldValues(x, y) failed.\r\n" +
                     "Activator.CreateInstance failed for type WithoutDefaultCtor.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Add a parameterless constructor to WithoutDefaultCtor, can be private.\r\n" +
                     "* Use FieldsSettings and specify how copying is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                     "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                     "  - ReferenceHandling.References means that references are copied.\r\n" 

                   : "Copy.PropertyValues(x, y) failed.\r\n" +
                     "Activator.CreateInstance failed for type WithoutDefaultCtor.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Add a parameterless constructor to WithoutDefaultCtor, can be private.\r\n" +
                     "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                     "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                     "  - ReferenceHandling.References means that references are copied.\r\n";

            var x = new With<WithoutDefaultCtor>(new WithoutDefaultCtor(1));
            var y = new With<WithoutDefaultCtor>(null);

            var exception = Assert.Throws<InvalidOperationException>(() => this.CopyMethod(x, y, ReferenceHandling.Structural));

            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyEnumerableOfIntsThrows(ReferenceHandling? referenceHandling)
        {
            var expected = "Can only copy the members of collections implementing IList or IDictionary";

            var x = Enumerable.Repeat(1, 2);
            var y = Enumerable.Repeat(1, 2);
            var exception = referenceHandling != null
                ? Assert.Throws<NotSupportedException>(() => this.CopyMethod(x, y, referenceHandling.Value))
                : Assert.Throws<NotSupportedException>(() => this.CopyMethod(x, y));
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyImmutable(ReferenceHandling? referenceHandling)
        {
            var expected = "Cannot copy the members of an immutable object";
            var x = new Immutable(1);
            var y = new Immutable(1);
            var exception = referenceHandling != null
                ? Assert.Throws<NotSupportedException>(() => this.CopyMethod(x, y, referenceHandling.Value))
                : Assert.Throws<NotSupportedException>(() => this.CopyMethod(x, y));

            Assert.AreEqual(expected, exception.Message);
        }
    }
}