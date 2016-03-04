namespace Gu.ChangeTracking.Tests.EqualByTests
{
    using System;

    using NUnit.Framework;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null);

        [Test]
        public void DetectesReferenceLoop()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                   ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                     "The field Parent.<Child>k__BackingField of type ComplexType the start of a reference loop.\r\n" +
                     "The loop is Parent.<Child>k__BackingField.<Parent>k__BackingField.<Child>k__BackingField...\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Implement IEquatable<Parent> for Parent or use a type that does.\r\n" +
                     "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                     "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                     "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                     "  - Exclude the type Parent.\r\n" +
                     "  - Exclude the type Child.\r\n" +
                     "  - Exclude the field Parent.<Child>k__BackingField.\r\n" +
                     "  - Exclude the field Child.<Parent>k__BackingField.\r\n"

                   : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                     "The property Parent.Child of type Child is the start of a reference loop.\r\n" +
                     "The loop is Parent.Child.Parent.Child...\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Implement IEquatable<Parent> for Parent or use a type that does.\r\n" +
                     "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                     "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                     "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                     "  - Exclude the type Parent.\r\n" +
                     "  - Exclude the type Child.\r\n" +
                     "  - Exclude the property Parent.Child.\r\n" +
                     "  - Exclude the property Child.Parent.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.Parent>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<EqualByTypes.Parent>(ReferenceHandling.StructuralWithReferenceLoops));
            Assert.DoesNotThrow(() => this.VerifyMethod<EqualByTypes.Parent>(ReferenceHandling.References));
        }

        [Test]
        public void ComplexValueThrowsWithoutReferenceHandling()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "The field WithComplexProperty.<ComplexType>k__BackingField of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithComplexProperty.\r\n" +
                                 "  - Exclude the field WithComplexProperty.<ComplexType>k__BackingField.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithComplexProperty.\r\n" +
                                 "  - Exclude the property WithComplexProperty.ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithComplexProperty>());
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item of type int is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item of type int is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n" +
                                 "  - Exclude the property WithIndexerType.Item.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithIndexerType>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}