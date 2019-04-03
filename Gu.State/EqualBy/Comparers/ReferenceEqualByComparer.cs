namespace Gu.State
{
    internal class ReferenceEqualByComparer : EqualByComparer
    {
        public static readonly ReferenceEqualByComparer Default = new ReferenceEqualByComparer();

        internal override bool Equals(object x, object y, MemberSettings settings, ReferencePairCollection referencePairs)
        {
            return ReferenceEquals(x, y);
        }
    }
}