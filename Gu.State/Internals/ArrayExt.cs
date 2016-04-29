namespace Gu.State
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class ArrayExt
    {
        internal static IEnumerable<int> Indices(this Array array, int dimension)
        {
            for (var i = array.GetLowerBound(dimension); i <= array.GetUpperBound(dimension); i++)
            {
                yield return i;
            }
        }

        internal static IEnumerable<int[]> Indices(this Array array)
        {
            var indices = new int[array.Rank];
            return GetIndices(indices, array, 0);
        }

        private static IEnumerable<int[]> GetIndices(int[] indices, Array array, int dimension)
        {
            foreach (var index in array.Indices(dimension))
            {
                indices[dimension] = index;
                if (dimension == array.Rank - 1)
                {
                    yield return indices.ToArray();
                }
                else
                {
                    foreach (var nested in GetIndices(indices, array, dimension+1))
                    {
                        yield return nested;
                    }
                }
            }
        }
    }
}
