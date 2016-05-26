namespace Gu.State
{
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
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var itemType = source.GetType().GetItemType();
            var copyMethod = this.GetType()
                                        .GetMethod(nameof(Copy), BindingFlags.NonPublic | BindingFlags.Static)
                                        .MakeGenericMethod(itemType, typeof(TSettings));
            copyMethod.Invoke(null, new[] { source, target, settings, referencePairs });
        }

        private static void Copy<T, TSettings>(
            IList<T> source,
            IList<T> target,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            if (Is.IsFixedSize(source, target) && source.Count != target.Count)
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(source, target, settings);
            }

            var copyValues = State.Copy.IsCopyValue(
                        source.GetType().GetItemType(),
                        settings);
            for (var i = 0; i < source.Count; i++)
            {
                if (copyValues)
                {
                    target.SetElementAt(i, source[i]);
                    continue;
                }

                var sv = source[i];
                var tv = target.ElementAtOrDefault(i);
                bool created;
                bool needsSync;
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out created, out needsSync);
                if (created)
                {
                    target.SetElementAt(i, clone);
                }

                if (needsSync)
                {
                    State.Copy.Sync(sv, clone, settings, referencePairs);
                }
            }

            target.TrimLengthTo(source);
        }
    }
}