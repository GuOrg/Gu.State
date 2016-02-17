namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;

    public static partial class Copy
    {
        private static void SyncLists<T>(IList sourceList, IList targetList, Action<object, object, T> syncItem, T settings)
            where T : CopySettings
        {
            for (int i = 0; i < sourceList.Count; i++)
            {
                var sv = sourceList[i];
                if (sv == null)
                {
                    SetItem(targetList, i, null);
                    continue;
                }

                if (!IsCopyableType(sv.GetType()))
                {
                    var tv = targetList.Count > i ? targetList[i] : null;
                    switch (settings.ReferenceHandling)
                    {
                        case ReferenceHandling.Reference:
                            if (ReferenceEquals(sv, tv))
                            {
                                continue;
                            }

                            SetItem(targetList, i, sv);
                            continue;
                        case ReferenceHandling.Structural:
                            if (tv == null)
                            {
                                tv = Activator.CreateInstance(sv.GetType(), true);
                                SetItem(targetList, i, tv);
                            }

                            syncItem(sv, tv, settings);
                            continue;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(settings.ReferenceHandling), settings.ReferenceHandling, null);
                    }
                }
                else
                {
                    SetItem(targetList, i, sv);
                    continue;
                }
            }

            while (targetList.Count > sourceList.Count)
            {
                targetList.RemoveAt(targetList.Count - 1);
            }
        }

        private static void SetItem(IList targetList, int index, object item)
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

        internal static bool IsCopyableType(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }
    }
}
