namespace Gu.State.Tests.CopyTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    using static CopyTypes;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>()
            where T : class;

        public abstract void VerifyMethod<T>(
            ReferenceHandling referenceHandling,
            string excludedMembers = null,
            Type ignoredType = null,
            Type immutableType = null)
            where T : class;

        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void WithSimpleProperties(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<WithSimpleProperties>(referenceHandling);
        }

        [Test]
        public void CanCopyWithCalculatedProperty()
        {
            this.VerifyMethod<WithCalculatedProperty>();
        }

        [TestCase(ReferenceHandling.Throw)]
        public void CanCopyWithComplexThrows(ReferenceHandling referenceHandling)
        {
            var expected = this is FieldValues.Verify
                   ? "Copy.VerifyCanCopyFieldValues(x, y) failed.\r\n" +
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

                   : "Copy.VerifyCanCopyPropertyValues(x, y) failed.\r\n" +
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
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithComplexProperty>(referenceHandling));
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyWithComplexDoesNotThrowWithReferenceHandling(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<WithComplexProperty>(referenceHandling);
        }

        [TestCase(ReferenceHandling.Throw)]
        public void CanCopyListOfComplexTypeThrows(ReferenceHandling? referenceHandling)
        {
            if (referenceHandling != null)
            {
                Assert.Throws<NotSupportedException>(() => this.VerifyMethod<List<ComplexType>>(referenceHandling.Value));
            }
            else
            {
                Assert.Throws<NotSupportedException>(this.VerifyMethod<List<ComplexType>>);
            }
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyEnumerableOfIntsThrows(ReferenceHandling? referenceHandling)
        {
            var expected = "Can only copy the members of collections implementing IList or IDictionary";
            var exception = referenceHandling != null
            ? Assert.Throws<NotSupportedException>(() => this.VerifyMethod<IEnumerable<int>>(referenceHandling.Value))
            : Assert.Throws<NotSupportedException>(this.VerifyMethod<IEnumerable<int>>);
            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyImmutableThrows(ReferenceHandling? referenceHandling)
        {
            var expected = "Cannot copy the members of an immutable object";
            var exception = referenceHandling != null
                ? Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Immutable>(referenceHandling.Value))
                : Assert.Throws<NotSupportedException>(this.VerifyMethod<Immutable>);

            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyListOfIntsWithReferenceHandling(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<List<int>>(referenceHandling);
        }

        [Test]
        public void WithIndexerThrows()
        {
            var expected = this is FieldValues.Verify
                               ? "Copy.VerifyCanCopyFieldValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIllegalIndexer.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use FieldsSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                                 "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIllegalIndexer.Item.\r\n"

                               : "Copy.VerifyCanCopyPropertyValues(x, y) failed.\r\n" +
                                 "Indexers are not supported.\r\n" +
                                 "  - The property WithIllegalIndexer.Item is an indexer and not supported.\r\n" +
                                 "Solve the problem by any of:\r\n" +
                                 "* Use PropertiesSettings and specify how copying is performed:\r\n" +
                                 "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                                 "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                                 "  - ReferenceHandling.References means that references are copied.\r\n" +
                                 "  - Exclude a combination of the following:\r\n" +
                                 "    - The indexer property WithIllegalIndexer.Item.\r\n";

            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<WithIllegalIndexer>(ReferenceHandling.Structural));
            Assert.AreEqual(expected, exception.Message);
        }

        [Test]
        public void DetectsReferenceLoop()
        {
            var expected = this is FieldValues.Verify
                   ? "Copy.VerifyCanCopyFieldValues(x, y) failed.\r\n" +
                     "The field Parent.child of type Child is in a reference loop.\r\n" +
                     "  - The loop is Parent.child.<Parent>k__BackingField.child...\r\n" +
                     "The field Parent.child of type Child is not supported.\r\n" +
                     "The field Child.<Parent>k__BackingField of type Parent is not supported.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Make Child immutable or use an immutable type.\r\n" +
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
                     "    - The field Parent.child.\r\n" +
                     "    - The field Child.<Parent>k__BackingField.\r\n" +
                     "    - The type Child.\r\n"

                   : "Copy.VerifyCanCopyPropertyValues(x, y) failed.\r\n" +
                     "The property Parent.Child of type Child is in a reference loop.\r\n" +
                     "  - The loop is Parent.Child.Parent.Child...\r\n" +
                     "The property Parent.Child of type Child is not supported.\r\n" +
                     "The property Child.Parent of type Parent is not supported.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Make Child immutable or use an immutable type.\r\n" +
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
                     "    - The property Parent.Child.\r\n" +
                     "    - The property Child.Parent.\r\n" +
                     "    - The type Child.\r\n";
            var exception = Assert.Throws<NotSupportedException>(() => this.VerifyMethod<Parent>(ReferenceHandling.Throw));
            Assert.AreEqual(expected, exception.Message);

            Assert.DoesNotThrow(() => this.VerifyMethod<Parent>(ReferenceHandling.Structural));
            Assert.DoesNotThrow(() => this.VerifyMethod<Parent>(ReferenceHandling.References));
        }

        [TestCase(ReferenceHandling.References)]
        [TestCase(ReferenceHandling.Structural)]
        public void WithExplicitImmutable(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<With<IntCollection>>(referenceHandling, immutableType: typeof(IntCollection));
        }
    }
}
