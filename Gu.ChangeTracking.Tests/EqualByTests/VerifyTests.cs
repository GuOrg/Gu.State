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
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithComplexProperty.\r\n" +
                                 "  - Exclude the field WithComplexProperty.<ComplexType>k__BackingField.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
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
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item of type int is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n" +
                                 "  - Exclude the property WithIndexerType.Item.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithIndexerType>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}