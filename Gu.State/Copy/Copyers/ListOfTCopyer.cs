namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ListOfTCopyer : ICopyer
    {
        public static readonly ListOfTCopyer Default = new ListOfTCopyer();

        private ListOfTCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (Is.IListsOfT(x, y))
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
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var itemType = source.GetType().GetItemType();
            var copyMethod = this.GetType()
                                        .GetMethod(nameof(Copy), BindingFlags.NonPublic | BindingFlags.Static)
                                        .MakeGenericMethod(itemType, typeof(TSettings));
            copyMethod.Invoke(null, new[] { source, target, copyItem, settings, referencePairs });
        }

        private static void Copy<T, TSettings>(
            IList<T> source,
            IList<T> target,
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            if (Is.IsFixedSize(source, target) && source.Count != target.Count)
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(source, target, settings);
            }

            var isImmutable = settings.IsImmutable(
                source.GetType()
                      .GetItemType());
            for (var i = 0; i < source.Count; i++)
            {
                var sv = source[i];
                var tv = target.ElementAtOrDefault(i);
                var copy = State.Copy.Item(sv, tv, copyItem, settings, referencePairs, isImmutable);
                target.SetElementAt(i, copy);
            }

            target.TryTrimLengthTo(source);
        }
    }
}