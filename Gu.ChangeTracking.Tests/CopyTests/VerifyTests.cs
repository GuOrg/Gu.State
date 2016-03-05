namespace Gu.ChangeTracking.Tests.CopyTests
{
    using System;
    using System.Collections.Generic;

    using NUnit.Framework;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>() where T : class;

        public abstract void VerifyMethod<T>(ReferenceHandling referenceHandling) where T : class;

        [Test]
        public void CanCopyHappyPath()
        {
            this.VerifyMethod<CopyTypes.WithSimpleProperties>();
        }

        [Test]
        public void CanCopyWithCalculatedProperty()
        {
            this.VerifyMethod<CopyTypes.WithCalculatedProperty>();
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        public void CanCopyWithComplexThrows(ReferenceHandling? referenceHandling)
        {
            var expected = this.GetType() == typeof(FieldValues.Verify)
                   ? "Copy.FieldValues(x, y) failed.\r\n" +
                     "The field WithComplexProperty.<ComplexType>k__BackingField of type ComplexType is not supported.\r\n" +
                     "Solve the problem by any of:\r\n" +
                     "* Implement IEquatable<WithComplexProperty> for WithComplexProperty or use a type that does.\r\n" +
                     "* Implement IEquatable<ComplexType> for ComplexType or use a type that does.\r\n" +
                     "* Use FieldsSettings and specify how comparing is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a deep equals is performed.\r\n" +
                     "  - ReferenceHandling.StructuralWithReferenceLoops means that a deep equals that handles reference loops is performed.\r\n" +
                     "  - ReferenceHandling.References means that reference equality is used.\r\n" +
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
                     "* Use PropertiesSettings and specify how comparing is performed:\r\n" +
                     "  - ReferenceHandling.Structural means that a the entire graph is traversed and immutable property values are copied.\r\n" +
                     "  - ReferenceHandling.StructuralWithReferenceLoops same as Structural but tracks reference loops.\r\n" +
                     "    - For structural Activator.CreateInstance is used to create instances so a parameterless constructor may be needed, can be private.\r\n" +
                     "  - ReferenceHandling.References means that reference equality is used.\r\n" +
                     "  - Exclude a combination of the following:\r\n" +
                     "    - The property WithComplexProperty.ComplexType.\r\n" +
                     "    - The type ComplexType.\r\n";
            var exception = referenceHandling != null
                                ? Assert.Throws<NotSupportedException>(() => this.VerifyMethod<CopyTypes.WithComplexProperty>(referenceHandling.Value))
                                : Assert.Throws<NotSupportedException>(this.VerifyMethod<CopyTypes.WithComplexProperty>);

            Assert.AreEqual(expected, exception.Message);
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyWithComplexDoesNotThrowWithReferenceHandling(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<CopyTypes.WithComplexProperty>(referenceHandling);
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        public void CanCopyListOfIntsThrows(ReferenceHandling? referenceHandling)
        {
            if (referenceHandling != null)
            {
                Assert.Throws<NotSupportedException>(() => this.VerifyMethod<List<int>>(referenceHandling.Value));
            }
            else
            {
                Assert.Throws<NotSupportedException>(this.VerifyMethod<List<int>>);
            }
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyEnumerableOfIntsThrows(ReferenceHandling? referenceHandling)
        {
            if (referenceHandling != null)
            {
                Assert.Throws<NotSupportedException>(() => this.VerifyMethod<IEnumerable<int>>(referenceHandling.Value));
            }
            else
            {
                Assert.Throws<NotSupportedException>(this.VerifyMethod<IEnumerable<int>>);
            }
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanCopyListOfIntsWithReferenceHandling(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<List<int>>(referenceHandling);
        }
    }
}
