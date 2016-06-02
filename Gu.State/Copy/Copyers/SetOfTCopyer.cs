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

        public void Copy<TSettings>(
            object source,
            object target,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var itemType = source.GetType().GetItemType();
            var copyMethod = this.GetType()
                                        .GetMethod(nameof(SetOfTCopyer.Copy), BindingFlags.NonPublic | BindingFlags.Static)
                                        .MakeGenericMethod(itemType, typeof(TSettings));
            copyMethod.Invoke(null, new[] { source, target, settings, referencePairs });
        }

        private static void Copy<T, TSettings>(
            ISet<T> source,
            ISet<T> target,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            if (Is.IsFixedSize(source, target) && source.Count != target.Count)
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

                    var copyIngComparer = new CopyIngComparer<T, TSettings>(comparer, settings, referencePairs);
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

        private class CopyIngComparer<T, TSettings> : IEqualityComparer<T>
             where TSettings : class, IMemberSettings
        {
            private readonly IEqualityComparer<T> inner;
            private readonly TSettings settings;
            private readonly ReferencePairCollection referencePairs;

            private bool isCopying;

            public CopyIngComparer(
                IEqualityComparer<T> inner,
                TSettings settings,
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
