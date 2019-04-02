namespace Gu.State
{
    internal abstract class EqualByComparer<T> : EqualByComparer
    {
        public override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            if (TryGetEitherNullEquals(x, y, out var result))
            {
                return result;
            }

            return this.Equals((T)x, (T)y, settings, referencePairs);
        }

        public abstract bool Equals(T x, T y, MemberSettings settings, ReferencePairCollection referencePairs);
    }
}
