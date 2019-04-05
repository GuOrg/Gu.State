namespace Gu.State
{
    using System;

    internal class LazyEqualByComparer<T> : EqualByComparer<T>
    {
        private TypeAndComparer lazyTypeAndComparer;

        internal override bool Equals(T x, T y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            var currentType = x.GetType();
            if (currentType != this.lazyTypeAndComparer.Type)
            {
                this.lazyTypeAndComparer = new TypeAndComparer(currentType, settings.GetEqualByComparer(currentType));
            }

            var comparer = this.lazyTypeAndComparer.Comparer;
            if (comparer is EqualByComparer<T> genericComparer)
            {
                return genericComparer.Equals(x, y, settings, referencePairs);
            }

            return comparer.Equals(x, y, settings, referencePairs);
        }

        private struct TypeAndComparer
        {
            internal readonly Type Type;
            internal readonly EqualByComparer Comparer;

            public TypeAndComparer(Type type, EqualByComparer comparer)
            {
                this.Type = type;
                this.Comparer = comparer;
            }
        }
    }
}