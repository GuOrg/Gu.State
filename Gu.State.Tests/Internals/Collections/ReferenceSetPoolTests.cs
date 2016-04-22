namespace Gu.State.Tests.Internals.Collections
{
    using System.Collections.Generic;

    using NUnit.Framework;

    public class ReferenceSetPoolTests
    {
        [Test]
        public void BorrowTwiceReturnsDifferent()
        {
            using (Disposer<HashSet<object>> disposer1 = ReferenceSetPool<object>.Borrow())
            {
                using (Disposer<HashSet<object>> disposer2 = ReferenceSetPool<object>.Borrow())
                {
                    Assert.AreNotSame(disposer1.Value, disposer2.Value);
                }
            }
        }

        [Test]
        public void BorrowTwiceReturnsSame()
        {
            HashSet<object> set;
            using (var disposer = ReferenceSetPool<object>.Borrow())
            {
                disposer.Value.Add(new object());
                set = disposer.Value;
            }

            using (var disposer = ReferenceSetPool<object>.Borrow())
            {
                CollectionAssert.IsEmpty(disposer.Value);
                Assert.AreSame(set, disposer.Value);
            }
        }
    }
}