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
            if (referenceHandling != null)
            {
                Assert.Throws<NotSupportedException>(() => this.VerifyMethod<CopyTypes.WithComplexProperty>(referenceHandling.Value));
            }
            else
            {
                Assert.Throws<NotSupportedException>(this.VerifyMethod<CopyTypes.WithComplexProperty>);
            }
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
