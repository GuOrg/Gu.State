namespace Gu.State.Tests.Sandbox
{
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    using NUnit.Framework;

    public class GetOrCreate
    {
        [Test]
        public void ConcurrentDictionary()
        {
            var dict = new ConcurrentDictionary<object, object>();
            object o1 = null;
            var o2 = dict.GetOrAdd(
                1,
                x =>
                    {
                        o1 = dict.GetOrAdd(x, __ => new object());
                        return new object();
                    });

            Assert.AreSame(o1, o2);
        }

        [Test]
        public void ConditionalWeakTable()
        {
            var dict = new ConditionalWeakTable<object, object>();
            object o1 = null;
            var o2 = dict.GetValue(
                1,
                x =>
                {
                    o1 = dict.GetValue(x, __ => new object());
                    return new object();
                });

            Assert.AreSame(o1, o2);
        }
    }
}
