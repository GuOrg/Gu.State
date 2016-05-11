// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.DiffTests
{
    using System;

    using NUnit.Framework;

    using static DiffTypes;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>(ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null);

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void WithSimpleProperties(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<WithSimpleProperties>(referenceHandling);
        }

        [Test]
        public void ComplexValueThrowsWithoutReferenceHandling()
        {
            var expected = this is DiffTests.FieldValues.Verify
                               ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                                 "The field WithProperty<ComplexType>.<Value>k__BackingField of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithProperty<ComplexType>> for WithProperty<ComplexType> or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The field WithProperty<ComplexType>.<Value>k__BackingField.\r\n" +
                                 "    - The type ComplexType.\r\n"

                               : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithProperty<ComplexType>.Value of type ComplexType is not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithProperty<ComplexType>> for WithProperty<ComplexType> or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The property WithProperty<ComplexType>.Value.\r\n" +
                                 "    - The type ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithProperty<ComplexType>>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<ComplexType>(ReferenceHandling.Throw));
            Assert.DoesNotThrow(() => this.VerifyMethod<WithProperty<ComplexType>>());
            Assert.DoesNotThrow(() => this.VerifyMethod<WithProperty<ComplexType>>(ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.VerifyMethod<WithProperty<ComplexType>>(ReferenceHandling.References));
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this is DiffTests.FieldValues.Verify
                               ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n"

                               : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithIndexerType>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ListOfWithIndexers()
        {
            var expected = this is DiffTests.FieldValues.Verify
                               ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                                 "The field WithListProperty<WithIndexerType>.<Items>k__BackingField of type List<WithIndexerType> is not supported.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithListProperty<WithIndexerType>> for WithListProperty<WithIndexerType> or use a type that does.\r\n" +
                                 "* Use a type that implements IEquatable<> instead of List<WithIndexerType>.\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The field WithListProperty<WithIndexerType>.<Items>k__BackingField.\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n" +
                                 "    - The type List<WithIndexerType>.\r\n" +
                                 "    - The type WithIndexerType.\r\n"

                               : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithListProperty<WithIndexerType>.Items of type List<WithIndexerType> is not supported.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIndexerType.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Implement IEquatable<WithListProperty<WithIndexerType>> for WithListProperty<WithIndexerType> or use a type that does.\r\n" +
                                 "* Use a type that implements IEquatable<> instead of List<WithIndexerType>.\r\n" +
                                 "* Implement IEquatable<WithIndexerType> for WithIndexerType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The property WithListProperty<WithIndexerType>.Items.\r\n" +
                                 "    - The indexer property WithIndexerType.Item.\r\n" +
                                 "    - The type List<WithIndexerType>.\r\n" +
                                 "    - The type WithIndexerType.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithListProperty<WithIndexerType>>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ReferenceLoop()
        {
            Assert.DoesNotThrow(() => this.VerifyMethod<Parent>(ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.VerifyMethod<Parent>(ReferenceHandling.References));
        }
    }
}