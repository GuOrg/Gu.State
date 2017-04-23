namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <inheritdoc />
    internal class SetEqualByComparer<T> : EqualByComparer
    {
        /// <summary>The default instance.</summary>
        public static readonly SetEqualByComparer<T> Default = new SetEqualByComparer<T>();

        private SetEqualByComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals(
            object x,
            object y,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (TryGetEitherNullEquals(x, y, out bool result))
            {
                return result;
            }

            var xs = (ISet<T>)x;
            var ys = (ISet<T>)y;
            if (xs.Count != ys.Count)
            {
                return false;
            }

            var isEquatable = settings.IsEquatable(x.GetType().GetItemType());
            var xHashSet = xs as HashSet<T>;
            if (isEquatable)
            {
                if (Equals(xHashSet?.Comparer, EqualityComparer<T>.Default))
                {
                    return xs.SetEquals(ys);
                }

                return this.ItemsEquals(xs, ys, EqualityComparer<T>.Default.Equals, EqualityComparer<T>.Default.GetHashCode);
            }

            if (settings.ReferenceHandling == ReferenceHandling.References)
            {
                return this.ItemsEquals(xs, ys, (xi, yi) => ReferenceEquals(xi, yi), xi => RuntimeHelpers.GetHashCode(xi));
            }

            var hashCodeMethod = typeof(T).GetMethod(nameof(this.GetHashCode), new Type[0]);
            if (hashCodeMethod.DeclaringType == typeof(object))
            {
                return this.ItemsEquals(xs, ys, (xi, yi) => EqualBy.MemberValues(xi, yi, settings, referencePairs), _ => 0);
            }

            return this.ItemsEquals(xs, ys, (xi, yi) => EqualBy.MemberValues(xi, yi, settings, referencePairs), xi => xi.GetHashCode());
        }

        private bool ItemsEquals(ISet<T> x, ISet<T> y, Func<T, T, bool> compare, Func<T, int> getHashCode)
        {
            using (var borrow = HashSetPool<T>.Borrow(compare, getHashCode))
            {
                borrow.Value.UnionWith(x);
                var result = borrow.Value.SetEquals(y);
                return result;
            }
        }
    }
}