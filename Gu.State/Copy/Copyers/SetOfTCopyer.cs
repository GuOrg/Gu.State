namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal class SetOfTCopyer : ICopyer
    {
        public static readonly SetOfTCopyer Default = new SetOfTCopyer();

        private SetOfTCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (Is.ISetsOfT(x, y))
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        public void Copy(
            object source,
            object target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            var itemType = source.GetType().GetItemType();
            var copyMethod = this.GetType()
                                        .GetMethod(nameof(SetOfTCopyer.Copy), BindingFlags.NonPublic | BindingFlags.Static)
                                        .MakeGenericMethod(itemType);
            copyMethod.Invoke(null, new[] { source, target, settings, referencePairs });
        }

        private static void Copy<T>(
            ISet<T> source,
            ISet<T> target,
            MemberSettings settings,
            ReferencePairCollection referencePairs)
        {
            if (Is.FixedSize(source, target) && source.Count != target.Count)
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(source, target, settings);
            }

            if (settings.IsImmutable(typeof(T)))
            {
                using (var borrow = HashSetPool<T>.Borrow(EqualityComparer<T>.Default))
                {
                    borrow.Value.UnionWith(source);
                    target.IntersectWith(borrow.Value);
                    target.UnionWith(borrow.Value);
                    return;
                }
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                    break;
                case ReferenceHandling.References:
                    using (var borrow = HashSetPool<T>.Borrow((x, y) => ReferenceEquals(x, y), x => RuntimeHelpers.GetHashCode(x)))
                    {
                        borrow.Value.UnionWith(source);
                        target.IntersectWith(borrow.Value);
                        target.UnionWith(borrow.Value);
                    }

                    break;
                case ReferenceHandling.Structural:
                    IEqualityComparer<T> comparer;
                    if (!Set.TryGetComparer(source, out comparer))
                    {
                        comparer = EqualityComparer<T>.Default;
                    }

                    var copyIngComparer = new CopyingComparer<T>(comparer, settings, referencePairs);
                    using (var borrow = HashSetPool<T>.Borrow(copyIngComparer))
                    {
                        borrow.Value.UnionWith(source);
                        target.IntersectWith(borrow.Value);
                        copyIngComparer.StartCopying();
                        target.UnionWith(borrow.Value);
                    }

                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private class CopyingComparer<T> : IEqualityComparer<T>
        {
            private readonly IEqualityComparer<T> inner;
            private readonly MemberSettings settings;
            private readonly ReferencePairCollection referencePairs;

            private bool isCopying;

            public CopyingComparer(
                IEqualityComparer<T> inner,
                MemberSettings settings,
                ReferencePairCollection referencePairs)
            {
                this.inner = inner;
                this.settings = settings;
                this.referencePairs = referencePairs;
            }

            public bool Equals(T x, T y)
            {
                var result = this.inner.Equals(x, y);
                if (result && this.isCopying)
                {
                    State.Copy.Sync(x, y, this.settings, this.referencePairs);
                }

                return result;
            }

            public int GetHashCode(T obj)
            {
                return this.inner.GetHashCode(obj);
            }

            internal void StartCopying()
            {
                this.isCopying = true;
            }
        }
    }
}
