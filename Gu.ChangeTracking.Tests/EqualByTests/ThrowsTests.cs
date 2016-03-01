namespace Gu.ChangeTracking.Tests.EqualByTests
{
    using System;

    using NUnit.Framework;

    public abstract class ThrowsTests
    {
        public abstract bool EqualByMethod<T>(T x, T y);

        public abstract bool EqualByMethod<T>(T x, T y, ReferenceHandling referenceHandling);

        [Test]
        public void ComplexValueThrowsWithoutReferenceHandling()
        {
            var expected = this.GetType() == typeof(FieldValues.Throws)
            ? "EqualBy.FieldValues(x, y) failed.\r\n" +
            "The field WithComplexProperty.<ComplexType>k__BackingField is not supported.\r\n" +
            "The field is of type ComplexType.\r\n" +
            "Solve the problem by any of:\r\n" +
            "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
            "* Use EqualByFieldsSettings and specify how comparing is performed:\r\n" +
            "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
            "  - ReferenceHandling.References means that reference equality is used.\r\n" +
            "  - Exclude the type WithComplexProperty.\r\n" +
            "  - Exclude the field WithComplexProperty.<ComplexType>k__BackingField.\r\n"

            : "EqualBy.PropertyValues(x, y) failed.\r\n" +
            "The property WithComplexProperty.ComplexType is not supported.\r\n" +
            "The property is of type ComplexType.\r\n" +
            "Solve the problem by any of:\r\n" +
            "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
            "* Use EqualByFieldsSettings and specify how comparing is performed:\r\n" +
            "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
            "  - ReferenceHandling.References means that reference equality is used.\r\n" +
            "  - Exclude the type WithComplexProperty.\r\n" +
            "  - Exclude the property WithComplexProperty.ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.EqualByMethod<EqualByTypes.WithComplexProperty>(null, null));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this.GetType() == typeof(FieldValues.Throws)
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item is not supported.\r\n" +
                                 "The property is of type int.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use IEqualByFieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "The property WithIndexerType.Item is not supported.\r\n" +
                                 "The property is of type int.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use IEqualByPropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude the type WithIndexerType.\r\n" +
                                 "  - Exclude the property WithIndexerType.Item.\r\n";
            var source = new EqualByTypes.WithIndexerType();
            var target = new EqualByTypes.WithIndexerType();

            var exception = Assert.Throws<NotSupportedException>(() => this.EqualByMethod(source, target, ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }
    }
}