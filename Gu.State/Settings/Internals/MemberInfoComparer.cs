namespace Gu.State.Internals
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <inheritdoc />
    internal sealed class MemberInfoComparer<T> : IEqualityComparer<T>
        where T : MemberInfo
    {
        internal static readonly MemberInfoComparer<T> Default = new MemberInfoComparer<T>();

        private MemberInfoComparer()
        {
        }

        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            return x.Name == y.Name && x.DeclaringType == y.DeclaringType;
        }

        /// <inheritdoc />
        public int GetHashCode(T obj)
        {
            // http://stackoverflow.com/a/263416/1069200
            unchecked
            {
                var hash = (int)2166136261;
                hash = hash * 16777619 ^ obj.Name.GetHashCode();
                hash = hash * 16777619 ^ (obj.DeclaringType?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
