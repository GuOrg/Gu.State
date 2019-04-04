// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests
{
    using System;

    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class ThrowsTests
    {
        public abstract bool EqualBy<T>(T x, T y, ReferenceHandling referenceHandling = ReferenceHandling.Structural, string excludedMembers = null, Type excludedType = null);

        [Test]
        public void WithComplexPropertyThrowsWhenReferenceHandlingThrow()
        {
            var expected = this is FieldValues.Throws
                ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                  "The field WithComplexProperty.<ComplexType>k__BackingField of type ComplexType is not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                  "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The field WithComplexProperty.<ComplexType>k__BackingField.\r\n" +
                  "    - The type ComplexType.\r\n"

                : "EqualBy.PropertyValues(x, y) failed.\r\n" +
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
            var x = new WithComplexProperty { ComplexType = new ComplexType() };
            var y = new WithComplexProperty { ComplexType = new ComplexType() };
            var exception = Assert.Throws<NotSupportedException>(() => this.EqualBy(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.EqualBy(new ComplexType(), new ComplexType()));
            Assert.DoesNotThrow(() => this.EqualBy(x, y));
            Assert.DoesNotThrow(() => this.EqualBy(x, y, ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.EqualBy(x, y, ReferenceHandling.References));
        }

        [Test]
        public void WithIllegalIndexer()
        {
            var expected = this is FieldValues.Throws
                ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                  "Indexers are not supported.\r\n" +
                  "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The indexer property WithIllegalIndexer[int].\r\n"

                : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                  "Indexers are not supported.\r\n" +
                  "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                  "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The indexer property WithIllegalIndexer[int].\r\n";
            var source = new WithIllegalIndexer();
            var target = new WithIllegalIndexer();

            var exception = Assert.Throws<NotSupportedException>(() => this.EqualBy(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIllegalIndexerProperty()
        {
            var expected = this is FieldValues.Throws
                ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                  "The field With<WithIllegalIndexer>.<Value>k__BackingField of type WithIllegalIndexer is not supported.\r\n" +
                  "Indexers are not supported.\r\n" +
                  "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<With<WithIllegalIndexer>> for With<WithIllegalIndexer> or use a type that does.\r\n" +
                  "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The field With<WithIllegalIndexer>.<Value>k__BackingField.\r\n" +
                  "    - The indexer property WithIllegalIndexer[int].\r\n" +
                  "    - The type WithIllegalIndexer.\r\n"

                : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                  "The property With<WithIllegalIndexer>.Value of type WithIllegalIndexer is not supported.\r\n" +
                  "Indexers are not supported.\r\n" +
                  "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<With<WithIllegalIndexer>> for With<WithIllegalIndexer> or use a type that does.\r\n" +
                  "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                  "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The property With<WithIllegalIndexer>.Value.\r\n" +
                  "    - The indexer property WithIllegalIndexer[int].\r\n" +
                  "    - The type WithIllegalIndexer.\r\n";

            var source = new With<WithIllegalIndexer>(new WithIllegalIndexer());
            var target = new With<WithIllegalIndexer>(new WithIllegalIndexer());

            var exception = Assert.Throws<NotSupportedException>(() => this.EqualBy(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void ReferenceLoop()
        {
            var expected = this is FieldValues.Throws
                ? "EqualBy.FieldValues(x, y) failed.\r\n" +
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

                : "EqualBy.PropertyValues(x, y) failed.\r\n" +
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
            var exception = Assert.Throws<NotSupportedException>(() => this.EqualBy(x, y, ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.AreEqual(true, this.EqualBy(x, y, ReferenceHandling.Structural));
            Assert.AreEqual(false, this.EqualBy(x, y, ReferenceHandling.References));
        }
    }
}