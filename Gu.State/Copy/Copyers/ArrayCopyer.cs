namespace Gu.State
{
    using System;
    using System.Reflection;

    public class ArrayCopyer : ICopyer
    {
        public static readonly ArrayCopyer Default = new ArrayCopyer();

        private ArrayCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (x is Array && y is Array)
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
            Copy((Array)source, (Array)target, syncItem, settings, referencePairs);
        }

        private static void Copy<TSettings>(
            Array sourceArray,
            Array targetArray,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            if (!Is.SameSize(sourceArray, targetArray))
            {
                throw State.Copy.Throw.CannotCopyFixesSizeCollections(sourceArray, targetArray, settings);
            }

            var itemType = sourceArray.GetType().GetItemType();
            var rank = sourceArray.Rank;
            if (rank < 4)
            {
                string methodName;
                switch (rank)
                {
                    case 1:
                        methodName = nameof(Copy1DItems);
                        break;
                    case 2:
                        methodName = nameof(Copy2DItems);
                        break;
                    case 3:
                        methodName = nameof(Copy3DItems);
                        break;
                    default:
                        throw Throw.ShouldNeverGetHereException("Expected rank {1, 2, 3} was : " + rank);
                }

                var copyMethod = typeof(ArrayCopyer).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)
                                                    .MakeGenericMethod(itemType, typeof(TSettings));
                copyMethod.Invoke(null, new object[] { sourceArray, targetArray, syncItem, settings, referencePairs });
            }
            else
            {
                CopyAnyDimension(sourceArray, targetArray, syncItem, settings, referencePairs);
            }
        }

        private static void Copy1DItems<T, TSettings>(
            T[] sourceArray,
            T[] targetArray,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var isImmutable = settings.IsImmutable(
                sourceArray.GetType()
                           .GetItemType());
            for (var i = 0; i < sourceArray.Length; i++)
            {
                var sv = sourceArray[i];
                var tv = targetArray[i];
                var copyItem = State.Copy.Item(sv, tv, syncItem, settings, referencePairs, isImmutable);
                targetArray[i] = copyItem;
            }
        }

        private static void Copy2DItems<T, TSettings>(
            T[,] sourceArray,
            T[,] targetArray,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var isImmutable = settings.IsImmutable(
                sourceArray.GetType()
                           .GetItemType());
            for (var i = sourceArray.GetLowerBound(0); i <= sourceArray.GetUpperBound(0); i++)
            {
                for (var j = sourceArray.GetLowerBound(1); j <= sourceArray.GetUpperBound(1); j++)
                {
                    var sv = sourceArray[i, j];
                    var tv = targetArray[i, j];
                    var copyItem = State.Copy.Item(sv, tv, syncItem, settings, referencePairs, isImmutable);
                    targetArray[i, j] = copyItem;
                }
            }
        }

        private static void Copy3DItems<T, TSettings>(
            T[,,] sourceArray,
            T[,,] targetArray,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var isImmutable = settings.IsImmutable(
                sourceArray.GetType()
                           .GetItemType());
            for (var i = sourceArray.GetLowerBound(0); i <= sourceArray.GetUpperBound(0); i++)
            {
                for (var j = sourceArray.GetLowerBound(1); j <= sourceArray.GetUpperBound(1); j++)
                {
                    for (var k = sourceArray.GetLowerBound(2); k <= sourceArray.GetUpperBound(2); k++)
                    {
                        var sv = sourceArray[i, j, k];
                        var tv = targetArray[i, j, k];
                        var copyItem = State.Copy.Item(sv, tv, syncItem, settings, referencePairs, isImmutable);
                        targetArray[i, j, k] = copyItem;
                    }
                }
            }
        }

        private static void CopyAnyDimension<TSettings>(
            Array sourceArray,
            Array targetArray,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var isImmutable = settings.IsImmutable(
                sourceArray.GetType()
                           .GetItemType());
            foreach (var index in sourceArray.Indices())
            {
                var sv = sourceArray.GetValue(index);
                var tv = targetArray.GetValue(index);
                var copyItem = State.Copy.Item(sv, tv, syncItem, settings, referencePairs, isImmutable);
                targetArray.SetValue(copyItem, index);
            }
        }
    }
}
