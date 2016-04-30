namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

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
            if (Is.FixedSize(source, target) && source.Count != target.Count)
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(source, target, settings);
            }

            if (settings.IsImmutable(typeof(T)))
            {
                target.IntersectWith(source);
                target.UnionWith(source);
                return;
            }

            switch (settings.ReferenceHandling)
            {
                case ReferenceHandling.Throw:
                    break;
                case ReferenceHandling.References:
                        target.IntersectWith(source);
                        target.UnionWith(source);
                    break;
                case ReferenceHandling.Structural:
                case ReferenceHandling.StructuralWithReferenceLoops:
                    target.IntersectWith(source);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
