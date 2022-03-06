// ReSharper disable RedundantArgumentDefaultValue
namespace Gu.State.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NUnit.Framework;

    using static EqualByTypes;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>(
            ReferenceHandling referenceHandling = ReferenceHandling.Structural,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null);

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void WithSimpleProperties(ReferenceHandling referenceHandling)
        {
            Assert.DoesNotThrow(() => this.VerifyMethod<WithSimpleProperties>(referenceHandling));
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.Throw)]
        public void ArrayOfint(ReferenceHandling referenceHandling)
        {
            Assert.DoesNotThrow(() => this.VerifyMethod<int[]>(referenceHandling));
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.Throw)]
        public void DictionaryOfIntAndString(ReferenceHandling referenceHandling)
        {
            Assert.DoesNotThrow(() => this.VerifyMethod<Dictionary<int, string>>(referenceHandling));
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void WithComplexType(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<With<ComplexType>>(referenceHandling);
        }

        [Test]
        public void WithComplexComplexTypeWhenReferenceHandlingThrow()
        {
            var expected = this is FieldValues.Verify
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "The field With<ComplexType>.<Value>k__BackingField of type ComplexType is not supported.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Implement IEquatable<With<ComplexType>> for With<ComplexType> or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The field With<ComplexType>.<Value>k__BackingField.\r\n" +
                                 "    - The type ComplexType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "The property With<ComplexType>.Value of type ComplexType is not supported.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Implement IEquatable<With<ComplexType>> for With<ComplexType> or use a type that does.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The property With<ComplexType>.Value.\r\n" +
                                 "    - The type ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<With<ComplexType>>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<ComplexType>());
        }

        [Test]
        public void ListOfComplexComplexTypeWhenReferenceHandlingThrow()
        {
            var expected = this is FieldValues.Verify
                               ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Use a type that implements IEquatable<> instead of List<ComplexType>.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The type ComplexType.\r\n"

                               : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                                 "Below are a couple of suggestions that may solve the problem:\r\n" +
                                 "* Use a type that implements IEquatable<> instead of List<ComplexType>.\r\n" +
                                 "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                                 "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                                 "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The type ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<List<ComplexType>>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<ComplexType>());
        }

        [Test]
        public void WithIndexer()
        {
            var expected = this is FieldValues.Verify
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

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithIllegalIndexer>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ListOfComplexType(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<List<ComplexType>>(referenceHandling);
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.Throw)]
        public void ListOfWithIllegalIndexer(ReferenceHandling referenceHandling)
        {
            switch (referenceHandling)
            {
                case ReferenceHandling.References:
                    Assert.DoesNotThrow(() => this.VerifyMethod<WithListProperty<WithIllegalIndexer>>(referenceHandling));
                    break;
                case ReferenceHandling.Structural:
                    {
                        var expected = this is FieldValues.Verify
                            ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                              "The field WithListProperty<WithIllegalIndexer>.<Items>k__BackingField of type List<WithIllegalIndexer> is not supported.\r\n" +
                              "Indexers are not supported.\r\n" +
                              "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Implement IEquatable<WithListProperty<WithIllegalIndexer>> for WithListProperty<WithIllegalIndexer> or use a type that does.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The field WithListProperty<WithIllegalIndexer>.<Items>k__BackingField.\r\n" +
                              "    - The indexer property WithIllegalIndexer[int].\r\n" +
                              "    - The type WithIllegalIndexer.\r\n"

                            : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                              "The property WithListProperty<WithIllegalIndexer>.Items of type List<WithIllegalIndexer> is not supported.\r\n" +
                              "Indexers are not supported.\r\n" +
                              "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Implement IEquatable<WithListProperty<WithIllegalIndexer>> for WithListProperty<WithIllegalIndexer> or use a type that does.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The property WithListProperty<WithIllegalIndexer>.Items.\r\n" +
                              "    - The indexer property WithIllegalIndexer[int].\r\n" +
                              "    - The type WithIllegalIndexer.\r\n";
                        var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithListProperty<WithIllegalIndexer>>(referenceHandling));
                        Assert.AreEqual(expected, exception.Message);
                        break;
                    }

                case ReferenceHandling.Throw:
                    {
                        var expected = this is FieldValues.Verify
                            ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                              "The field WithListProperty<WithIllegalIndexer>.<Items>k__BackingField of type List<WithIllegalIndexer> is not supported.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Implement IEquatable<WithListProperty<WithIllegalIndexer>> for WithListProperty<WithIllegalIndexer> or use a type that does.\r\n" +
                              "* Use a type that implements IEquatable<> instead of List<WithIllegalIndexer>.\r\n" +
                              "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                              "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                              "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The field WithListProperty<WithIllegalIndexer>.<Items>k__BackingField.\r\n" +
                              "    - The type List<WithIllegalIndexer>.\r\n"

                            : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                              "The property WithListProperty<WithIllegalIndexer>.Items of type List<WithIllegalIndexer> is not supported.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Implement IEquatable<WithListProperty<WithIllegalIndexer>> for WithListProperty<WithIllegalIndexer> or use a type that does.\r\n" +
                              "* Use a type that implements IEquatable<> instead of List<WithIllegalIndexer>.\r\n" +
                              "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                              "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                              "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The property WithListProperty<WithIllegalIndexer>.Items.\r\n" +
                              "    - The type List<WithIllegalIndexer>.\r\n";
                        var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithListProperty<WithIllegalIndexer>>(referenceHandling));
                        Assert.AreEqual(expected, exception.Message);
                        break;
                    }
            }
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.Throw)]
        public void DictionaryOfWithIllegalIndexerAndInt(ReferenceHandling referenceHandling)
        {
            switch (referenceHandling)
            {
                case ReferenceHandling.References:
                    Assert.DoesNotThrow(() => this.VerifyMethod<WithListProperty<WithIllegalIndexer>>(referenceHandling));
                    break;
                case ReferenceHandling.Structural:
                    {
                        var expected = this is FieldValues.Verify
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
                        var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Dictionary<WithIllegalIndexer, int>>(referenceHandling));
                        Assert.AreEqual(expected, exception.Message);
                        break;
                    }

                case ReferenceHandling.Throw:
                    {
                        var expected = this is FieldValues.Verify
                            ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Use a type that implements IEquatable<> instead of Dictionary<WithIllegalIndexer, int>.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                              "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                              "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The type WithIllegalIndexer.\r\n"

                            : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Use a type that implements IEquatable<> instead of Dictionary<WithIllegalIndexer, int>.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                              "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                              "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The type WithIllegalIndexer.\r\n";
                        var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Dictionary<WithIllegalIndexer, int>>(referenceHandling));
                        Assert.AreEqual(expected, exception.Message);
                        break;
                    }
            }
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.Throw)]
        public void DictionaryOfIntAndWithIllegalIndexer(ReferenceHandling referenceHandling)
        {
            switch (referenceHandling)
            {
                case ReferenceHandling.References:
                    Assert.DoesNotThrow(() => this.VerifyMethod<WithListProperty<WithIllegalIndexer>>(referenceHandling));
                    break;
                case ReferenceHandling.Structural:
                    {
                        var expected = this is FieldValues.Verify
                            ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                              "Indexers are not supported.\r\n" +
                              "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Use a type that implements IEquatable<> instead of Dictionary<int, WithIllegalIndexer>.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The indexer property WithIllegalIndexer[int].\r\n" +
                              "    - The type WithIllegalIndexer.\r\n"

                            : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                              "Indexers are not supported.\r\n" +
                              "  - The property WithIllegalIndexer[int] is an indexer and not supported.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Use a type that implements IEquatable<> instead of Dictionary<int, WithIllegalIndexer>.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The indexer property WithIllegalIndexer[int].\r\n" +
                              "    - The type WithIllegalIndexer.\r\n";
                        var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Dictionary<int, WithIllegalIndexer>>(referenceHandling));
                        Assert.AreEqual(expected, exception.Message);
                        break;
                    }

                case ReferenceHandling.Throw:
                    {
                        var expected = this is FieldValues.Verify
                            ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Use a type that implements IEquatable<> instead of Dictionary<int, WithIllegalIndexer>.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                              "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                              "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The type WithIllegalIndexer.\r\n"

                            : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                              "Below are a couple of suggestions that may solve the problem:\r\n" +
                              "* Use a type that implements IEquatable<> instead of Dictionary<int, WithIllegalIndexer>.\r\n" +
                              "* Implement IEquatable<WithIllegalIndexer> for WithIllegalIndexer or use a type that does.\r\n" +
                              "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                              "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                              "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                              "  - Exclude a combination of the following:\r\n" +
                              "    - The type WithIllegalIndexer.\r\n";
                        var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Dictionary<int, WithIllegalIndexer>>(referenceHandling));
                        Assert.AreEqual(expected, exception.Message);
                        break;
                    }
            }
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void ObservableCollectionOfComplexType(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<ObservableCollection<ComplexType>>(referenceHandling);
        }

        [Test]
        public void ThrowsForArrayOfComplex()
        {
            var expected = this is FieldValues.Verify
                ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<ComplexType[]> for ComplexType[] or use a type that does.\r\n" +
                  "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The type ComplexType.\r\n"

                : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Implement IEquatable<ComplexType[]> for ComplexType[] or use a type that does.\r\n" +
                  "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                  "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The type ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<ComplexType[]>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<ComplexType[]>(ReferenceHandling.References));
            Assert.DoesNotThrow(() => this.VerifyMethod<ComplexType[]>(ReferenceHandling.Structural));
        }

        [Test]
        public void ThrowsForDictionaryOfIntAndComplexType()
        {
            var expected = this is FieldValues.Verify
                ? "EqualBy.FieldValues(x, y) failed.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Use a type that implements IEquatable<> instead of Dictionary<int, ComplexType>.\r\n" +
                  "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                  "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The type ComplexType.\r\n"

                : "EqualBy.PropertyValues(x, y) failed.\r\n" +
                  "Below are a couple of suggestions that may solve the problem:\r\n" +
                  "* Use a type that implements IEquatable<> instead of Dictionary<int, ComplexType>.\r\n" +
                  "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                  "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                  "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                  "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                  "  - Exclude a combination of the following:\r\n" +
                  "    - The type ComplexType.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Dictionary<int, ComplexType>>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<Dictionary<int, ComplexType>>(ReferenceHandling.References));
            Assert.DoesNotThrow(() => this.VerifyMethod<Dictionary<int, ComplexType>>(ReferenceHandling.Structural));
        }

        [Test]
        public void ThrowsReferenceLoopWhenReferenceHandlingThrow()
        {
            var expected = this is FieldValues.Verify
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
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Parent>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<Parent>(ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.VerifyMethod<Parent>(ReferenceHandling.References));
        }
    }
}
