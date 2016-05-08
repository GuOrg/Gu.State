namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    internal class ConditionalWeakTable2D<TKey, TValue>
        where TKey : class
        where TValue : class
    {
        private readonly ConditionalWeakTable<TKey, ConditionalWeakTable<TKey, TValue>> xMap = new ConditionalWeakTable<TKey, ConditionalWeakTable<TKey, TValue>>();

        public TValue GetValue(TKey xKey, TKey yKey, Func<TValue> createValueCallback)
        {
            var yMap = this.xMap.GetValue(xKey, CreateYMap);
            return yMap.GetValue(yKey, _ => createValueCallback());
        }

        private static ConditionalWeakTable<TKey, TValue> CreateYMap(TKey _)
        {
            return new ConditionalWeakTable<TKey, TValue>();
        }
    }
}
