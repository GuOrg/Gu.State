namespace Gu.State
{
    using System;
    using System.Reflection;

    internal class ArrayCopyer : ICopyer
    {
        private static readonly ArrayCopyer Default = new ArrayCopyer();

        private ArrayCopyer()
        {
        }

        public static bool TryGetOrCreate(object x, object y, out ICopyer comparer)
        {
            if (Is.Type<Array>(x, y))
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
            Copy((Array)source, (Array)target, settings, referencePairs);
        }

        private static void Copy<TSettings>(
            Array sourceArray,
            Array targetArray,
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
                copyMethod.Invoke(null, new object[] { sourceArray, targetArray, settings, referencePairs });
            }
            else
            {
                CopyAnyDimension(sourceArray, targetArray, settings, referencePairs);
            }
        }

        private static void Copy1DItems<T, TSettings>(
            T[] sourceArray,
            T[] targetArray,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var copyValues = State.Copy.IsCopyValue(
                        sourceArray.GetType().GetItemType(),
                        settings);
            for (var i = 0; i < sourceArray.Length; i++)
            {
                if (copyValues)
                {
                    targetArray[i] = sourceArray[i];
                    continue;
                }

                var sv = sourceArray[i];
                var tv = targetArray[i];
                bool created;
                bool needsSync;
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out created, out needsSync);
                if (created)
                {
                    targetArray[i] = clone;
                }

                if (needsSync)
                {
                    State.Copy.Sync(sv, clone, settings, referencePairs);
                }
            }
        }

        private static void Copy2DItems<T, TSettings>(
            T[,] sourceArray,
            T[,] targetArray,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var copyValues = State.Copy.IsCopyValue(
                        sourceArray.GetType().GetItemType(),
                        settings);
            for (var i = sourceArray.GetLowerBound(0); i <= sourceArray.GetUpperBound(0); i++)
            {
                for (var j = sourceArray.GetLowerBound(1); j <= sourceArray.GetUpperBound(1); j++)
                {
                    if (copyValues)
                    {
                        targetArray[i, j] = sourceArray[i, j];
                        continue;
                    }

                    var sv = sourceArray[i, j];
                    var tv = targetArray[i, j];
                    bool created;
                    bool needsSync;
                    var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out created, out needsSync);
                    if (created)
                    {
                        targetArray[i, j] = clone;
                    }

                    if (needsSync)
                    {
                        State.Copy.Sync(sv, clone, settings, referencePairs);
                    }
                }
            }
        }

        private static void Copy3DItems<T, TSettings>(
            T[,,] sourceArray,
            T[,,] targetArray,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var copyValues = State.Copy.IsCopyValue(
                        sourceArray.GetType().GetItemType(),
                        settings);
            for (var i = sourceArray.GetLowerBound(0); i <= sourceArray.GetUpperBound(0); i++)
            {
                for (var j = sourceArray.GetLowerBound(1); j <= sourceArray.GetUpperBound(1); j++)
                {
                    for (var k = sourceArray.GetLowerBound(2); k <= sourceArray.GetUpperBound(2); k++)
                    {
                        if (copyValues)
                        {
                            targetArray[i, j, k] = sourceArray[i, j, k];
                            continue;
                        }

                        var sv = sourceArray[i, j, k];
                        var tv = targetArray[i, j, k];
                        bool created;
                        bool needsSync;
                        var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out created, out needsSync);
                        if (created)
                        {
                            targetArray[i, j, k] = clone;
                        }

                        if (needsSync)
                        {
                            State.Copy.Sync(sv, clone, settings, referencePairs);
                        }
                    }
                }
            }
        }

        private static void CopyAnyDimension<TSettings>(
            Array sourceArray,
            Array targetArray,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings
        {
            var copyValues = State.Copy.IsCopyValue(
                        sourceArray.GetType().GetItemType(),
                        settings);
            foreach (var index in sourceArray.Indices())
            {
                if (copyValues)
                {
                    targetArray.SetValue(sourceArray.GetValue(index), index);
                    continue;
                }

                var sv = sourceArray.GetValue(index);
                var tv = targetArray.GetValue(index);
                bool created;
                bool needsSync;
                var clone = State.Copy.CloneWithoutSync(sv, tv, settings, out created, out needsSync);
                if (created)
                {
                    targetArray.SetValue(clone, index);
                }

                if (needsSync)
                {
                    State.Copy.Sync(sv, clone, settings, referencePairs);
                }
            }
        }
    }
}
