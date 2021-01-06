namespace Gu.State
{
    using System;
    using System.Collections.Generic;

    internal class DeferredEqualByComparer<T> : EqualByComparer<T>
    {
        private TypeAndComparer lazyTypeAndComparer;

        internal override bool CanHaveReferenceLoops => true;

        internal override bool Equals(T x, T y, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
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

        private readonly struct TypeAndComparer
        {
            internal readonly Type Type;
            internal readonly EqualByComparer Comparer;

            internal TypeAndComparer(Type type, EqualByComparer comparer)
            {
                this.Type = type;
                this.Comparer = comparer;
            }
        }
    }
}
