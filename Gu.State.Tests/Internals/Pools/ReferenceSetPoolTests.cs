namespace Gu.State.Tests.Internals.Collections
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;

    public class ReferenceSetPoolTests
    {
        [Test]
        public void BorrowTwiceReturnsDifferent()
        {
            using var disposer1 = ReferenceSetPool<object>.Borrow();
            using var disposer2 = ReferenceSetPool<object>.Borrow();
            Assert.AreNotSame(disposer1.Value, disposer2.Value);
        }

        [Test]
        public void BorrowTwiceReturnsSame()
        {
            var value = (ConcurrentQueue<HashSet<object>>)typeof(ReferenceSetPool<object>)
                .GetField("Pool", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .GetValue(null);
            while (value.TryDequeue(out _))
            {
            }

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