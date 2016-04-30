namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class SetOfTCopyer : ICopyer
    {
        public static readonly State.SetOfTCopyer Default = new State.SetOfTCopyer();

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
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var itemType = source.GetType().GetItemType();
            var copyMethod = this.GetType()
                                        .GetMethod(nameof(State.Copy), BindingFlags.NonPublic | BindingFlags.Static)
                                        .MakeGenericMethod(itemType, typeof(TSettings));
            copyMethod.Invoke(null, new[] { source, target, syncItem, settings, referencePairs });
        }

        private static void Copy<T, TSettings>(
            ISet<T> source,
            ISet<T> target,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
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
                using (var borrow = SetPool<T>.Borrow(EqualityComparer<T>.Default))
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
                    using (var borrow = SetPool<T>.Borrow((x, y) => ReferenceEquals(x, y), x => RuntimeHelpers.GetHashCode(x)))
                    {
                        borrow.Value.UnionWith(source);
                        target.IntersectWith(borrow.Value);
                        target.UnionWith(borrow.Value);
                    }

                    break;
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    IEqualityComparer<T> comparer;
                    if (!Set.TryGetComparer(source, out comparer))
                    {
                        comparer = EqualityComparer<T>.Default;
                    }

                    var copyIngComparer = new CopyIngComparer<T, TSettings>(comparer, syncItem, settings, referencePairs);
                    using (var borrow = SetPool<T>.Borrow(copyIngComparer))
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
            private readonly Action<object, object, TSettings, ReferencePairCollection> syncItem;
            private readonly TSettings settings;
            private readonly ReferencePairCollection referencePairs;

            private bool isCopying;

            public CopyIngComparer(
                IEqualityComparer<T> inner,
                Action<object, object, TSettings, ReferencePairCollection> syncItem,
                TSettings settings,
                ReferencePairCollection referencePairs)
            {
                this.inner = inner;
                this.syncItem = syncItem;
                this.settings = settings;
                this.referencePairs = referencePairs;
            }

            public bool Equals(T x, T y)
            {
                var result = this.inner.Equals(x, y);
                if (result && this.isCopying)
                {
                    State.Copy.Item(x, y, this.syncItem, this.settings, this.referencePairs, false);
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
