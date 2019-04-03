namespace Gu.State
{
    using System;

    /// <inheritdoc />
    internal class ArrayEqualByComparer : EqualByComparer
    {
        /// <summary>The default instance.</summary>
        public static readonly ArrayEqualByComparer Default = new ArrayEqualByComparer();

        private ArrayEqualByComparer()
        {
        }

        /// <inheritdoc />
        public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            if (TryGetEitherNullEquals(x, y, out var result))
            {
                return result;
            }

            return Equals((Array)x, (Array)y, settings, referencePairs);
        }

        internal static bool TryGet(Type type, MemberSettings settings, out EqualByComparer comparer)
        {
            if (type.IsArray)
            {
                comparer = Default;
                return true;
            }

            comparer = null;
            return false;
        }

        private static bool Equals(Array x, Array y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            if (!Is.SameSize(x, y))
            {
                return false;
            }

            var comparer = settings.GetEqualByComparer(x.GetType().GetItemType());
            var xe = x.GetEnumerator();
            var ye = y.GetEnumerator();
            while (xe.MoveNext() && ye.MoveNext())
            {
                if (!comparer.Equals(xe.Current, ye.Current, settings, referencePairs))
                {
                    return false;
                }
            }

            return true;
        }
    }
}