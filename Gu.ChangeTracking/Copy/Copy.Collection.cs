namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Linq;

    public static partial class Copy
    {
        private static void CopyCollectionItems<T>(object source, object target, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
             where T : class, IMemberSettings
        {
            var sl = source as IList;
            var tl = target as IList;
            if (sl != null && tl != null)
            {
                if ((sl.IsFixedSize || tl.IsFixedSize) &&
                    sl.Count != tl.Count)
                {
                    Throw.CannotCopyFixesSizeCollections(sl, tl, settings);
                }

                Collection.CopyListItems(sl, tl, syncItem, settings, referencePairs);
                return;
            }

            var sd = source as IDictionary;
            var td = target as IDictionary;
            if (sd != null && td != null)
            {
                if ((sd.IsFixedSize || td.IsFixedSize) &&
                    sd.Count != td.Count)
                {
                    Throw.CannotCopyFixesSizeCollections(sd, td, settings);
                }

                Collection.CopyDictionaryItems(sd, td, syncItem, settings, referencePairs);
                return;
            }

            if (source is IEnumerable || target is IEnumerable)
            {
                throw ChangeTracking.Throw.ShouldNeverGetHere("Should be checked before");
            }
        }

        private static class Collection
        {
            internal static void CopyListItems<T>(IList sourceList, IList targetList, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
                where T : class, IMemberSettings
            {
                for (int i = 0; i < sourceList.Count; i++)
                {
                    var sv = sourceList[i];
                    if (sv == null)
                    {
                        SetListItem(targetList, i, null);
                        continue;
                    }

                    if (IsCopyableType(sv.GetType()))
                    {
                        SetListItem(targetList, i, sv);
                        continue;
                    }

                    var tv = targetList.Count > i ? targetList[i] : null;
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.References:
                            if (ReferenceEquals(sv, tv))
                            {
                                continue;
                            }

                            SetListItem(targetList, i, sv);
                            continue;
                        case ReferenceHandling.Structural:
                        case ReferenceHandling.StructuralWithReferenceLoops:
                            if (tv == null)
                            {
                                tv = CreateInstance(sv, null, settings);
                                SetListItem(targetList, i, tv);
                            }

                            syncItem(sv, tv, settings, referencePairs);
                            continue;
                        case ReferenceHandling.Throw:
                            throw ChangeTracking.Throw.ShouldNeverGetHere();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                    }
                }

                while (targetList.Count > sourceList.Count)
                {
                    targetList.RemoveAt(targetList.Count - 1);
                }
            }

            internal static void CopyDictionaryItems<T>(IDictionary sourceDict, IDictionary targetDict, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
                where T : class, IMemberSettings
            {
                foreach (var key in sourceDict.Keys)
                {
                    var sv = sourceDict[key];
                    if (sv == null)
                    {
                        targetDict[key] = null;
                        continue;
                    }

                    if (IsCopyableType(sv.GetType()))
                    {
                        targetDict[key] = sv;
                        continue;
                    }

                    var tv = targetDict.Contains(key) ? targetDict[key] : null;
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.References:
                            if (ReferenceEquals(sv, tv))
                            {
                                continue;
                            }

                            targetDict[key] = sv;
                            continue;
                        case ReferenceHandling.Structural:
                        case ReferenceHandling.StructuralWithReferenceLoops:
                            if (tv == null)
                            {
                                tv = CreateInstance(sv, null, settings);
                                targetDict[key] = tv;
                            }

                            syncItem(sv, tv, settings, referencePairs);
                            continue;
                        case ReferenceHandling.Throw:
                            throw ChangeTracking.Throw.ShouldNeverGetHere();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                    }
                }

                var toRemove = targetDict.Keys.Cast<object>()
                                         .Where(x => !sourceDict.Contains(x))
                                         .ToList();
                foreach (var key in toRemove)
                {
                    targetDict.Remove(key);
                }
            }

            private static void SetListItem(IList targetList, int index, object item)
            {
                if (targetList.Count > index)
                {
                    targetList[index] = item;
                }
                else
                {
                    targetList.Insert(index, item);
                }
            }
        }
    }
}
