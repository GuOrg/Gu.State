namespace Gu.ChangeTracking.Tests.EqualByTests
{
    using System;

    using NUnit.Framework;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Throw, string excludedMembers = null, Type excludedType = null);

        [Test]
        public void ComplexValueThrowsWithoutReferenceHandling()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "The field WithComplexProperty.<ComplexType>k__BackingField of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The field WithComplexProperty.<ComplexType>k__BackingField.\r\n" +
                                 "    - The type ComplexType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The property WithComplexProperty.ComplexType.\r\n" +
                                 "    - The type ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithComplexProperty>());
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithIndexerType>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }


        [Test]
        public void ListOfWithIndexers()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "The field WithListProperty<WithIndexerType>.<Items>k__BackingField of type List<WithIndexerType> is not supported.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithListProperty<WithIndexerType>> for WithListProperty<WithIndexerType> or use a type that does.\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The field WithListProperty<WithIndexerType>.<Items>k__BackingField.\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n" +
                                 "    - The type List<WithIndexerType>.\r\n" +
                                 "    - The type WithIndexerType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithListProperty<WithIndexerType>.Items of type List<WithIndexerType> is not supported.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithListProperty<WithIndexerType>> for WithListProperty<WithIndexerType> or use a type that does.\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The property WithListProperty<WithIndexerType>.Items.\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n" +
                                 "    - The type List<WithIndexerType>.\r\n" +
                                 "    - The type WithIndexerType.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithListProperty<EqualByTypes.WithIndexerType>>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void DetectsReferenceLoop()
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                   ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                     "The field Parent.<Child>k__BackingField of type Child is not supported.\r\n" +
                     "The field Child.<Parent>k__BackingField of type Parent is not supported.\r\n" +
                     "The field Parent.<Child>k__BackingField of type Child is in a reference loop.\r\n" +
                     "  - The loop is Parent.<Child>k__BackingField.<Parent>k__BackingField.<Child>k__BackingField...\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Implement IEquatable<Parent> for Parent or use a type that does.\r\n" +
                     "* Implement IEquatable<Child> for Child or use a type that does.\r\n" +
                     "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                     "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                     "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                     "  - Exclude a combination of the following:\r\n" +
                     "    - The field Parent.<Child>k__BackingField.\r\n" +
                     "    - The field Child.<Parent>k__BackingField.\r\n" +
                     "    - The type Child.\r\n" 

                   : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                     "The property Parent.Child of type Child is not supported.\r\n" +
                     "The property Child.Parent of type Parent is not supported.\r\n" +
                     "The property Parent.Child of type Child is in a reference loop.\r\n" +
                     "  - The loop is Parent.Child.Parent.Child...\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Implement IEquatable<Parent> for Parent or use a type that does.\r\n" +
                     "* Implement IEquatable<Child> for Child or use a type that does.\r\n" +
                     "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                     "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but handles reference loops.\r\n" +
                     "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                     "  - Exclude a combination of the following:\r\n" +
                     "    - The property Parent.Child.\r\n" +
                     "    - The property Child.Parent.\r\n" +
                     "    - The type Child.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.Parent>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<EqualByTypes.Parent>(ReferenceHandling.StructuralWithReferenceLoops));
            Assert.DoesNotThrow(() => this.VerifyMethod<EqualByTypes.Parent>(ReferenceHandling.References));
        }
    }
}