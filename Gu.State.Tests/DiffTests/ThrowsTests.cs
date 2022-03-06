// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.DiffTests
{
    using System;

    using NUnit.Framework;

    using static DiffTypes;

    public abstract class ThrowsTests
    {
        public abstract Diff DiffBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null);

        [Test]
        public void ComplexValueThrowsWithoutReferenceHandling()
        {
            var expected = this is FieldValues.Throws
                               ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                                 "The field WithComplexProperty.complexType of type ComplexType is not supported.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The field WithComplexProperty.complexType.\r\n" +
                                 "    - The type ComplexType.\r\n"

                               : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property WithComplexProperty.ComplexType of type ComplexType is not supported.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The property WithComplexProperty.ComplexType.\r\n" +
                                 "    - The type ComplexType.\r\n";
            var x = new WithComplexProperty();
            var y = new WithComplexProperty();
            var exception = Assert.Throws<NotSupportedException>(() => this.DiffBy(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.DiffBy(x, y, ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.DiffBy(x, y, ReferenceHandling.References));
            Assert.DoesNotThrow(() => this.DiffBy(new ComplexType(), new ComplexType()));
        }

        [Test]
        public void WithIllegalIndexer()
        {
            var expected = this is FieldValues.Throws
                               ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIllegalIndexer[int].\r\n"

                               : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIllegalIndexer[int].\r\n";
            var source = new WithIllegalIndexer();
            var target = new WithIllegalIndexer();

            var exception = Assert.Throws<NotSupportedException>(() => this.DiffBy(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIllegalIndexerProperty()
        {
            var expected = this is FieldValues.Throws
                ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                  "The field WithProperty<WithIllegalIndexer>.<Value>k__BackingField of type WithIllegalIndexer is not supported.\r\n" +
                  "Indexers are not supported.\r\n" +
                  "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<WithProperty<WithIllegalIndexer>> for WithProperty<WithIllegalIndexer> or use a type that does.\r\n" +
                  "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The field WithProperty<WithIllegalIndexer>.<Value>k__BackingField.\r\n" +
                  "    - The indexer property WithIllegalIndexer[int].\r\n" +
                  "    - The type WithIllegalIndexer.\r\n"

                : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                  "The property WithProperty<WithIllegalIndexer>.Value of type WithIllegalIndexer is not supported.\r\n" +
                  "Indexers are not supported.\r\n" +
                  "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<WithProperty<WithIllegalIndexer>> for WithProperty<WithIllegalIndexer> or use a type that does.\r\n" +
                  "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                  "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The property WithProperty<WithIllegalIndexer>.Value.\r\n" +
                  "    - The indexer property WithIllegalIndexer[int].\r\n" +
                  "    - The type WithIllegalIndexer.\r\n";

            var source = new WithProperty<WithIllegalIndexer>();
            var target = new WithProperty<WithIllegalIndexer>();

            var exception = Assert.Throws<NotSupportedException>(() => this.DiffBy(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ReferenceLoop()
        {
            var expected = this is FieldValues.Throws
                ? "DiffBy.FieldValues(x, y) failed.\r\n" +
                  "The field Parent.<Child>k__BackingField of type Child is not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<Parent> for Parent or use a type that does.\r\n" +
                  "* Implement IEquatable<Child> for Child or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The field Parent.<Child>k__BackingField.\r\n" +
                  "    - The type Child.\r\n"

                : "DiffBy.PropertyValues(x, y) failed.\r\n" +
                  "The property Parent.Child of type Child is not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<Parent> for Parent or use a type that does.\r\n" +
                  "* Implement IEquatable<Child> for Child or use a type that does.\r\n" +
                  "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The property Parent.Child.\r\n" +
                  "    - The type Child.\r\n";

            var x = new Parent("p", new Child("c"));
            var y = new Parent("p", new Child("c"));
            var exception = Assert.Throws<NotSupportedException>(() => this.DiffBy(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.AreEqual("Empty", this.DiffBy(x, y, ReferenceHandling.Structural).ToString());
            expected = this is FieldValues.Throws
                           ? "Parent <Child>k__BackingField x: Gu.State.Tests.DiffTests.DiffTypes+Child y: Gu.State.Tests.DiffTests.DiffTypes+Child"
                           : "Parent Child x: Gu.State.Tests.DiffTests.DiffTypes+Child y: Gu.State.Tests.DiffTests.DiffTypes+Child";
            Assert.AreEqual(expected, this.DiffBy(x, y, ReferenceHandling.References).ToString(string.Empty, " "));

            Assert.DoesNotThrow(() => this.DiffBy(x, y, ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.DiffBy(x, y, ReferenceHandling.References));
        }
    }
}
