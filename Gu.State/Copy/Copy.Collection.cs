namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Linq;

    public static partial class Copy
    {
        private static void CopyCollectionItems<T>(object source, object target, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
             where T : class, IMemberSettings
        {
            if (!Is.Enumerable(source, target))
            {
                return;
            }

            if (settings.ReferenceHandling == ReferenceHandling.Throw)
            {
                throw Gu.State.Throw.ShouldNeverGetHereException("Should have been checked for throw before copy");
            }

            IList sl;
            IList tl;
            if (Try.CastAs(source, target, out sl, out tl))
            {
                if ((sl.IsFixedSize || tl.IsFixedSize) &&
                    sl.Count != tl.Count)
                {
                    Throw.CannotCopyFixesSizeCollections(sl, tl, settings);
                }

                Collection.CopyItems(sl, tl, syncItem, settings, referencePairs);
                return;
            }

            IDictionary sd;
            IDictionary td;
            if (Try.CastAs(source, target, out sd, out td))
            {
                if ((sd.IsFixedSize || td.IsFixedSize) &&
                    sd.Count != td.Count)
                {
                    Throw.CannotCopyFixesSizeCollections(sd, td, settings);
                }

                Collection.CopyItems(sd, td, syncItem, settings, referencePairs);
                return;
            }

            if (Is.Sets(source, target))
            {
                Collection.CopySetItems(source, target, syncItem, settings, referencePairs);
                return;
            }

            if (source is IEnumerable || target is IEnumerable)
            {
                throw State.Throw.ShouldNeverGetHereException("Should be checked before");
            }
        }

        private static class Collection
        {
            internal static void CopyItems<T>(IList sourceList, IList targetList, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
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

                    if (settings.IsImmutable(sv.GetType()))
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
                            throw State.Throw.ShouldNeverGetHereException();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                    }
                }

                while (targetList.Count > sourceList.Count)
                {
                    targetList.RemoveAt(targetList.Count - 1);
                }
            }

            internal static void CopyItems<T>(IDictionary sourceDict, IDictionary targetDict, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
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

                    if (settings.IsImmutable(sv.GetType()))
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
                            throw State.Throw.ShouldNeverGetHereException();
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

            internal static void CopySetItems<T>(object source, object target, Action<object, object, T, ReferencePairCollection> syncItem, T settings, ReferencePairCollection referencePairs)
                where T : class, IMemberSettings
            {
                if (settings.ReferenceHandling == ReferenceHandling.References ||
                    settings.IsImmutable(source.GetType().GetItemType()))
                {
                    Set.Clear(target);
                    Set.UnionWith(target, source);
                    return;
                }

                switch (settings.ReferenceHandling)
                {
                    case ReferenceHandling.Throw:
                        throw Gu.State.Throw.ShouldNeverGetHereException("Should have been checked for throw before copy");
                    case ReferenceHandling.References:
                        throw Gu.State.Throw.ShouldNeverGetHereException("Handled above");
                    case ReferenceHandling.Structural:
                    case ReferenceHandling.StructuralWithReferenceLoops:
                        Set.IntersectWith(target, source);
                        throw new NotImplementedException("message");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //foreach (var sv in ((IEnumerable)source).Cast<object>())
                //{
                //    if (sv == null)
                //    {
                //        Set.Add(target, null);
                //        continue;
                //    }

                //    var tv = targetDict.Contains(key)
                //                 ? targetDict[key]
                //                 : null;
                //    switch (settings.ReferenceHandling)
                //    {
                //        case ReferenceHandling.References:
                //            if (ReferenceEquals(sv, tv))
                //            {
                //                continue;
                //            }

                //            targetDict[key] = sv;
                //            continue;
                //        case ReferenceHandling.Structural:
                //        case ReferenceHandling.StructuralWithReferenceLoops:
                //            if (tv == null)
                //            {
                //                tv = CreateInstance(sv, null, settings);
                //                targetDict[key] = tv;
                //            }

                //            syncItem(sv, tv, settings, referencePairs);
                //            continue;
                //        case ReferenceHandling.Throw:
                //            throw State.Throw.ShouldNeverGetHereException();
                //        default:
                //            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                //    }
                //}
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
