namespace Gu.ChangeTracking.Tests.EqualByTests
{
    using System;
    using System.Collections.Generic;
    using Gu.ChangeTracking.Tests.CopyTests;
    using NUnit.Framework;

    public abstract class VerifyTests
    {
        public abstract void VerifyMethod<T>() where T : class;

        public abstract void VerifyMethod<T>(ReferenceHandling referenceHandling) where T : class;

        [Test]
        public void CanEquateWithSimple()
        {
            this.VerifyMethod<EqualByTypes.WithSimpleProperties>();
        }

        [Test]
        public void CanEquateWithCalculatedProperty()
        {
            this.VerifyMethod<EqualByTypes.WithCalculatedProperty>();
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        public void CanEquateWithComplexThrows(ReferenceHandling? referenceHandling)
        {
            if (referenceHandling != null)
            {
                Assert.Throws<NotSupportedException>(() => this.VerifyMethod<EqualByTypes.WithComplexProperty>(referenceHandling.Value));
            }
            else
            {
                Assert.Throws<NotSupportedException>(this.VerifyMethod<EqualByTypes.WithComplexProperty>);
            }
        }

        [TestCase(ReferenceHandling.Structural)]
        [TestCase(ReferenceHandling.References)]
        public void CanEquateWithComplexDoesNotThrowWithReferenceHandling(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<EqualByTypes.WithComplexProperty>(referenceHandling);
        }

        [TestCase(null)]
        [TestCase(ReferenceHandling.Throw)]
        public void CanEquateListOfIntsThrows(ReferenceHandling? referenceHandling)
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
        public void CanEquateEnumerableOfIntsThrows(ReferenceHandling? referenceHandling)
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
        public void CanEquateListOfIntsWithReferenceHandling(ReferenceHandling referenceHandling)
        {
            this.VerifyMethod<List<int>>(referenceHandling);
        }
    }
}
