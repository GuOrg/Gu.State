namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal static class ISetEqualByComparer
    {
        internal static bool TryCreate(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.Implements(typeof(IEnumerable<>)) &&
                type.Namespace?.StartsWith("System.", StringComparison.Ordinal) == true &&
                type.Name.EndsWith("Set`1", StringComparison.Ordinal))
            {
                comparer = Activator.CreateInstance<EqualByComparer>(
                    typeof(EqualByComparer<,>).MakeGenericType(type, type.GetItemType()),
                    new object[] { settings.GetEqualByComparerOrDeferred(type.GetItemType()) });
                return true;
            }

            comparer = null;
            return false;
        }

        internal class EqualByComparer<TSet, TItem> : CollectionEqualByComparer<TSet, TItem>
            where TSet : IEnumerable<TItem>
        {
            internal EqualByComparer(EqualByComparer itemComparer)
                : base(itemComparer)
            {
            }

            internal override bool Equals(TSet xs, TSet ys, MemberSettings settings, HashSet<ReferencePairStruct> referencePairs)
            {
                // Not using pattern matching here as AppVeyor does not yet have a VS2019 image.
                if (xs is HashSet<TItem> hashSet &&
                    typeof(TItem).IsSealed &&
                    ReferenceEquals(hashSet.Comparer, EqualityComparer<TItem>.Default) &&
                    settings.IsEquatable(typeof(TItem)))
                {
                    return hashSet.SetEquals(ys);
                }

                if (this.ItemComparer is ReferenceEqualByComparer)
                {
                    using var borrowed = HashSetPool<TItem>.Borrow(
                        (x, y) => ReferenceEquals(x, y),
                        x => RuntimeHelpers.GetHashCode(x));
                    borrowed.Value.UnionWith(xs);
                    return borrowed.Value.SetEquals(ys);
                }

                if (typeof(TItem).IsSealed &&
                    settings.IsEquatable(typeof(TItem)))
                {
                    using var borrowed = HashSetPool<TItem>.Borrow(
                        (x, y) => this.ItemComparer.Equals(x, y, settings, referencePairs),
                        x => x.GetHashCode());
                    borrowed.Value.UnionWith(xs);
                    return borrowed.Value.SetEquals(ys);
                }

                using (var borrowed = HashSetPool<TItem>.Borrow(
                    (x, y) => this.ItemComparer.Equals(x, y, settings, referencePairs),
                    x => 0))
                {
                    borrowed.Value.UnionWith(xs);
                    return borrowed.Value.SetEquals(ys);
                }
            }
        }
    }
}
